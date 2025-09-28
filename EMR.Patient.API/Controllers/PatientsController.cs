using EMR.Domain.Entities;
using EMR.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using PatientEntity = EMR.Domain.Entities.Patient;

namespace EMR.Patient.API.Controllers;

[ApiController]
[Route("api/patients")] 
public class PatientsController : ControllerBase
{
    private readonly IPatientRepository _patientRepository;

    public PatientsController(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var patient = await _patientRepository.GetByIdAsync(id, ct);
        if (patient is null) return NotFound();
        return Ok(patient);
    }

    public record CreatePatientRequest(string FirstName, string? MiddleName, string LastName, DateTime DateOfBirth, string? Gender, string? Phone, string? Email, string? City, string? State);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatientRequest request, CancellationToken ct)
    {
        var patient = new PatientEntity
        {
            PatientId = Guid.NewGuid(),
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Phone = request.Phone,
            Email = request.Email,
            City = request.City,
            State = request.State,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = "api",
            IsDeleted = false
        };

        await _patientRepository.AddAsync(patient, ct);
        return CreatedAtAction(nameof(GetById), new { id = patient.PatientId }, patient);
    }
}

