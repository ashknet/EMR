using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Infrastructure.Data;

namespace PatientService.API.Controllers;

[ApiController]
[Route("api/familyhistory")]
public class FamilyHistoryController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly ILogger<FamilyHistoryController> _logger;

    public FamilyHistoryController(PatientDbContext context, ILogger<FamilyHistoryController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/familyhistory/patient/{patientId}
    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<FamilyHistoryDto>>> GetPatientFamilyHistory(Guid patientId)
    {
        try
        {
            var history = await _context.FamilyMedicalHistories
                .Where(f => f.PatientId == patientId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            var dtos = history.Select(f => new FamilyHistoryDto
            {
                FamilyHistoryId = f.FamilyHistoryId,
                PatientId = f.PatientId,
                Relative = f.Relative,
                Condition = f.Condition,
                Notes = f.Notes
            }).ToList();

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving family history for patient {PatientId}", patientId);
            return StatusCode(500, new { message = "An error occurred while retrieving family history" });
        }
    }

    // POST: api/familyhistory
    [HttpPost]
    public async Task<ActionResult<FamilyHistoryDto>> CreateFamilyHistory([FromBody] CreateFamilyHistoryRequest request)
    {
        try
        {
            var history = new FamilyMedicalHistory
            {
                PatientId = request.PatientId,
                Relative = request.Relative,
                Condition = request.Condition,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.FamilyMedicalHistories.Add(history);
            await _context.SaveChangesAsync();

            var dto = new FamilyHistoryDto
            {
                FamilyHistoryId = history.FamilyHistoryId,
                PatientId = history.PatientId,
                Relative = history.Relative,
                Condition = history.Condition,
                Notes = history.Notes
            };

            return CreatedAtAction(nameof(GetPatientFamilyHistory), new { patientId = history.PatientId }, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating family history");
            return StatusCode(500, new { message = "An error occurred while creating the family history" });
        }
    }

    // PUT: api/familyhistory/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFamilyHistory(int id, [FromBody] UpdateFamilyHistoryRequest request)
    {
        try
        {
            var history = await _context.FamilyMedicalHistories.FindAsync(id);

            if (history == null)
            {
                return NotFound(new { message = "Family history not found" });
            }

            history.Relative = request.Relative ?? history.Relative;
            history.Condition = request.Condition ?? history.Condition;
            history.Notes = request.Notes ?? history.Notes;

            await _context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating family history {FamilyHistoryId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the family history" });
        }
    }

    // DELETE: api/familyhistory/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFamilyHistory(int id)
    {
        try
        {
            var history = await _context.FamilyMedicalHistories.FindAsync(id);

            if (history == null)
            {
                return NotFound(new { message = "Family history not found" });
            }

            _context.FamilyMedicalHistories.Remove(history);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting family history {FamilyHistoryId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the family history" });
        }
    }
}

public class FamilyHistoryDto
{
    public int FamilyHistoryId { get; set; }
    public Guid PatientId { get; set; }
    public string Relative { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class CreateFamilyHistoryRequest
{
    public Guid PatientId { get; set; }
    public string Relative { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class UpdateFamilyHistoryRequest
{
    public string? Relative { get; set; }
    public string? Condition { get; set; }
    public string? Notes { get; set; }
}

