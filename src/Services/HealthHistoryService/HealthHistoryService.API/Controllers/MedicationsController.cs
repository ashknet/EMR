using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthHistoryService.Domain.DTOs;
using HealthHistoryService.Domain.Entities;
using HealthHistoryService.Infrastructure.Data;
using Shared.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HealthHistoryService.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class MedicationsController : ControllerBase
    {
        private readonly HealthHistoryDbContext _context;
        private readonly ILogger<MedicationsController> _logger;

        public MedicationsController(HealthHistoryDbContext context, ILogger<MedicationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("patient/{patientId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<MedicationDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<MedicationDto>>>> GetByPatient(Guid patientId)
        {
            try
            {
                var medications = await _context.Medications
                    .Where(m => m.PatientId == patientId && !m.IsDeleted)
                    .OrderByDescending(m => m.StartDate ?? m.CreatedAt)
                    .ToListAsync();

                var dtos = medications.Select(MapToDto).ToList();
                return Ok(ApiResponse<IEnumerable<MedicationDto>>.SuccessResponse(dtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving medications for patient: {patientId}");
                return StatusCode(500, ApiResponse<IEnumerable<MedicationDto>>.ErrorResponse("Error retrieving medications"));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<MedicationDto>), 201)]
        public async Task<ActionResult<ApiResponse<MedicationDto>>> Create([FromBody] CreateMedicationRequest request)
        {
            try
            {
                var medication = new Medication
                {
                    PatientId = request.PatientId,
                    MedicationName = request.MedicationName,
                    Dosage = request.Dosage,
                    Frequency = request.Frequency,
                    StartDate = request.StartDate,
                    PrescribedBy = request.PrescribedBy,
                    Instructions = request.Instructions,
                    Status = "active",
                    CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system"
                };

                _context.Medications.Add(medication);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Created medication: {medication.Id} for patient: {request.PatientId}");
                return CreatedAtAction(nameof(GetByPatient), new { patientId = request.PatientId }, 
                    ApiResponse<MedicationDto>.SuccessResponse(MapToDto(medication), "Medication created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating medication");
                return StatusCode(500, ApiResponse<MedicationDto>.ErrorResponse("Error creating medication"));
            }
        }

        [HttpPut("{id}/discontinue")]
        [ProducesResponseType(typeof(ApiResponse<MedicationDto>), 200)]
        public async Task<ActionResult<ApiResponse<MedicationDto>>> Discontinue(Guid id)
        {
            try
            {
                var medication = await _context.Medications.FindAsync(id);
                if (medication == null || medication.IsDeleted)
                    return NotFound(ApiResponse<MedicationDto>.ErrorResponse("Medication not found"));

                medication.Status = "stopped";
                medication.EndDate = DateTime.UtcNow;
                medication.UpdatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
                
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Discontinued medication: {id}");
                
                return Ok(ApiResponse<MedicationDto>.SuccessResponse(MapToDto(medication), "Medication discontinued"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error discontinuing medication: {id}");
                return StatusCode(500, ApiResponse<MedicationDto>.ErrorResponse("Error discontinuing medication"));
            }
        }

        private MedicationDto MapToDto(Medication medication)
        {
            return new MedicationDto
            {
                Id = medication.Id,
                PatientId = medication.PatientId,
                MedicationName = medication.MedicationName,
                GenericName = medication.GenericName,
                Dosage = medication.Dosage,
                Frequency = medication.Frequency,
                Status = medication.Status,
                StartDate = medication.StartDate,
                EndDate = medication.EndDate,
                PrescribedBy = medication.PrescribedBy,
                Instructions = medication.Instructions,
                CreatedAt = medication.CreatedAt
            };
        }
    }
}
