using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Infrastructure.Data;

namespace PatientService.API.Controllers;

/// <summary>
/// Providers API - manages patient healthcare providers
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProvidersController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly ILogger<ProvidersController> _logger;

    public ProvidersController(
        PatientDbContext context,
        ILogger<ProvidersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all providers for a patient
    /// </summary>
    [HttpGet("patient/{patientId}")]
    [ProducesResponseType(typeof(List<ProviderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProviderDto>>> GetProviders(Guid patientId)
    {
        var providers = await _context.PatientProviders
            .Where(p => p.PatientId == patientId && p.IsActive)
            .OrderBy(p => p.IsPrimary ? 0 : 1)
            .ThenBy(p => p.ProviderTypeName)
            .Select(p => new ProviderDto
            {
                PatientProviderId = p.PatientProviderId,
                ProviderTypeId = p.ProviderTypeId,
                ProviderTypeName = p.ProviderTypeName,
                ProviderName = p.ProviderName,
                Specialty = p.Specialty,
                NPI = p.NPI,
                Phone = p.Phone,
                Fax = p.Fax,
                Email = p.Email,
                AddressLine1 = p.AddressLine1,
                AddressLine2 = p.AddressLine2,
                City = p.City,
                State = p.State,
                ZipCode = p.ZipCode,
                Country = p.Country,
                PracticeName = p.PracticeName,
                Website = p.Website,
                Notes = p.Notes,
                IsPrimary = p.IsPrimary,
                IsAcceptingPatients = p.IsAcceptingPatients,
                Status = p.Status,
                FirstVisitDate = p.FirstVisitDate,
                LastVisitDate = p.LastVisitDate
            })
            .ToListAsync();

        return Ok(providers);
    }

    /// <summary>
    /// Get provider by ID
    /// </summary>
    [HttpGet("{providerId}")]
    [ProducesResponseType(typeof(ProviderDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProviderDto>> GetProvider(Guid providerId)
    {
        var provider = await _context.PatientProviders
            .Where(p => p.PatientProviderId == providerId)
            .Select(p => new ProviderDto
            {
                PatientProviderId = p.PatientProviderId,
                ProviderTypeId = p.ProviderTypeId,
                ProviderTypeName = p.ProviderTypeName,
                ProviderName = p.ProviderName,
                Specialty = p.Specialty,
                NPI = p.NPI,
                Phone = p.Phone,
                Fax = p.Fax,
                Email = p.Email,
                AddressLine1 = p.AddressLine1,
                AddressLine2 = p.AddressLine2,
                City = p.City,
                State = p.State,
                ZipCode = p.ZipCode,
                Country = p.Country,
                PracticeName = p.PracticeName,
                Website = p.Website,
                Notes = p.Notes,
                IsPrimary = p.IsPrimary,
                IsAcceptingPatients = p.IsAcceptingPatients,
                Status = p.Status,
                FirstVisitDate = p.FirstVisitDate,
                LastVisitDate = p.LastVisitDate
            })
            .FirstOrDefaultAsync();

        if (provider == null)
        {
            return NotFound();
        }

        return Ok(provider);
    }

    /// <summary>
    /// Add new provider
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProviderDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ProviderDto>> CreateProvider([FromBody] CreateProviderRequest request)
    {
        // Get provider type name
        var providerType = await _context.ProviderTypes.FindAsync(request.ProviderTypeId);
        if (providerType == null)
        {
            return BadRequest("Invalid provider type");
        }

        var provider = new PatientProvider
        {
            PatientProviderId = Guid.NewGuid(),
            PatientId = request.PatientId,
            ProviderTypeId = request.ProviderTypeId,
            ProviderTypeName = providerType.Name,
            ProviderName = request.ProviderName,
            Specialty = request.Specialty,
            NPI = request.NPI,
            Phone = request.Phone,
            Fax = request.Fax,
            Email = request.Email,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
            Country = request.Country ?? "USA",
            PracticeName = request.PracticeName,
            Website = request.Website,
            Notes = request.Notes,
            IsPrimary = request.IsPrimary,
            IsAcceptingPatients = request.IsAcceptingPatients,
            Status = "Active",
            FirstVisitDate = request.FirstVisitDate,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = User?.Identity?.Name ?? "system",
            IsActive = true
        };

        _context.PatientProviders.Add(provider);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Added provider {ProviderId} for patient {PatientId}", 
            provider.PatientProviderId, request.PatientId);

        var dto = new ProviderDto
        {
            PatientProviderId = provider.PatientProviderId,
            ProviderTypeId = provider.ProviderTypeId,
            ProviderTypeName = provider.ProviderTypeName,
            ProviderName = provider.ProviderName,
            Specialty = provider.Specialty,
            Phone = provider.Phone,
            IsPrimary = provider.IsPrimary,
            Status = provider.Status
        };

        return CreatedAtAction(nameof(GetProvider), new { providerId = provider.PatientProviderId }, dto);
    }

    /// <summary>
    /// Update provider
    /// </summary>
    [HttpPut("{providerId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProvider(Guid providerId, [FromBody] UpdateProviderRequest request)
    {
        var provider = await _context.PatientProviders.FindAsync(providerId);

        if (provider == null)
        {
            return NotFound();
        }

        // Update provider type if changed
        if (request.ProviderTypeId.HasValue && request.ProviderTypeId != provider.ProviderTypeId)
        {
            var providerType = await _context.ProviderTypes.FindAsync(request.ProviderTypeId.Value);
            if (providerType == null)
            {
                return BadRequest("Invalid provider type");
            }
            provider.ProviderTypeId = request.ProviderTypeId.Value;
            provider.ProviderTypeName = providerType.Name;
        }

        provider.ProviderName = request.ProviderName ?? provider.ProviderName;
        provider.Specialty = request.Specialty ?? provider.Specialty;
        provider.NPI = request.NPI ?? provider.NPI;
        provider.Phone = request.Phone ?? provider.Phone;
        provider.Fax = request.Fax ?? provider.Fax;
        provider.Email = request.Email ?? provider.Email;
        provider.AddressLine1 = request.AddressLine1 ?? provider.AddressLine1;
        provider.AddressLine2 = request.AddressLine2 ?? provider.AddressLine2;
        provider.City = request.City ?? provider.City;
        provider.State = request.State ?? provider.State;
        provider.ZipCode = request.ZipCode ?? provider.ZipCode;
        provider.PracticeName = request.PracticeName ?? provider.PracticeName;
        provider.Website = request.Website ?? provider.Website;
        provider.Notes = request.Notes ?? provider.Notes;
        provider.IsPrimary = request.IsPrimary ?? provider.IsPrimary;
        provider.IsAcceptingPatients = request.IsAcceptingPatients ?? provider.IsAcceptingPatients;
        provider.LastVisitDate = request.LastVisitDate ?? provider.LastVisitDate;
        provider.UpdatedAt = DateTime.UtcNow;
        provider.UpdatedBy = User?.Identity?.Name ?? "system";

        await _context.SaveChangesAsync();

        return Ok();
    }

    /// <summary>
    /// Delete provider
    /// </summary>
    [HttpDelete("{providerId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteProvider(Guid providerId)
    {
        var provider = await _context.PatientProviders.FindAsync(providerId);

        if (provider == null)
        {
            return NotFound();
        }

        provider.IsActive = false;
        provider.Status = "Inactive";
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Get all provider types
    /// </summary>
    [HttpGet("types")]
    [ProducesResponseType(typeof(List<ProviderTypeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProviderTypeDto>>> GetProviderTypes()
    {
        var types = await _context.ProviderTypes
            .Where(t => t.IsActive)
            .OrderBy(t => t.DisplayOrder)
            .Select(t => new ProviderTypeDto
            {
                ProviderTypeId = t.ProviderTypeId,
                Name = t.Name,
                Description = t.Description,
                DisplayOrder = t.DisplayOrder
            })
            .ToListAsync();

        return Ok(types);
    }
}

#region DTOs

public class ProviderDto
{
    public Guid PatientProviderId { get; set; }
    public int ProviderTypeId { get; set; }
    public string ProviderTypeName { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public string? Specialty { get; set; }
    public string? NPI { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
    public string? PracticeName { get; set; }
    public string? Website { get; set; }
    public string? Notes { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsAcceptingPatients { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? FirstVisitDate { get; set; }
    public DateTime? LastVisitDate { get; set; }
}

public class CreateProviderRequest
{
    public Guid PatientId { get; set; }
    public int ProviderTypeId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string? Specialty { get; set; }
    public string? NPI { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
    public string? PracticeName { get; set; }
    public string? Website { get; set; }
    public string? Notes { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsAcceptingPatients { get; set; } = true;
    public DateTime? FirstVisitDate { get; set; }
}

public class UpdateProviderRequest
{
    public int? ProviderTypeId { get; set; }
    public string? ProviderName { get; set; }
    public string? Specialty { get; set; }
    public string? NPI { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? PracticeName { get; set; }
    public string? Website { get; set; }
    public string? Notes { get; set; }
    public bool? IsPrimary { get; set; }
    public bool? IsAcceptingPatients { get; set; }
    public DateTime? LastVisitDate { get; set; }
}

public class ProviderTypeDto
{
    public int ProviderTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
}

#endregion

