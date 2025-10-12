using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InsuranceService.Domain.DTOs;
using InsuranceService.Domain.Entities;
using InsuranceService.Infrastructure.Data;
using Shared.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InsuranceService.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class ClaimsController : ControllerBase
    {
        private readonly InsuranceDbContext _context;
        private readonly ILogger<ClaimsController> _logger;

        public ClaimsController(InsuranceDbContext context, ILogger<ClaimsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("patient/{patientId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ClaimDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ClaimDto>>>> GetByPatient(Guid patientId)
        {
            try
            {
                var claims = await _context.Claims
                    .Where(c => c.PatientId == patientId && !c.IsDeleted)
                    .OrderByDescending(c => c.ServiceDate)
                    .ToListAsync();

                var dtos = claims.Select(MapToDto).ToList();
                return Ok(ApiResponse<IEnumerable<ClaimDto>>.SuccessResponse(dtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving claims for patient: {patientId}");
                return StatusCode(500, ApiResponse<IEnumerable<ClaimDto>>.ErrorResponse("Error retrieving claims"));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ClaimDto>), 201)]
        public async Task<ActionResult<ApiResponse<ClaimDto>>> Create([FromBody] CreateClaimRequest request)
        {
            try
            {
                var claim = new Claim
                {
                    PatientId = request.PatientId,
                    InsurancePolicyId = request.InsurancePolicyId,
                    ClaimNumber = request.ClaimNumber,
                    ServiceDate = request.ServiceDate,
                    ClaimDate = DateTime.UtcNow,
                    TotalCharges = request.TotalCharges,
                    ProviderName = request.ProviderName,
                    PrimaryDiagnosisCode = request.PrimaryDiagnosisCode,
                    Status = "submitted",
                    CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "system"
                };

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Created claim: {claim.Id} for patient: {request.PatientId}");
                return CreatedAtAction(nameof(GetByPatient), new { patientId = request.PatientId }, 
                    ApiResponse<ClaimDto>.SuccessResponse(MapToDto(claim), "Claim created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating claim");
                return StatusCode(500, ApiResponse<ClaimDto>.ErrorResponse("Error creating claim"));
            }
        }

        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse<ClaimDto>), 200)]
        public async Task<ActionResult<ApiResponse<ClaimDto>>> UpdateStatus(Guid id, [FromBody] string status)
        {
            try
            {
                var claim = await _context.Claims.FindAsync(id);
                if (claim == null || claim.IsDeleted)
                    return NotFound(ApiResponse<ClaimDto>.ErrorResponse("Claim not found"));

                claim.Status = status;
                claim.ProcessedDate = DateTime.UtcNow;
                claim.UpdatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "system";
                
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Updated claim status: {id} to {status}");
                
                return Ok(ApiResponse<ClaimDto>.SuccessResponse(MapToDto(claim), "Claim status updated"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating claim status: {id}");
                return StatusCode(500, ApiResponse<ClaimDto>.ErrorResponse("Error updating claim status"));
            }
        }

        private ClaimDto MapToDto(Claim claim)
        {
            return new ClaimDto
            {
                Id = claim.Id,
                PatientId = claim.PatientId,
                InsurancePolicyId = claim.InsurancePolicyId,
                ClaimNumber = claim.ClaimNumber,
                ClaimType = claim.ClaimType,
                ServiceDate = claim.ServiceDate,
                ClaimDate = claim.ClaimDate,
                TotalCharges = claim.TotalCharges,
                PaidAmount = claim.PaidAmount,
                PatientResponsibility = claim.PatientResponsibility,
                Status = claim.Status,
                ProviderName = claim.ProviderName,
                CreatedAt = claim.CreatedAt
            };
        }
    }
}
