using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Infrastructure.Data;

namespace PatientService.API.Controllers;

[ApiController]
[Route("api/hospitalizations")]
public class HospitalizationsController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly ILogger<HospitalizationsController> _logger;

    public HospitalizationsController(PatientDbContext context, ILogger<HospitalizationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/hospitalizations/patient/{patientId}
    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<HospitalizationDto>>> GetPatientHospitalizations(Guid patientId)
    {
        try
        {
            var hospitalizations = await _context.PatientHospitalizations
                .Where(h => h.PatientId == patientId)
                .OrderByDescending(h => h.AdmissionDate ?? h.CreatedAt)
                .ToListAsync();

            var dtos = hospitalizations.Select(h => new HospitalizationDto
            {
                HospitalizationId = h.HospitalizationId,
                PatientId = h.PatientId,
                HospitalName = h.HospitalName,
                Reason = h.Reason,
                AdmissionDate = h.AdmissionDate,
                DischargeDate = h.DischargeDate,
                Notes = h.Notes
            }).ToList();

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving hospitalizations for patient {PatientId}", patientId);
            return StatusCode(500, new { message = "An error occurred while retrieving hospitalizations" });
        }
    }

    // POST: api/hospitalizations
    [HttpPost]
    public async Task<ActionResult<HospitalizationDto>> CreateHospitalization([FromBody] CreateHospitalizationRequest request)
    {
        try
        {
            var hospitalization = new PatientHospitalization
            {
                PatientId = request.PatientId,
                HospitalName = request.HospitalName,
                Reason = request.Reason,
                AdmissionDate = request.AdmissionDate,
                DischargeDate = request.DischargeDate,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.PatientHospitalizations.Add(hospitalization);
            await _context.SaveChangesAsync();

            var dto = new HospitalizationDto
            {
                HospitalizationId = hospitalization.HospitalizationId,
                PatientId = hospitalization.PatientId,
                HospitalName = hospitalization.HospitalName,
                Reason = hospitalization.Reason,
                AdmissionDate = hospitalization.AdmissionDate,
                DischargeDate = hospitalization.DischargeDate,
                Notes = hospitalization.Notes
            };

            return CreatedAtAction(nameof(GetPatientHospitalizations), new { patientId = hospitalization.PatientId }, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating hospitalization");
            return StatusCode(500, new { message = "An error occurred while creating the hospitalization" });
        }
    }

    // PUT: api/hospitalizations/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHospitalization(int id, [FromBody] UpdateHospitalizationRequest request)
    {
        try
        {
            var hospitalization = await _context.PatientHospitalizations.FindAsync(id);

            if (hospitalization == null)
            {
                return NotFound(new { message = "Hospitalization not found" });
            }

            hospitalization.HospitalName = request.HospitalName ?? hospitalization.HospitalName;
            hospitalization.Reason = request.Reason ?? hospitalization.Reason;
            hospitalization.AdmissionDate = request.AdmissionDate ?? hospitalization.AdmissionDate;
            hospitalization.DischargeDate = request.DischargeDate ?? hospitalization.DischargeDate;
            hospitalization.Notes = request.Notes ?? hospitalization.Notes;

            await _context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating hospitalization {HospitalizationId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the hospitalization" });
        }
    }

    // DELETE: api/hospitalizations/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHospitalization(int id)
    {
        try
        {
            var hospitalization = await _context.PatientHospitalizations.FindAsync(id);

            if (hospitalization == null)
            {
                return NotFound(new { message = "Hospitalization not found" });
            }

            _context.PatientHospitalizations.Remove(hospitalization);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting hospitalization {HospitalizationId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the hospitalization" });
        }
    }
}

public class HospitalizationDto
{
    public int HospitalizationId { get; set; }
    public Guid PatientId { get; set; }
    public string? HospitalName { get; set; }
    public string? Reason { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public DateTime? DischargeDate { get; set; }
    public string? Notes { get; set; }
}

public class CreateHospitalizationRequest
{
    public Guid PatientId { get; set; }
    public string? HospitalName { get; set; }
    public string? Reason { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public DateTime? DischargeDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateHospitalizationRequest
{
    public string? HospitalName { get; set; }
    public string? Reason { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public DateTime? DischargeDate { get; set; }
    public string? Notes { get; set; }
}

