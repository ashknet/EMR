using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Infrastructure.Data;

namespace PatientService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChronicConditionsController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly ILogger<ChronicConditionsController> _logger;

    public ChronicConditionsController(PatientDbContext context, ILogger<ChronicConditionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/chronicconditions/patient/{patientId}
    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<ChronicConditionDto>>> GetPatientConditions(Guid patientId)
    {
        try
        {
            var conditions = await _context.ChronicConditions
                .Where(c => c.PatientId == patientId && c.IsActive)
                .OrderByDescending(c => c.DiagnosedDate)
                .Select(c => new ChronicConditionDto
                {
                    ConditionId = c.ConditionId,
                    PatientId = c.PatientId,
                    ConditionName = c.ConditionName,
                    DiagnosedDate = c.DiagnosedDate,
                    Status = c.Status,
                    Notes = c.Notes
                })
                .ToListAsync();

            return Ok(conditions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving chronic conditions for patient {PatientId}", patientId);
            return StatusCode(500, new { message = "An error occurred while retrieving chronic conditions" });
        }
    }

    // GET: api/chronicconditions/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ChronicConditionDto>> GetCondition(Guid id)
    {
        try
        {
            var condition = await _context.ChronicConditions
                .Where(c => c.ConditionId == id && c.IsActive)
                .Select(c => new ChronicConditionDto
                {
                    ConditionId = c.ConditionId,
                    PatientId = c.PatientId,
                    ConditionName = c.ConditionName,
                    DiagnosedDate = c.DiagnosedDate,
                    Status = c.Status,
                    Notes = c.Notes
                })
                .FirstOrDefaultAsync();

            if (condition == null)
            {
                return NotFound();
            }

            return Ok(condition);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving chronic condition {ConditionId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the chronic condition" });
        }
    }

    // POST: api/chronicconditions
    [HttpPost]
    public async Task<ActionResult<ChronicConditionDto>> CreateCondition(ChronicConditionDto dto)
    {
        try
        {
            var condition = new ChronicCondition
            {
                ConditionId = Guid.NewGuid(),
                PatientId = dto.PatientId,
                ConditionName = dto.ConditionName,
                DiagnosedDate = dto.DiagnosedDate,
                Status = dto.Status ?? "Active",
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System", // TODO: Get from auth context
                IsActive = true
            };

            _context.ChronicConditions.Add(condition);
            await _context.SaveChangesAsync();

            dto.ConditionId = condition.ConditionId;

            return CreatedAtAction(nameof(GetCondition), new { id = condition.ConditionId }, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating chronic condition");
            return StatusCode(500, new { message = "An error occurred while creating the chronic condition" });
        }
    }

    // PUT: api/chronicconditions/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCondition(Guid id, ChronicConditionDto dto)
    {
        try
        {
            var condition = await _context.ChronicConditions
                .FirstOrDefaultAsync(c => c.ConditionId == id && c.IsActive);

            if (condition == null)
            {
                return NotFound();
            }

            condition.ConditionName = dto.ConditionName;
            condition.DiagnosedDate = dto.DiagnosedDate;
            condition.Status = dto.Status ?? condition.Status;
            condition.Notes = dto.Notes;
            condition.UpdatedAt = DateTime.UtcNow;
            condition.UpdatedBy = "System"; // TODO: Get from auth context

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating chronic condition {ConditionId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the chronic condition" });
        }
    }

    // DELETE: api/chronicconditions/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCondition(Guid id)
    {
        try
        {
            var condition = await _context.ChronicConditions
                .FirstOrDefaultAsync(c => c.ConditionId == id && c.IsActive);

            if (condition == null)
            {
                return NotFound();
            }

            condition.IsActive = false;
            condition.UpdatedAt = DateTime.UtcNow;
            condition.UpdatedBy = "System"; // TODO: Get from auth context

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting chronic condition {ConditionId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the chronic condition" });
        }
    }
}

public class ChronicConditionDto
{
    public Guid ConditionId { get; set; }
    public Guid PatientId { get; set; }
    public string ConditionName { get; set; } = string.Empty;
    public DateTime? DiagnosedDate { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
}

