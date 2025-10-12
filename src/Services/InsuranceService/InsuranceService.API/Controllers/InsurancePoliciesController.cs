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
    public class InsurancePoliciesController : ControllerBase
    {
        private readonly InsuranceDbContext _context;
        private readonly ILogger<InsurancePoliciesController> _logger;

        public InsurancePoliciesController(InsuranceDbContext context, ILogger<InsurancePoliciesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("patient/{patientId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<InsurancePolicyDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<InsurancePolicyDto>>>> GetByPatient(Guid patientId)
        {
            try
            {
                var policies = await _context.InsurancePolicies
                    .Where(p => p.PatientId == patientId && !p.IsDeleted)
                    .OrderByDescending(p => p.IsPrimary)
                    .ThenBy(p => p.Priority)
                    .ToListAsync();

                var dtos = policies.Select(MapToDto).ToList();
                return Ok(ApiResponse<IEnumerable<InsurancePolicyDto>>.SuccessResponse(dtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving insurance policies for patient: {patientId}");
                return StatusCode(500, ApiResponse<IEnumerable<InsurancePolicyDto>>.ErrorResponse("Error retrieving insurance policies"));
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<InsurancePolicyDto>), 200)]
        public async Task<ActionResult<ApiResponse<InsurancePolicyDto>>> GetById(Guid id)
        {
            try
            {
                var policy = await _context.InsurancePolicies.FindAsync(id);
                if (policy == null || policy.IsDeleted)
                    return NotFound(ApiResponse<InsurancePolicyDto>.ErrorResponse("Insurance policy not found"));

                return Ok(ApiResponse<InsurancePolicyDto>.SuccessResponse(MapToDto(policy)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving insurance policy: {id}");
                return StatusCode(500, ApiResponse<InsurancePolicyDto>.ErrorResponse("Error retrieving insurance policy"));
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<InsurancePolicyDto>), 201)]
        public async Task<ActionResult<ApiResponse<InsurancePolicyDto>>> Create([FromBody] CreateInsurancePolicyRequest request)
        {
            try
            {
                var policy = new InsurancePolicy
                {
                    PatientId = request.PatientId,
                    PolicyNumber = request.PolicyNumber,
                    GroupNumber = request.GroupNumber,
                    InsuranceCompanyName = request.InsuranceCompanyName,
                    PolicyType = request.PolicyType,
                    CoverageType = request.CoverageType,
                    PlanName = request.PlanName,
                    PlanNetwork = request.PlanNetwork,
                    EffectiveDate = request.EffectiveDate,
                    AnnualDeductible = request.AnnualDeductible,
                    OutOfPocketMax = request.OutOfPocketMax,
                    Status = "active",
                    CreatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system"
                };

                _context.InsurancePolicies.Add(policy);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Created insurance policy: {policy.Id} for patient: {request.PatientId}");
                return CreatedAtAction(nameof(GetById), new { id = policy.Id }, 
                    ApiResponse<InsurancePolicyDto>.SuccessResponse(MapToDto(policy), "Insurance policy created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating insurance policy");
                return StatusCode(500, ApiResponse<InsurancePolicyDto>.ErrorResponse("Error creating insurance policy"));
            }
        }

        [HttpPut("{id}/verify")]
        [ProducesResponseType(typeof(ApiResponse<InsurancePolicyDto>), 200)]
        public async Task<ActionResult<ApiResponse<InsurancePolicyDto>>> VerifyPolicy(Guid id)
        {
            try
            {
                var policy = await _context.InsurancePolicies.FindAsync(id);
                if (policy == null || policy.IsDeleted)
                    return NotFound(ApiResponse<InsurancePolicyDto>.ErrorResponse("Insurance policy not found"));

                policy.IsVerified = true;
                policy.VerifiedAt = DateTime.UtcNow;
                policy.VerifiedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
                policy.LastEligibilityCheck = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Verified insurance policy: {id}");
                
                return Ok(ApiResponse<InsurancePolicyDto>.SuccessResponse(MapToDto(policy), "Insurance policy verified"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error verifying insurance policy: {id}");
                return StatusCode(500, ApiResponse<InsurancePolicyDto>.ErrorResponse("Error verifying insurance policy"));
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            try
            {
                var policy = await _context.InsurancePolicies.FindAsync(id);
                if (policy == null)
                    return NotFound(ApiResponse<bool>.ErrorResponse("Insurance policy not found"));

                policy.IsDeleted = true;
                policy.DeletedAt = DateTime.UtcNow;
                policy.DeletedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
                
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Deleted insurance policy: {id}");
                
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Insurance policy deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting insurance policy: {id}");
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("Error deleting insurance policy"));
            }
        }

        private InsurancePolicyDto MapToDto(InsurancePolicy policy)
        {
            return new InsurancePolicyDto
            {
                Id = policy.Id,
                PatientId = policy.PatientId,
                PolicyNumber = policy.PolicyNumber,
                GroupNumber = policy.GroupNumber,
                InsuranceCompanyName = policy.InsuranceCompanyName,
                PolicyType = policy.PolicyType,
                CoverageType = policy.CoverageType,
                PlanName = policy.PlanName,
                PlanNetwork = policy.PlanNetwork,
                EffectiveDate = policy.EffectiveDate,
                TerminationDate = policy.TerminationDate,
                AnnualDeductible = policy.AnnualDeductible,
                DeductibleMet = policy.DeductibleMet,
                OutOfPocketMax = policy.OutOfPocketMax,
                OutOfPocketMet = policy.OutOfPocketMet,
                Status = policy.Status,
                IsPrimary = policy.IsPrimary,
                IsVerified = policy.IsVerified,
                CreatedAt = policy.CreatedAt
            };
        }
    }
}
