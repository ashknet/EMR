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
    public class AllergiesController : ControllerBase
    {
        private readonly HealthHistoryDbContext _context;
        private readonly ILogger<AllergiesController> _logger;

        public AllergiesController(HealthHistoryDbContext context, ILogger<AllergiesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("patient/{patientId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AllergyDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<AllergyDto>>>> GetByPatient(Guid patientId)
        {
            try
            {
                var allergies = await _context.Allergies
                    .Where(a => a.PatientId == patientId && !a.IsDeleted)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();

                var dtos = allergies.Select(MapToDto).ToList();
                return Ok(ApiResponse<IEnumerable<AllergyDto>>.SuccessResponse(dtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving allergies for patient: {patientId}");
                return StatusCode(500, ApiResponse<IEnumerable<AllergyDto>>.ErrorResponse("Error retrieving allergies"));
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<AllergyDto>), 200)]
        public async Task<ActionResult<ApiResponse<AllergyDto>>> GetById(Guid id)
        {
            try
            {
                var allergy = await _context.Allergies.FindAsync(id);
                if (allergy == null || allergy.IsDeleted)
                    return NotFound(ApiResponse<AllergyDto>.ErrorResponse("Allergy not found"));

                return Ok(ApiResponse<AllergyDto>.SuccessResponse(MapToDto(allergy)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving allergy: {id}");
                return StatusCode(500, ApiResponse<AllergyDto>.ErrorResponse("Error retrieving allergy"));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AllergyDto>), 201)]
        public async Task<ActionResult<ApiResponse<AllergyDto>>> Create([FromBody] CreateAllergyRequest request)
        {
            try
            {
                var allergy = new Allergy
                {
                    PatientId = request.PatientId,
                    AllergenName = request.AllergenName,
                    Category = request.Category,
                    Criticality = request.Criticality,
                    Severity = request.Severity,
                    ReactionDescription = request.ReactionDescription,
                    OnsetDate = request.OnsetDate,
                    Notes = request.Notes,
                    CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system"
                };

                _context.Allergies.Add(allergy);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Created allergy: {allergy.Id} for patient: {request.PatientId}");
                return CreatedAtAction(nameof(GetById), new { id = allergy.Id }, 
                    ApiResponse<AllergyDto>.SuccessResponse(MapToDto(allergy), "Allergy created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating allergy");
                return StatusCode(500, ApiResponse<AllergyDto>.ErrorResponse("Error creating allergy"));
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            try
            {
                var allergy = await _context.Allergies.FindAsync(id);
                if (allergy == null)
                    return NotFound(ApiResponse<bool>.ErrorResponse("Allergy not found"));

                allergy.IsDeleted = true;
                allergy.DeletedAt = DateTime.UtcNow;
                allergy.DeletedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
                
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Deleted allergy: {id}");
                
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Allergy deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting allergy: {id}");
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("Error deleting allergy"));
            }
        }

        private AllergyDto MapToDto(Allergy allergy)
        {
            return new AllergyDto
            {
                Id = allergy.Id,
                PatientId = allergy.PatientId,
                AllergenName = allergy.AllergenName,
                Category = allergy.Category,
                Criticality = allergy.Criticality,
                Severity = allergy.Severity,
                ClinicalStatus = allergy.ClinicalStatus,
                ReactionDescription = allergy.ReactionDescription,
                OnsetDate = allergy.OnsetDate,
                Notes = allergy.Notes,
                CreatedAt = allergy.CreatedAt
            };
        }
    }
}
