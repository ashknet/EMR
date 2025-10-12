using Microsoft.AspNetCore.Mvc;
using FamilyService.Domain.DTOs;
using FamilyService.Domain.Entities;
using FamilyService.Infrastructure.Repositories;
using Shared.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FamilyService.API.Controllers
{
    /// <summary>
    /// Family group management controller
    /// Handles family creation, member management, and relationships
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class FamilyGroupsController : ControllerBase
    {
        private readonly FamilyGroupRepository _repository;
        private readonly ILogger<FamilyGroupsController> _logger;

        public FamilyGroupsController(
            FamilyGroupRepository repository,
            ILogger<FamilyGroupsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Get all family groups
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<FamilyGroupDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<FamilyGroupDto>>>> GetAll()
        {
            try
            {
                var groups = await _repository.GetAllAsync();
                var dtos = groups.Select(MapToDto).ToList();
                
                _logger.LogInformation($"Retrieved {dtos.Count} family groups");
                
                return Ok(ApiResponse<IEnumerable<FamilyGroupDto>>.SuccessResponse(dtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving family groups");
                return StatusCode(500, ApiResponse<IEnumerable<FamilyGroupDto>>.ErrorResponse(
                    "An error occurred while retrieving family groups"));
            }
        }

        /// <summary>
        /// Get family group by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<FamilyGroupDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<FamilyGroupDto>>> GetById(Guid id)
        {
            try
            {
                var group = await _repository.GetByIdAsync(id);
                
                if (group == null)
                {
                    return NotFound(ApiResponse<FamilyGroupDto>.ErrorResponse("Family group not found"));
                }
                
                var dto = MapToDto(group);
                return Ok(ApiResponse<FamilyGroupDto>.SuccessResponse(dto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving family group: {id}");
                return StatusCode(500, ApiResponse<FamilyGroupDto>.ErrorResponse(
                    "An error occurred while retrieving the family group"));
            }
        }

        /// <summary>
        /// Get family group by primary account holder
        /// </summary>
        [HttpGet("by-account-holder/{accountHolderId}")]
        [ProducesResponseType(typeof(ApiResponse<FamilyGroupDto>), 200)]
        public async Task<ActionResult<ApiResponse<FamilyGroupDto>>> GetByAccountHolder(Guid accountHolderId)
        {
            try
            {
                var group = await _repository.GetByPrimaryAccountHolderAsync(accountHolderId);
                
                if (group == null)
                {
                    return NotFound(ApiResponse<FamilyGroupDto>.ErrorResponse("Family group not found for this account holder"));
                }
                
                var dto = MapToDto(group);
                return Ok(ApiResponse<FamilyGroupDto>.SuccessResponse(dto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving family group for account holder: {accountHolderId}");
                return StatusCode(500, ApiResponse<FamilyGroupDto>.ErrorResponse(
                    "An error occurred while retrieving the family group"));
            }
        }

        /// <summary>
        /// Create a new family group
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<FamilyGroupDto>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ApiResponse<FamilyGroupDto>>> Create([FromBody] CreateFamilyGroupRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<FamilyGroupDto>.ErrorResponse(
                        "Invalid request data",
                        ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
                }

                var familyGroup = new FamilyGroup
                {
                    FamilyName = request.FamilyName,
                    PrimaryAccountHolderId = request.PrimaryAccountHolderId,
                    PrimaryEmail = request.PrimaryEmail,
                    AddressLine1 = request.AddressLine1,
                    City = request.City,
                    State = request.State,
                    ZipCode = request.ZipCode,
                    IsActive = true,
                    CreatedBy = GetCurrentUserId()
                };

                var created = await _repository.AddAsync(familyGroup);
                var dto = MapToDto(created);
                
                _logger.LogInformation($"Created family group: {created.Id}");
                
                return CreatedAtAction(
                    nameof(GetById), 
                    new { id = created.Id }, 
                    ApiResponse<FamilyGroupDto>.SuccessResponse(dto, "Family group created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating family group");
                return StatusCode(500, ApiResponse<FamilyGroupDto>.ErrorResponse(
                    "An error occurred while creating the family group"));
            }
        }

        /// <summary>
        /// Add a member to family group
        /// </summary>
        [HttpPost("members")]
        [ProducesResponseType(typeof(ApiResponse<FamilyMemberDto>), 200)]
        public async Task<ActionResult<ApiResponse<FamilyMemberDto>>> AddMember([FromBody] AddFamilyMemberRequest request)
        {
            try
            {
                var group = await _repository.GetByIdAsync(request.FamilyGroupId);
                if (group == null)
                {
                    return NotFound(ApiResponse<FamilyMemberDto>.ErrorResponse("Family group not found"));
                }

                var member = new FamilyMember
                {
                    FamilyGroupId = request.FamilyGroupId,
                    PatientId = request.PatientId,
                    RelationshipType = request.RelationshipType,
                    RelationshipToHead = request.RelationshipToHead,
                    CanManageFamilyData = request.CanManageFamilyData,
                    CanViewAllRecords = request.CanViewAllRecords,
                    CanShareOnBehalf = request.CanShareOnBehalf,
                    IsActive = true,
                    CreatedBy = GetCurrentUserId()
                };

                group.Members.Add(member);
                await _repository.UpdateAsync(group);

                _logger.LogInformation($"Added member {request.PatientId} to family group {request.FamilyGroupId}");

                var dto = new FamilyMemberDto
                {
                    Id = member.Id,
                    FamilyGroupId = member.FamilyGroupId,
                    PatientId = member.PatientId,
                    RelationshipType = member.RelationshipType,
                    IsActive = member.IsActive,
                    JoinedDate = member.JoinedDate
                };

                return Ok(ApiResponse<FamilyMemberDto>.SuccessResponse(dto, "Member added successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding family member");
                return StatusCode(500, ApiResponse<FamilyMemberDto>.ErrorResponse(
                    "An error occurred while adding the family member"));
            }
        }

        /// <summary>
        /// Delete a family group
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            try
            {
                var result = await _repository.SoftDeleteAsync(id, GetCurrentUserId());
                
                if (!result)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse("Family group not found"));
                }
                
                _logger.LogInformation($"Deleted family group: {id}");
                
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Family group deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting family group: {id}");
                return StatusCode(500, ApiResponse<bool>.ErrorResponse(
                    "An error occurred while deleting the family group"));
            }
        }

        private FamilyGroupDto MapToDto(FamilyGroup group)
        {
            return new FamilyGroupDto
            {
                Id = group.Id,
                FamilyName = group.FamilyName,
                PrimaryAccountHolderId = group.PrimaryAccountHolderId,
                PrimaryEmail = group.PrimaryEmail,
                PrimaryPhone = group.PrimaryPhone,
                AddressLine1 = group.AddressLine1,
                AddressLine2 = group.AddressLine2,
                City = group.City,
                State = group.State,
                ZipCode = group.ZipCode,
                Country = group.Country,
                IsActive = group.IsActive,
                MemberCount = group.Members.Count,
                Members = group.Members.Select(m => new FamilyMemberDto
                {
                    Id = m.Id,
                    FamilyGroupId = m.FamilyGroupId,
                    PatientId = m.PatientId,
                    RelationshipType = m.RelationshipType,
                    RelationshipToHead = m.RelationshipToHead,
                    IsHead = m.IsHead,
                    CanManageFamilyData = m.CanManageFamilyData,
                    CanViewAllRecords = m.CanViewAllRecords,
                    CanShareOnBehalf = m.CanShareOnBehalf,
                    IsActive = m.IsActive,
                    JoinedDate = m.JoinedDate
                }).ToList(),
                CreatedAt = group.CreatedAt,
                UpdatedAt = group.UpdatedAt
            };
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        }
    }
}
