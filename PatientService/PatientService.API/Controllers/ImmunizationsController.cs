using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Infrastructure.Data;

namespace PatientService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImmunizationsController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly ILogger<ImmunizationsController> _logger;

    public ImmunizationsController(PatientDbContext context, ILogger<ImmunizationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/immunizations/patient/{patientId}
    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<ImmunizationDto>>> GetPatientImmunizations(Guid patientId)
    {
        try
        {
            var immunizations = await _context.Immunizations
                .Where(i => i.PatientId == patientId)
                .OrderByDescending(i => i.AdministeredDate)
                .Select(i => new ImmunizationDto
                {
                    ImmunizationId = i.ImmunizationId,
                    PatientId = i.PatientId,
                    VaccineName = i.VaccineName,
                    AdministeredDate = i.AdministeredDate,
                    DoseNumber = i.DoseNumber,
                    Provider = null, // Database has ProviderName, not Provider
                    LotNumber = i.LotNumber,
                    ExpirationDate = i.ExpirationDate,
                    Site = i.Site,
                    Route = i.Route,
                    Notes = null // This column doesn't exist in database
                })
                .ToListAsync();

            return Ok(immunizations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving immunizations for patient {PatientId}", patientId);
            return StatusCode(500, new { message = "An error occurred while retrieving immunizations" });
        }
    }

    // GET: api/immunizations/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ImmunizationDto>> GetImmunization(Guid id)
    {
        try
        {
            var immunization = await _context.Immunizations
                .Where(i => i.ImmunizationId == id)
                .Select(i => new ImmunizationDto
                {
                    ImmunizationId = i.ImmunizationId,
                    PatientId = i.PatientId,
                    VaccineName = i.VaccineName,
                    AdministeredDate = i.AdministeredDate,
                    DoseNumber = i.DoseNumber,
                    Provider = null, // Database has ProviderName, not Provider
                    LotNumber = i.LotNumber,
                    ExpirationDate = i.ExpirationDate,
                    Site = i.Site,
                    Route = i.Route,
                    Notes = null // This column doesn't exist in database
                })
                .FirstOrDefaultAsync();

            if (immunization == null)
            {
                return NotFound();
            }

            return Ok(immunization);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving immunization {ImmunizationId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the immunization" });
        }
    }

    // POST: api/immunizations
    [HttpPost]
    public async Task<ActionResult<ImmunizationDto>> CreateImmunization(ImmunizationDto dto)
    {
        try
        {
            var immunization = new Immunization
            {
                ImmunizationId = Guid.NewGuid(),
                PatientId = dto.PatientId,
                VaccineName = dto.VaccineName,
                AdministeredDate = dto.AdministeredDate,
                DoseNumber = dto.DoseNumber,
                LotNumber = dto.LotNumber,
                ExpirationDate = dto.ExpirationDate,
                Site = dto.Site,
                Route = dto.Route,
                Status = dto.Status ?? "Completed",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System" // TODO: Get from auth context
            };

            _context.Immunizations.Add(immunization);
            await _context.SaveChangesAsync();

            dto.ImmunizationId = immunization.ImmunizationId;

            return CreatedAtAction(nameof(GetImmunization), new { id = immunization.ImmunizationId }, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating immunization");
            return StatusCode(500, new { message = "An error occurred while creating the immunization" });
        }
    }

    // PUT: api/immunizations/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateImmunization(Guid id, ImmunizationDto dto)
    {
        try
        {
            var immunization = await _context.Immunizations
                .FirstOrDefaultAsync(i => i.ImmunizationId == id);

            if (immunization == null)
            {
                return NotFound();
            }

            immunization.VaccineName = dto.VaccineName;
            immunization.AdministeredDate = dto.AdministeredDate;
            immunization.DoseNumber = dto.DoseNumber;
            immunization.LotNumber = dto.LotNumber;
            immunization.ExpirationDate = dto.ExpirationDate;
            immunization.Site = dto.Site;
            immunization.Route = dto.Route;
            immunization.UpdatedAt = DateTime.UtcNow;
            immunization.UpdatedBy = "System"; // TODO: Get from auth context

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating immunization {ImmunizationId}", id);
            return StatusCode(500, new { message = "An error occurred while updating the immunization" });
        }
    }

    // DELETE: api/immunizations/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteImmunization(Guid id)
    {
        try
        {
            var immunization = await _context.Immunizations
                .FirstOrDefaultAsync(i => i.ImmunizationId == id);

            if (immunization == null)
            {
                return NotFound();
            }

            // Soft delete by setting status
            immunization.Status = "Cancelled";
            immunization.UpdatedAt = DateTime.UtcNow;
            immunization.UpdatedBy = "System"; // TODO: Get from auth context

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting immunization {ImmunizationId}", id);
            return StatusCode(500, new { message = "An error occurred while deleting the immunization" });
        }
    }
}

public class ImmunizationDto
{
    public Guid ImmunizationId { get; set; }
    public Guid PatientId { get; set; }
    public string VaccineName { get; set; } = string.Empty;
    public DateTime AdministeredDate { get; set; }
    public int? DoseNumber { get; set; }
    public string? Provider { get; set; }
    public string? LotNumber { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Site { get; set; }
    public string? Route { get; set; }
    public string? Notes { get; set; }
    public string? Status { get; set; }
}

