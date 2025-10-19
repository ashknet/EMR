using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Infrastructure.Data;
using PatientService.Infrastructure.Services;

namespace PatientService.API.Controllers;

/// <summary>
/// Allergies API - manages patient allergies and intolerances (FHIR AllergyIntolerance)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AllergiesController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly IFhirService _fhirService;
    private readonly ILogger<AllergiesController> _logger;

    public AllergiesController(PatientDbContext context, IFhirService fhirService, ILogger<AllergiesController> logger)
    {
        _context = context;
        _fhirService = fhirService;
        _logger = logger;
    }

    /// <summary>
    /// Get all allergies for a patient
    /// </summary>
    [HttpGet("patient/{patientId}")]
    [ProducesResponseType(typeof(List<AllergyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AllergyDto>>> GetAllergies(Guid patientId)
    {
        var allergies = await _context.Allergies
            .Where(a => a.PatientId == patientId && a.IsActive)
            .Select(a => new AllergyDto
            {
                AllergyId = a.AllergyId,
                AllergenName = a.AllergenName,
                AllergenType = a.AllergenType,
                Severity = a.Severity,
                ClinicalStatus = a.ClinicalStatus,
                Reaction = a.Reaction,
                OnsetDate = a.OnsetDate,
                VerificationStatus = a.VerificationStatus
            })
            .ToListAsync();

        return Ok(allergies);
    }

    /// <summary>
    /// Add new allergy
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AllergyDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<AllergyDto>> CreateAllergy([FromBody] CreateAllergyRequest request)
    {
        var allergy = new Allergy
        {
            AllergyId = Guid.NewGuid(),
            PatientId = request.PatientId,
            AllergenName = request.AllergenName,
            AllergenType = request.AllergenType,
            AllergenCode = request.AllergenCode,
            CodeSystem = request.CodeSystem,
            Severity = request.Severity,
            ClinicalStatus = request.ClinicalStatus ?? "Active",
            Reaction = request.Reaction,
            OnsetDate = request.OnsetDate,
            ReportedBy = request.ReportedBy,
            VerificationStatus = request.VerificationStatus,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = User?.Identity?.Name ?? "system",
            IsActive = true
        };

        _context.Allergies.Add(allergy);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Added allergy {AllergyId} for patient {PatientId}", allergy.AllergyId, request.PatientId);

        var dto = new AllergyDto
        {
            AllergyId = allergy.AllergyId,
            AllergenName = allergy.AllergenName,
            AllergenType = allergy.AllergenType,
            Severity = allergy.Severity,
            ClinicalStatus = allergy.ClinicalStatus,
            Reaction = allergy.Reaction,
            OnsetDate = allergy.OnsetDate,
            VerificationStatus = allergy.VerificationStatus
        };

        return CreatedAtAction(nameof(GetAllergies), new { patientId = request.PatientId }, dto);
    }

    /// <summary>
    /// Get allergy as FHIR resource
    /// </summary>
    [HttpGet("{allergyId}/fhir")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> GetAllergyAsFhir(Guid allergyId)
    {
        var allergy = await _context.Allergies.FindAsync(allergyId);

        if (allergy == null)
        {
            return NotFound();
        }

        var fhirResource = _fhirService.MapToFhirAllergyIntolerance(allergy);
        var json = _fhirService.SerializeToJson(fhirResource);

        return Ok(json);
    }

    /// <summary>
    /// Update allergy
    /// </summary>
    [HttpPut("{allergyId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAllergy(Guid allergyId, [FromBody] UpdateAllergyRequest request)
    {
        var allergy = await _context.Allergies.FindAsync(allergyId);

        if (allergy == null)
        {
            return NotFound();
        }

        allergy.AllergenName = request.AllergenName;
        allergy.AllergenType = request.AllergenType;
        allergy.Severity = request.Severity;
        allergy.ClinicalStatus = request.ClinicalStatus;
        allergy.Reaction = request.Reaction;
        allergy.VerificationStatus = request.VerificationStatus;
        allergy.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok();
    }

    /// <summary>
    /// Delete allergy
    /// </summary>
    [HttpDelete("{allergyId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAllergy(Guid allergyId)
    {
        var allergy = await _context.Allergies.FindAsync(allergyId);

        if (allergy == null)
        {
            return NotFound();
        }

        allergy.IsActive = false;
        allergy.ClinicalStatus = "Inactive";
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

#region DTOs

public class AllergyDto
{
    public Guid AllergyId { get; set; }
    public string AllergenName { get; set; } = string.Empty;
    public string AllergenType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string ClinicalStatus { get; set; } = string.Empty;
    public string? Reaction { get; set; }
    public DateTime? OnsetDate { get; set; }
    public string? VerificationStatus { get; set; }
}

public class CreateAllergyRequest
{
    public Guid PatientId { get; set; }
    public string AllergenName { get; set; } = string.Empty;
    public string AllergenType { get; set; } = string.Empty; // Drug, Food, Environment, Biologic
    public string? AllergenCode { get; set; }
    public string? CodeSystem { get; set; }
    public string Severity { get; set; } = string.Empty; // Mild, Moderate, Severe
    public string? ClinicalStatus { get; set; }
    public string? Reaction { get; set; }
    public DateTime? OnsetDate { get; set; }
    public string? ReportedBy { get; set; }
    public string? VerificationStatus { get; set; }
}

public class UpdateAllergyRequest
{
    public string AllergenName { get; set; } = string.Empty;
    public string AllergenType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string ClinicalStatus { get; set; } = string.Empty;
    public string? Reaction { get; set; }
    public string? VerificationStatus { get; set; }
}

#endregion

