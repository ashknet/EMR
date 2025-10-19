using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Infrastructure.Data;
using PatientService.Infrastructure.Services;

namespace PatientService.API.Controllers;

/// <summary>
/// Record Transfer API - manages medical record transfers with FHIR bundles
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TransfersController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly IFhirService _fhirService;
    private readonly ILogger<TransfersController> _logger;

    public TransfersController(
        PatientDbContext context,
        IFhirService fhirService,
        ILogger<TransfersController> logger)
    {
        _context = context;
        _fhirService = fhirService;
        _logger = logger;
    }

    /// <summary>
    /// Get all transfers for a patient
    /// </summary>
    [HttpGet("patient/{patientId}")]
    [ProducesResponseType(typeof(List<TransferDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TransferDto>>> GetTransfers(Guid patientId)
    {
        var transfers = await _context.RecordTransfers
            .Where(t => t.PatientId == patientId)
            .OrderByDescending(t => t.RequestedDate)
            .Select(t => new TransferDto
            {
                TransferId = t.TransferId,
                TransferType = t.TransferType,
                TransferMethod = t.TransferMethod,
                RecipientOrganization = t.RecipientOrganization,
                RecipientName = t.RecipientName,
                SenderOrganization = t.SenderOrganization,
                Purpose = t.Purpose,
                RequestedDate = t.RequestedDate,
                Status = t.Status,
                StatusReason = t.StatusReason,
                CompletedDate = t.CompletedDate,
                TrackingNumber = t.TrackingNumber,
                ResourceCount = t.ResourceCount
            })
            .ToListAsync();

        return Ok(transfers);
    }

    /// <summary>
    /// Initiate new outbound record transfer
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TransferDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TransferDto>> InitiateTransfer([FromBody] CreateTransferRequest request)
    {
        // Validate consent if required
        if (request.ConsentId.HasValue)
        {
            var consent = await _context.Consents.FindAsync(request.ConsentId.Value);
            if (consent == null || consent.Status != "Active")
            {
                return BadRequest(new { message = "Valid consent required", traceId = HttpContext.TraceIdentifier });
            }
        }

        var transfer = new RecordTransfer
        {
            TransferId = Guid.NewGuid(),
            PatientId = request.PatientId,
            ConsentId = request.ConsentId,
            TransferType = request.TransferType ?? "Outbound",
            TransferMethod = request.TransferMethod ?? "FHIR",
            RecipientOrganization = request.RecipientOrganization,
            RecipientName = request.RecipientName,
            RecipientEmail = request.RecipientEmail,
            RecipientFhirEndpoint = request.RecipientFhirEndpoint,
            RecipientNpi = request.RecipientNpi,
            Purpose = request.Purpose,
            RequestedBy = User?.Identity?.Name ?? "system",
            RequestedDate = DateTime.UtcNow,
            Status = "Requested",
            IncludeDemographics = request.IncludeDemographics,
            IncludeAllergies = request.IncludeAllergies,
            IncludeConditions = request.IncludeConditions,
            IncludeImmunizations = request.IncludeImmunizations,
            IncludeMedications = request.IncludeMedications,
            IncludeEncounters = request.IncludeEncounters,
            IncludeDocuments = request.IncludeDocuments,
            IncludeInsurance = request.IncludeInsurance,
            DataStartDate = request.DataStartDate,
            DataEndDate = request.DataEndDate,
            TrackingNumber = GenerateTrackingNumber(),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = User?.Identity?.Name ?? "system"
        };

        _context.RecordTransfers.Add(transfer);

        // Log transfer initiation
        var audit = new TransferAudit
        {
            AuditId = Guid.NewGuid(),
            TransferId = transfer.TransferId,
            Action = "Requested",
            ActionDetails = $"Transfer requested to {request.RecipientOrganization}",
            ActionDate = DateTime.UtcNow,
            ActorId = User?.Identity?.Name ?? "system",
            ActorName = User?.Identity?.Name ?? "System User",
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString()
        };

        _context.TransferAudits.Add(audit);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Initiated transfer {TransferId} for patient {PatientId} to {Recipient}",
            transfer.TransferId, request.PatientId, request.RecipientOrganization);

        var dto = MapToDto(transfer);

        return CreatedAtAction(nameof(GetTransfer), new { transferId = transfer.TransferId }, dto);
    }

    /// <summary>
    /// Get transfer by ID
    /// </summary>
    [HttpGet("{transferId}")]
    [ProducesResponseType(typeof(TransferDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<TransferDto>> GetTransfer(Guid transferId)
    {
        var transfer = await _context.RecordTransfers.FindAsync(transferId);

        if (transfer == null)
        {
            return NotFound();
        }

        return Ok(MapToDto(transfer));
    }

    /// <summary>
    /// Generate FHIR bundle for transfer
    /// </summary>
    [HttpPost("{transferId}/generate-bundle")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> GenerateFhirBundle(Guid transferId)
    {
        var transfer = await _context.RecordTransfers
            .Include(t => t.Patient)
            .FirstOrDefaultAsync(t => t.TransferId == transferId);

        if (transfer == null)
        {
            return NotFound();
        }

        // Fetch requested data
        var allergies = transfer.IncludeAllergies
            ? await _context.Allergies.Where(a => a.PatientId == transfer.PatientId && a.IsActive).ToListAsync()
            : new List<Allergy>();

        var conditions = transfer.IncludeConditions
            ? await _context.Conditions.Where(c => c.PatientId == transfer.PatientId && c.IsActive).ToListAsync()
            : new List<Domain.Entities.Condition>();

        var immunizations = transfer.IncludeImmunizations
            ? await _context.Immunizations.Where(i => i.PatientId == transfer.PatientId).ToListAsync()
            : new List<Domain.Entities.Immunization>();

        // Create FHIR bundle
        var bundle = _fhirService.CreateTransferBundle(
            transfer.Patient,
            allergies,
            conditions,
            immunizations
        );

        transfer.FhirBundleId = bundle.Id;
        transfer.ResourceCount = bundle.Entry.Count;
        transfer.Status = "Processing";
        transfer.UpdatedAt = DateTime.UtcNow;

        // Log bundle generation
        var audit = new TransferAudit
        {
            AuditId = Guid.NewGuid(),
            TransferId = transfer.TransferId,
            Action = "Bundle Generated",
            ActionDetails = $"FHIR bundle created with {bundle.Entry.Count} resources",
            ActionDate = DateTime.UtcNow,
            ActorId = User?.Identity?.Name ?? "system"
        };

        _context.TransferAudits.Add(audit);
        await _context.SaveChangesAsync();

        var json = _fhirService.SerializeToJson(bundle);

        return Ok(json);
    }

    /// <summary>
    /// Update transfer status
    /// </summary>
    [HttpPatch("{transferId}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateTransferStatus(Guid transferId, [FromBody] UpdateTransferStatusRequest request)
    {
        var transfer = await _context.RecordTransfers.FindAsync(transferId);

        if (transfer == null)
        {
            return NotFound();
        }

        var oldStatus = transfer.Status;
        transfer.Status = request.Status;
        transfer.StatusReason = request.StatusReason;
        transfer.UpdatedAt = DateTime.UtcNow;

        if (request.Status == "Completed")
        {
            transfer.CompletedDate = DateTime.UtcNow;
        }
        else if (request.Status == "Cancelled")
        {
            transfer.CancelledDate = DateTime.UtcNow;
            transfer.CancellationReason = request.StatusReason;
        }

        // Log status change
        var audit = new TransferAudit
        {
            AuditId = Guid.NewGuid(),
            TransferId = transfer.TransferId,
            Action = "Status Changed",
            ActionDetails = $"Status changed from {oldStatus} to {request.Status}",
            ActionDate = DateTime.UtcNow,
            ActorId = User?.Identity?.Name ?? "system"
        };

        _context.TransferAudits.Add(audit);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated transfer {TransferId} status to {Status}", transferId, request.Status);

        return Ok();
    }

    /// <summary>
    /// Get transfer audit trail
    /// </summary>
    [HttpGet("{transferId}/audit")]
    [ProducesResponseType(typeof(List<TransferAuditDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TransferAuditDto>>> GetTransferAudit(Guid transferId)
    {
        var audits = await _context.TransferAudits
            .Where(a => a.TransferId == transferId)
            .OrderByDescending(a => a.ActionDate)
            .Select(a => new TransferAuditDto
            {
                AuditId = a.AuditId,
                Action = a.Action,
                ActionDetails = a.ActionDetails,
                ActionDate = a.ActionDate,
                ActorName = a.ActorName,
                ErrorMessage = a.ErrorMessage
            })
            .ToListAsync();

        return Ok(audits);
    }

    #region Helper Methods

    private static string GenerateTrackingNumber()
    {
        return $"TRK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }

    private static TransferDto MapToDto(RecordTransfer transfer)
    {
        return new TransferDto
        {
            TransferId = transfer.TransferId,
            TransferType = transfer.TransferType,
            TransferMethod = transfer.TransferMethod,
            RecipientOrganization = transfer.RecipientOrganization,
            RecipientName = transfer.RecipientName,
            SenderOrganization = transfer.SenderOrganization,
            Purpose = transfer.Purpose,
            RequestedDate = transfer.RequestedDate,
            Status = transfer.Status,
            StatusReason = transfer.StatusReason,
            CompletedDate = transfer.CompletedDate,
            TrackingNumber = transfer.TrackingNumber,
            ResourceCount = transfer.ResourceCount
        };
    }

    #endregion
}

#region DTOs

public class TransferDto
{
    public Guid TransferId { get; set; }
    public string TransferType { get; set; } = string.Empty;
    public string TransferMethod { get; set; } = string.Empty;
    public string? RecipientOrganization { get; set; }
    public string? RecipientName { get; set; }
    public string? SenderOrganization { get; set; }
    public string? Purpose { get; set; }
    public DateTime RequestedDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? StatusReason { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? TrackingNumber { get; set; }
    public int? ResourceCount { get; set; }
}

public class CreateTransferRequest
{
    public Guid PatientId { get; set; }
    public Guid? ConsentId { get; set; }
    public string? TransferType { get; set; }
    public string? TransferMethod { get; set; }
    public string? RecipientOrganization { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientEmail { get; set; }
    public string? RecipientFhirEndpoint { get; set; }
    public string? RecipientNpi { get; set; }
    public string? Purpose { get; set; }
    public bool IncludeDemographics { get; set; } = true;
    public bool IncludeAllergies { get; set; } = true;
    public bool IncludeConditions { get; set; } = true;
    public bool IncludeImmunizations { get; set; } = true;
    public bool IncludeMedications { get; set; } = true;
    public bool IncludeEncounters { get; set; } = true;
    public bool IncludeDocuments { get; set; } = true;
    public bool IncludeInsurance { get; set; } = false;
    public DateTime? DataStartDate { get; set; }
    public DateTime? DataEndDate { get; set; }
}

public class UpdateTransferStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public string? StatusReason { get; set; }
}

public class TransferAuditDto
{
    public Guid AuditId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? ActionDetails { get; set; }
    public DateTime ActionDate { get; set; }
    public string? ActorName { get; set; }
    public string? ErrorMessage { get; set; }
}

#endregion

