using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace PatientService.API.Controllers;

/// <summary>
/// Consent Management API - granular consent with QR code sharing
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ConsentController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly ILogger<ConsentController> _logger;

    public ConsentController(PatientDbContext context, ILogger<ConsentController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all consents for a patient (via query parameter)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ConsentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ConsentDto>>> GetConsentsByQuery([FromQuery] Guid patientId)
    {
        return await GetConsentsInternal(patientId);
    }

    /// <summary>
    /// Get all consents for a patient (via route parameter)
    /// </summary>
    [HttpGet("patient/{patientId}")]
    [ProducesResponseType(typeof(List<ConsentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ConsentDto>>> GetConsents(Guid patientId)
    {
        return await GetConsentsInternal(patientId);
    }

    private async Task<ActionResult<List<ConsentDto>>> GetConsentsInternal(Guid patientId)
    {
        var consents = await _context.Consents
            .Where(c => c.PatientId == patientId)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new ConsentDto
            {
                ConsentId = c.ConsentId,
                ConsentType = c.ConsentType,
                Status = c.Status,
                Scope = c.Scope,
                RecipientName = c.RecipientName,
                RecipientOrganization = c.RecipientOrganization,
                RecipientType = c.RecipientType,
                GrantedDate = c.GrantedDate,
                ExpiryDate = c.ExpiryDate,
                AllowDemographics = c.AllowDemographics,
                AllowAllergies = c.AllowAllergies,
                AllowConditions = c.AllowConditions,
                AllowImmunizations = c.AllowImmunizations,
                AllowMedications = c.AllowMedications,
                AllowInsurance = c.AllowInsurance,
                AllowDocuments = c.AllowDocuments,
                AllowEncounters = c.AllowEncounters,
                ShareToken = c.ShareToken,
                TokenExpiryDate = c.TokenExpiryDate,
                AccessCount = c.AccessCount,
                MaxAccessCount = c.MaxAccessCount
            })
            .ToListAsync();

        return Ok(consents);
    }

    /// <summary>
    /// Create new consent with optional QR sharing token
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ConsentDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ConsentDto>> CreateConsent([FromBody] CreateConsentRequest request)
    {
        var consent = new Consent
        {
            ConsentId = Guid.NewGuid(),
            PatientId = request.PatientId,
            ConsentType = request.ConsentType,
            Status = "Active",
            Scope = request.Scope,
            AllowDemographics = request.AllowDemographics,
            AllowAllergies = request.AllowAllergies,
            AllowConditions = request.AllowConditions,
            AllowImmunizations = request.AllowImmunizations,
            AllowMedications = request.AllowMedications,
            AllowInsurance = request.AllowInsurance,
            AllowDocuments = request.AllowDocuments,
            AllowEncounters = request.AllowEncounters,
            RecipientName = request.RecipientName,
            RecipientOrganization = request.RecipientOrganization,
            RecipientType = request.RecipientType,
            RecipientEmail = request.RecipientEmail,
            GrantedDate = DateTime.UtcNow,
            ExpiryDate = request.ExpiryDate,
            Purpose = request.Purpose,
            LegalBasis = request.LegalBasis,
            SignatureObtained = request.SignatureObtained,
            SignatureDate = request.SignatureObtained ? DateTime.UtcNow : null,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = User?.Identity?.Name ?? "system"
        };

        // Generate QR share token if requested
        if (request.GenerateShareToken)
        {
            consent.ShareToken = GenerateSecureToken();
            consent.TokenExpiryDate = request.TokenExpiryDate ?? DateTime.UtcNow.AddDays(30);
            consent.MaxAccessCount = request.MaxAccessCount;
        }

        _context.Consents.Add(consent);

        // Log consent creation
        var audit = new ConsentAudit
        {
            AuditId = Guid.NewGuid(),
            ConsentId = consent.ConsentId,
            Action = "Created",
            ActionDetails = $"Consent created with scope: {consent.Scope}",
            ActionDate = DateTime.UtcNow,
            ActorId = User?.Identity?.Name ?? "system",
            ActorName = User?.Identity?.Name ?? "System User",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        };

        _context.ConsentAudits.Add(audit);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created consent {ConsentId} for patient {PatientId}", 
            consent.ConsentId, request.PatientId);

        var dto = MapToDto(consent);

        return CreatedAtAction(nameof(GetConsent), new { consentId = consent.ConsentId }, dto);
    }

    /// <summary>
    /// Get consent by ID
    /// </summary>
    [HttpGet("{consentId}")]
    [ProducesResponseType(typeof(ConsentDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ConsentDto>> GetConsent(Guid consentId)
    {
        var consent = await _context.Consents.FindAsync(consentId);

        if (consent == null)
        {
            return NotFound();
        }

        return Ok(MapToDto(consent));
    }

    /// <summary>
    /// Get consent by share token (for QR code access)
    /// </summary>
    [HttpGet("token/{token}")]
    [ProducesResponseType(typeof(ConsentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status410Gone)]
    public async Task<ActionResult<ConsentDto>> GetConsentByToken(string token)
    {
        var consent = await _context.Consents
            .Include(c => c.Patient)
            .FirstOrDefaultAsync(c => c.ShareToken == token);

        if (consent == null)
        {
            return NotFound(new { message = "Invalid share token", traceId = HttpContext.TraceIdentifier });
        }

        // Validate token
        if (consent.Status != "Active")
        {
            return StatusCode(410, new { message = "Consent has been revoked", traceId = HttpContext.TraceIdentifier });
        }

        if (consent.TokenExpiryDate.HasValue && consent.TokenExpiryDate < DateTime.UtcNow)
        {
            return StatusCode(410, new { message = "Share token has expired", traceId = HttpContext.TraceIdentifier });
        }

        if (consent.MaxAccessCount.HasValue && consent.AccessCount >= consent.MaxAccessCount.Value)
        {
            return StatusCode(410, new { message = "Maximum access count reached", traceId = HttpContext.TraceIdentifier });
        }

        // Increment access count
        consent.AccessCount++;
        consent.LastAccessedAt = DateTime.UtcNow;
        consent.LastAccessedBy = HttpContext.Connection.RemoteIpAddress?.ToString();

        // Log access
        var audit = new ConsentAudit
        {
            AuditId = Guid.NewGuid(),
            ConsentId = consent.ConsentId,
            Action = "Accessed",
            ActionDetails = "Consent accessed via QR share token",
            ActionDate = DateTime.UtcNow,
            ActorId = "QR-Scanner",
            ActorName = "QR Code Access",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        };

        _context.ConsentAudits.Add(audit);
        await _context.SaveChangesAsync();

        return Ok(MapToDto(consent));
    }

    /// <summary>
    /// Revoke consent
    /// </summary>
    [HttpPost("{consentId}/revoke")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RevokeConsent(Guid consentId, [FromBody] RevokeConsentRequest request)
    {
        var consent = await _context.Consents.FindAsync(consentId);

        if (consent == null)
        {
            return NotFound();
        }

        consent.Status = "Revoked";
        consent.RevokedDate = DateTime.UtcNow;
        consent.RevocationReason = request.Reason;
        consent.UpdatedAt = DateTime.UtcNow;
        consent.UpdatedBy = User?.Identity?.Name ?? "system";

        // Log revocation
        var audit = new ConsentAudit
        {
            AuditId = Guid.NewGuid(),
            ConsentId = consent.ConsentId,
            Action = "Revoked",
            ActionDetails = $"Consent revoked. Reason: {request.Reason}",
            ActionDate = DateTime.UtcNow,
            ActorId = User?.Identity?.Name ?? "system",
            ActorName = User?.Identity?.Name ?? "System User",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        };

        _context.ConsentAudits.Add(audit);
        await _context.SaveChangesAsync();

        _logger.LogWarning("Revoked consent {ConsentId} for patient {PatientId}. Reason: {Reason}",
            consentId, consent.PatientId, request.Reason);

        return Ok();
    }

    /// <summary>
    /// Get consent audit trail
    /// </summary>
    [HttpGet("{consentId}/audit")]
    [ProducesResponseType(typeof(List<ConsentAuditDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ConsentAuditDto>>> GetConsentAudit(Guid consentId)
    {
        var audits = await _context.ConsentAudits
            .Where(a => a.ConsentId == consentId)
            .OrderByDescending(a => a.ActionDate)
            .Select(a => new ConsentAuditDto
            {
                AuditId = a.AuditId,
                Action = a.Action,
                ActionDetails = a.ActionDetails,
                ActionDate = a.ActionDate,
                ActorName = a.ActorName,
                ActorRole = a.ActorRole,
                IpAddress = a.IpAddress
            })
            .ToListAsync();

        return Ok(audits);
    }

    #region Helper Methods

    private static string GenerateSecureToken()
    {
        var bytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    private static ConsentDto MapToDto(Consent consent)
    {
        return new ConsentDto
        {
            ConsentId = consent.ConsentId,
            ConsentType = consent.ConsentType,
            Status = consent.Status,
            Scope = consent.Scope,
            RecipientName = consent.RecipientName,
            RecipientOrganization = consent.RecipientOrganization,
            RecipientType = consent.RecipientType,
            GrantedDate = consent.GrantedDate,
            ExpiryDate = consent.ExpiryDate,
            AllowDemographics = consent.AllowDemographics,
            AllowAllergies = consent.AllowAllergies,
            AllowConditions = consent.AllowConditions,
            AllowImmunizations = consent.AllowImmunizations,
            AllowMedications = consent.AllowMedications,
            AllowInsurance = consent.AllowInsurance,
            AllowDocuments = consent.AllowDocuments,
            AllowEncounters = consent.AllowEncounters,
            ShareToken = consent.ShareToken,
            TokenExpiryDate = consent.TokenExpiryDate,
            AccessCount = consent.AccessCount,
            MaxAccessCount = consent.MaxAccessCount
        };
    }

    #endregion
}

#region DTOs

public class ConsentDto
{
    public Guid ConsentId { get; set; }
    public string ConsentType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public string? RecipientName { get; set; }
    public string? RecipientOrganization { get; set; }
    public string? RecipientType { get; set; }
    public DateTime GrantedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool AllowDemographics { get; set; }
    public bool AllowAllergies { get; set; }
    public bool AllowConditions { get; set; }
    public bool AllowImmunizations { get; set; }
    public bool AllowMedications { get; set; }
    public bool AllowInsurance { get; set; }
    public bool AllowDocuments { get; set; }
    public bool AllowEncounters { get; set; }
    public string? ShareToken { get; set; }
    public DateTime? TokenExpiryDate { get; set; }
    public int AccessCount { get; set; }
    public int? MaxAccessCount { get; set; }
}

public class CreateConsentRequest
{
    public Guid PatientId { get; set; }
    public string ConsentType { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public bool AllowDemographics { get; set; }
    public bool AllowAllergies { get; set; }
    public bool AllowConditions { get; set; }
    public bool AllowImmunizations { get; set; }
    public bool AllowMedications { get; set; }
    public bool AllowInsurance { get; set; }
    public bool AllowDocuments { get; set; }
    public bool AllowEncounters { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientOrganization { get; set; }
    public string? RecipientType { get; set; }
    public string? RecipientEmail { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Purpose { get; set; }
    public string? LegalBasis { get; set; }
    public bool SignatureObtained { get; set; }
    public bool GenerateShareToken { get; set; }
    public DateTime? TokenExpiryDate { get; set; }
    public int? MaxAccessCount { get; set; }
}

public class RevokeConsentRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class ConsentAuditDto
{
    public Guid AuditId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? ActionDetails { get; set; }
    public DateTime ActionDate { get; set; }
    public string ActorName { get; set; } = string.Empty;
    public string? ActorRole { get; set; }
    public string? IpAddress { get; set; }
}

#endregion

