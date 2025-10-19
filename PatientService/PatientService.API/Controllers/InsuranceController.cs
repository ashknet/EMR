using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Infrastructure.Data;
using PatientService.Infrastructure.Services;

namespace PatientService.API.Controllers;

/// <summary>
/// Insurance API - manages patient insurance coverage (FHIR Coverage)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class InsuranceController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<InsuranceController> _logger;

    public InsuranceController(
        PatientDbContext context,
        IEncryptionService encryptionService,
        ILogger<InsuranceController> logger)
    {
        _context = context;
        _encryptionService = encryptionService;
        _logger = logger;
    }

    /// <summary>
    /// Get all insurance policies for a patient
    /// </summary>
    [HttpGet("patient/{patientId}")]
    [ProducesResponseType(typeof(List<InsuranceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<InsuranceDto>>> GetInsurances(Guid patientId)
    {
        var insurances = await _context.Insurances
            .Where(i => i.PatientId == patientId && i.IsActive)
            .OrderBy(i => i.Priority)
            .Select(i => new InsuranceDto
            {
                InsuranceId = i.InsuranceId,
                PayerName = i.PayerName,
                PlanName = i.PlanName,
                PlanType = i.PlanType,
                MemberId = i.MemberId,
                GroupNumber = i.GroupNumber,
                GroupName = i.GroupName,
                EffectiveDate = i.EffectiveDate,
                ExpirationDate = i.ExpirationDate,
                IsPrimary = i.IsPrimary,
                Priority = i.Priority,
                SubscriberRelationship = i.SubscriberRelationship,
                SubscriberName = i.SubscriberName,
                PayerPhone = i.PayerPhone,
                PayerWebsite = i.PayerWebsite,
                Deductible = i.Deductible,
                DeductibleMet = i.DeductibleMet,
                OutOfPocketMax = i.OutOfPocketMax,
                OutOfPocketMet = i.OutOfPocketMet,
                Copay = i.Copay,
                CoinsurancePercentage = i.CoinsurancePercentage,
                Status = i.Status,
                IsVerified = i.IsVerified,
                LastVerifiedDate = i.LastVerifiedDate
            })
            .ToListAsync();

        return Ok(insurances);
    }

    /// <summary>
    /// Get insurance by ID
    /// </summary>
    [HttpGet("{insuranceId}")]
    [ProducesResponseType(typeof(InsuranceDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<InsuranceDto>> GetInsurance(Guid insuranceId)
    {
        var insurance = await _context.Insurances
            .Where(i => i.InsuranceId == insuranceId)
            .Select(i => new InsuranceDto
            {
                InsuranceId = i.InsuranceId,
                PayerName = i.PayerName,
                PlanName = i.PlanName,
                PlanType = i.PlanType,
                MemberId = i.MemberId,
                GroupNumber = i.GroupNumber,
                GroupName = i.GroupName,
                EffectiveDate = i.EffectiveDate,
                ExpirationDate = i.ExpirationDate,
                IsPrimary = i.IsPrimary,
                Priority = i.Priority,
                SubscriberRelationship = i.SubscriberRelationship,
                SubscriberName = i.SubscriberName,
                PayerPhone = i.PayerPhone,
                PayerWebsite = i.PayerWebsite,
                Deductible = i.Deductible,
                DeductibleMet = i.DeductibleMet,
                OutOfPocketMax = i.OutOfPocketMax,
                OutOfPocketMet = i.OutOfPocketMet,
                Copay = i.Copay,
                CoinsurancePercentage = i.CoinsurancePercentage,
                Status = i.Status,
                IsVerified = i.IsVerified,
                LastVerifiedDate = i.LastVerifiedDate
            })
            .FirstOrDefaultAsync();

        if (insurance == null)
        {
            return NotFound();
        }

        return Ok(insurance);
    }

    /// <summary>
    /// Add new insurance policy
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(InsuranceDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<InsuranceDto>> CreateInsurance([FromBody] CreateInsuranceRequest request)
    {
        var insurance = new Insurance
        {
            InsuranceId = Guid.NewGuid(),
            PatientId = request.PatientId,
            PayerName = request.PayerName,
            PlanName = request.PlanName,
            PlanType = request.PlanType,
            MemberId = request.MemberId,
            GroupNumber = request.GroupNumber,
            GroupName = request.GroupName,
            EffectiveDate = request.EffectiveDate,
            ExpirationDate = request.ExpirationDate,
            IsPrimary = request.IsPrimary,
            Priority = request.Priority,
            SubscriberRelationship = request.SubscriberRelationship ?? "Self",
            SubscriberName = request.SubscriberName,
            SubscriberDateOfBirth = request.SubscriberDateOfBirth,
            PayerPhone = request.PayerPhone,
            PayerWebsite = request.PayerWebsite,
            CustomerServicePhone = request.CustomerServicePhone,
            Deductible = request.Deductible,
            DeductibleMet = request.DeductibleMet ?? 0,
            OutOfPocketMax = request.OutOfPocketMax,
            OutOfPocketMet = request.OutOfPocketMet ?? 0,
            Copay = request.Copay,
            CoinsurancePercentage = request.CoinsurancePercentage,
            Status = "Active",
            IsVerified = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = User?.Identity?.Name ?? "system",
            IsActive = true
        };

        // Encrypt subscriber SSN if provided
        if (!string.IsNullOrEmpty(request.SubscriberSsn))
        {
            insurance.SubscriberSsnEncrypted = await _encryptionService.EncryptAsync(request.SubscriberSsn);
        }

        _context.Insurances.Add(insurance);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Added insurance {InsuranceId} for patient {PatientId}", 
            insurance.InsuranceId, request.PatientId);

        var dto = new InsuranceDto
        {
            InsuranceId = insurance.InsuranceId,
            PayerName = insurance.PayerName,
            PlanName = insurance.PlanName,
            PlanType = insurance.PlanType,
            MemberId = insurance.MemberId,
            GroupNumber = insurance.GroupNumber,
            IsPrimary = insurance.IsPrimary,
            Priority = insurance.Priority,
            Status = insurance.Status
        };

        return CreatedAtAction(nameof(GetInsurance), new { insuranceId = insurance.InsuranceId }, dto);
    }

    /// <summary>
    /// Update insurance policy
    /// </summary>
    [HttpPut("{insuranceId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateInsurance(Guid insuranceId, [FromBody] UpdateInsuranceRequest request)
    {
        var insurance = await _context.Insurances.FindAsync(insuranceId);

        if (insurance == null)
        {
            return NotFound();
        }

        insurance.PayerName = request.PayerName;
        insurance.PlanName = request.PlanName;
        insurance.PlanType = request.PlanType;
        insurance.MemberId = request.MemberId;
        insurance.GroupNumber = request.GroupNumber;
        insurance.ExpirationDate = request.ExpirationDate;
        insurance.PayerPhone = request.PayerPhone;
        insurance.DeductibleMet = request.DeductibleMet;
        insurance.OutOfPocketMet = request.OutOfPocketMet;
        insurance.IsVerified = request.IsVerified;
        insurance.LastVerifiedDate = request.IsVerified ? DateTime.UtcNow : insurance.LastVerifiedDate;
        insurance.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok();
    }

    /// <summary>
    /// Delete insurance policy
    /// </summary>
    [HttpDelete("{insuranceId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteInsurance(Guid insuranceId)
    {
        var insurance = await _context.Insurances.FindAsync(insuranceId);

        if (insurance == null)
        {
            return NotFound();
        }

        insurance.IsActive = false;
        insurance.Status = "Cancelled";
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

#region DTOs

public class InsuranceDto
{
    public Guid InsuranceId { get; set; }
    public string PayerName { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;
    public string? GroupNumber { get; set; }
    public string? GroupName { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsPrimary { get; set; }
    public int Priority { get; set; }
    public string SubscriberRelationship { get; set; } = string.Empty;
    public string? SubscriberName { get; set; }
    public string? PayerPhone { get; set; }
    public string? PayerWebsite { get; set; }
    public decimal? Deductible { get; set; }
    public decimal? DeductibleMet { get; set; }
    public decimal? OutOfPocketMax { get; set; }
    public decimal? OutOfPocketMet { get; set; }
    public decimal? Copay { get; set; }
    public decimal? CoinsurancePercentage { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public DateTime? LastVerifiedDate { get; set; }
}

public class CreateInsuranceRequest
{
    public Guid PatientId { get; set; }
    public string PayerName { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;
    public string? GroupNumber { get; set; }
    public string? GroupName { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsPrimary { get; set; }
    public int Priority { get; set; }
    public string? SubscriberRelationship { get; set; }
    public string? SubscriberName { get; set; }
    public DateTime? SubscriberDateOfBirth { get; set; }
    public string? SubscriberSsn { get; set; }
    public string? PayerPhone { get; set; }
    public string? PayerWebsite { get; set; }
    public string? CustomerServicePhone { get; set; }
    public decimal? Deductible { get; set; }
    public decimal? DeductibleMet { get; set; }
    public decimal? OutOfPocketMax { get; set; }
    public decimal? OutOfPocketMet { get; set; }
    public decimal? Copay { get; set; }
    public decimal? CoinsurancePercentage { get; set; }
}

public class UpdateInsuranceRequest
{
    public string PayerName { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty;
    public string MemberId { get; set; } = string.Empty;
    public string? GroupNumber { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? PayerPhone { get; set; }
    public decimal? DeductibleMet { get; set; }
    public decimal? OutOfPocketMet { get; set; }
    public bool IsVerified { get; set; }
}

#endregion

