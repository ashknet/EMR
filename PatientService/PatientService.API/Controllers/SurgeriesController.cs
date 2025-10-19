using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Infrastructure.Data;

namespace PatientService.API.Controllers;

[ApiController]
[Route("api/surgeries")]
public class SurgeriesController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly ILogger<SurgeriesController> _logger;

    public SurgeriesController(PatientDbContext context, ILogger<SurgeriesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/surgeries/patient/{patientId}
    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<SurgeryDto>>> GetPatientSurgeries(Guid patientId)
    {
        try
        {
            var surgeries = await _context.PatientSurgeries
                .Where(s => s.PatientId == patientId)
                .OrderByDescending(s => s.SurgeryDate ?? s.CreatedAt)
                .ToListAsync();

            var dtos = surgeries.Select(s => new SurgeryDto
            {
                SurgeryId = s.SurgeryId,
                PatientId = s.PatientId,
                SurgeryType = s.SurgeryType,
                SurgeryDate = s.SurgeryDate,
                Notes = s.Notes
            }).ToList();

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving surgeries for patient {PatientId}", patientId);
            return StatusCode(500, new { message = "An error occurred while retrieving surgeries" });
        }
    }

    // POST: api/surgeries
    [HttpPost]
    public async Task<ActionResult<SurgeryDto>> CreateSurgery([FromBody] CreateSurgeryRequest request)
    {
        try
        {
            var surgery = new PatientSurgery
            {
                PatientId = request.PatientId,
                SurgeryType = request.SurgeryType,
                SurgeryDate = request.SurgeryDate,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.PatientSurgeries.Add(surgery);
            await _context.SaveChangesAsync();

            var dto = new SurgeryDto
            {
                SurgeryId = surgery.SurgeryId,
                PatientId = surgery.PatientId,
                SurgeryType = surgery.SurgeryType,
                SurgeryDate = surgery.SurgeryDate,
                Notes = surgery.Notes
            };

            return CreatedAtAction(nameof(GetPatientSurgeries), new { patientId = surgery.PatientId }, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating surgery");
            return StatusCode(500, new { message = "An error occurred while creating the surgery" });
        }
    }

    // PUT: api/surgeries/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSurgery(int id, [FromBody] UpdateSurgeryRequest request)
    {
        try
        {
            var surgery = await _context.PatientSurgeries.FindAsync(id);

            if (surgery == null)
            {
                return NotFound(new { message = "Surgery not found" });
            }

            surgery.SurgeryType = request.SurgeryType ?? surgery.SurgeryType;
            surgery.SurgeryDate = request.SurgeryDate ?? surgery.SurgeryDate;
            surgery.Notes = request.Notes ?? surgery.Notes;

            await _context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating surgery {SurgeryId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the surgery" });
        }
    }

    // DELETE: api/surgeries/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSurgery(int id)
    {
        try
        {
            var surgery = await _context.PatientSurgeries.FindAsync(id);

            if (surgery == null)
            {
                return NotFound(new { message = "Surgery not found" });
            }

            _context.PatientSurgeries.Remove(surgery);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting surgery {SurgeryId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the surgery" });
        }
    }
}

public class SurgeryDto
{
    public int SurgeryId { get; set; }
    public Guid PatientId { get; set; }
    public string SurgeryType { get; set; } = string.Empty;
    public DateTime? SurgeryDate { get; set; }
    public string? Notes { get; set; }
}

public class CreateSurgeryRequest
{
    public Guid PatientId { get; set; }
    public string SurgeryType { get; set; } = string.Empty;
    public DateTime? SurgeryDate { get; set; }
    public string? Notes { get; set; }
}

public class UpdateSurgeryRequest
{
    public string? SurgeryType { get; set; }
    public DateTime? SurgeryDate { get; set; }
    public string? Notes { get; set; }
}

