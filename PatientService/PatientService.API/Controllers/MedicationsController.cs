using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Infrastructure.Data;

namespace PatientService.API.Controllers;

[ApiController]
[Route("api/medications")]
public class MedicationsController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly ILogger<MedicationsController> _logger;

    public MedicationsController(PatientDbContext context, ILogger<MedicationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/medications/patient/{patientId}
    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<MedicationDto>>> GetPatientMedications(Guid patientId)
    {
        try
        {
            var medications = await _context.Medications
                .Where(m => m.PatientId == patientId)
                .OrderByDescending(m => m.StartDate ?? m.CreatedAt)
                .ToListAsync();

            var dtos = medications.Select(m => new MedicationDto
            {
                MedicationId = m.MedicationId,
                PatientId = m.PatientId,
                MedicationName = m.MedicationName,
                Dosage = m.Dosage,
                Frequency = m.Frequency,
                Prescriber = m.Prescriber,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                IsActive = m.IsActive,
                Notes = m.Notes
            }).ToList();

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving medications for patient {PatientId}", patientId);
            return StatusCode(500, new { message = "An error occurred while retrieving medications" });
        }
    }

    // POST: api/medications
    [HttpPost]
    public async Task<ActionResult<MedicationDto>> CreateMedication([FromBody] CreateMedicationRequest request)
    {
        try
        {
            var medication = new Medication
            {
                MedicationId = Guid.NewGuid(),
                PatientId = request.PatientId,
                MedicationName = request.MedicationName,
                Dosage = request.Dosage,
                Frequency = request.Frequency,
                Prescriber = request.Prescriber,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = request.IsActive,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System" // TODO: Get from authenticated user
            };

            _context.Medications.Add(medication);
            await _context.SaveChangesAsync();

            var dto = new MedicationDto
            {
                MedicationId = medication.MedicationId,
                PatientId = medication.PatientId,
                MedicationName = medication.MedicationName,
                Dosage = medication.Dosage,
                Frequency = medication.Frequency,
                Prescriber = medication.Prescriber,
                StartDate = medication.StartDate,
                EndDate = medication.EndDate,
                IsActive = medication.IsActive,
                Notes = medication.Notes
            };

            return CreatedAtAction(nameof(GetPatientMedications), new { patientId = medication.PatientId }, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating medication");
            return StatusCode(500, new { message = "An error occurred while creating the medication" });
        }
    }

    // PUT: api/medications/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMedication(Guid id, [FromBody] UpdateMedicationRequest request)
    {
        try
        {
            var medication = await _context.Medications.FindAsync(id);

            if (medication == null)
            {
                return NotFound(new { message = "Medication not found" });
            }

            medication.MedicationName = request.MedicationName ?? medication.MedicationName;
            medication.Dosage = request.Dosage ?? medication.Dosage;
            medication.Frequency = request.Frequency ?? medication.Frequency;
            medication.Prescriber = request.Prescriber ?? medication.Prescriber;
            medication.StartDate = request.StartDate ?? medication.StartDate;
            medication.EndDate = request.EndDate ?? medication.EndDate;
            medication.IsActive = request.IsActive ?? medication.IsActive;
            medication.Notes = request.Notes ?? medication.Notes;
            medication.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating medication {MedicationId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the medication" });
        }
    }

    // DELETE: api/medications/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMedication(Guid id)
    {
        try
        {
            var medication = await _context.Medications.FindAsync(id);

            if (medication == null)
            {
                return NotFound(new { message = "Medication not found" });
            }

            _context.Medications.Remove(medication);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting medication {MedicationId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the medication" });
        }
    }
}

public class MedicationDto
{
    public Guid MedicationId { get; set; }
    public Guid PatientId { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public string? Prescriber { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}

public class CreateMedicationRequest
{
    public Guid PatientId { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public string? Prescriber { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}

public class UpdateMedicationRequest
{
    public string? MedicationName { get; set; }
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public string? Prescriber { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsActive { get; set; }
    public string? Notes { get; set; }
}

