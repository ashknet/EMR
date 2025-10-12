using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientService.Domain.DTOs;
using PatientService.Domain.Entities;
using PatientService.Infrastructure.Repositories;
using Shared.Common.Models;
using Shared.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PatientService.API.Controllers
{
    /// <summary>
    /// Patient management controller
    /// Handles patient CRUD operations with HIPAA compliance
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class PatientsController : ControllerBase
    {
        private readonly PatientRepository _patientRepository;
        private readonly ILogger<PatientsController> _logger;
        private readonly byte[] _encryptionKey; // In production, retrieve from Azure Key Vault

        public PatientsController(
            PatientRepository patientRepository,
            ILogger<PatientsController> logger,
            IConfiguration configuration)
        {
            _patientRepository = patientRepository;
            _logger = logger;
            
            // In production, retrieve from Azure Key Vault
            var keyString = configuration["EncryptionKey"] ?? "DefaultKeyForDevelopmentOnly123456";
            _encryptionKey = System.Text.Encoding.UTF8.GetBytes(keyString.PadRight(32).Substring(0, 32));
        }

        /// <summary>
        /// Get all patients
        /// </summary>
        /// <returns>List of patients</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PatientDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PatientDto>>>> GetAll()
        {
            try
            {
                var patients = await _patientRepository.GetAllAsync();
                var patientDtos = patients.Select(MapToDto).ToList();
                
                _logger.LogInformation($"Retrieved {patientDtos.Count} patients");
                
                return Ok(ApiResponse<IEnumerable<PatientDto>>.SuccessResponse(
                    patientDtos, 
                    $"Retrieved {patientDtos.Count} patients"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patients");
                return StatusCode(500, ApiResponse<IEnumerable<PatientDto>>.ErrorResponse(
                    "An error occurred while retrieving patients"));
            }
        }

        /// <summary>
        /// Get patient by ID
        /// </summary>
        /// <param name="id">Patient ID</param>
        /// <returns>Patient details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PatientDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<PatientDto>>> GetById(Guid id)
        {
            try
            {
                var patient = await _patientRepository.GetByIdAsync(id);
                
                if (patient == null)
                {
                    _logger.LogWarning($"Patient not found: {id}");
                    return NotFound(ApiResponse<PatientDto>.ErrorResponse("Patient not found"));
                }
                
                var patientDto = MapToDto(patient);
                
                _logger.LogInformation($"Retrieved patient: {id}");
                
                return Ok(ApiResponse<PatientDto>.SuccessResponse(patientDto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving patient: {id}");
                return StatusCode(500, ApiResponse<PatientDto>.ErrorResponse(
                    "An error occurred while retrieving the patient"));
            }
        }

        /// <summary>
        /// Create a new patient
        /// </summary>
        /// <param name="request">Patient creation request</param>
        /// <returns>Created patient</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PatientDto>), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<ApiResponse<PatientDto>>> Create([FromBody] CreatePatientRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<PatientDto>.ErrorResponse(
                        "Invalid request data",
                        ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()));
                }

                var patient = new Patient
                {
                    FirstName = request.FirstName,
                    MiddleName = request.MiddleName,
                    LastName = request.LastName,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    ProfileType = request.ProfileType,
                    PrimaryAccountHolderId = request.PrimaryAccountHolderId,
                    RelationshipToPrimary = request.RelationshipToPrimary,
                    IsPrimaryAccountHolder = request.ProfileType == "Self" && request.PrimaryAccountHolderId == null,
                    IsActive = true,
                    IsMinor = CalculateAge(request.DateOfBirth) < 18,
                    CreatedBy = GetCurrentUserId()
                };

                // Encrypt sensitive data
                if (!string.IsNullOrEmpty(patient.Email))
                {
                    patient.Email = EncryptionHelper.Encrypt(patient.Email, _encryptionKey);
                }
                if (!string.IsNullOrEmpty(patient.PhoneNumber))
                {
                    patient.PhoneNumber = EncryptionHelper.Encrypt(patient.PhoneNumber, _encryptionKey);
                }

                var createdPatient = await _patientRepository.AddAsync(patient);
                var patientDto = MapToDto(createdPatient);
                
                _logger.LogInformation($"Created patient: {createdPatient.Id}");
                
                return CreatedAtAction(
                    nameof(GetById), 
                    new { id = createdPatient.Id }, 
                    ApiResponse<PatientDto>.SuccessResponse(patientDto, "Patient created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient");
                return StatusCode(500, ApiResponse<PatientDto>.ErrorResponse(
                    "An error occurred while creating the patient"));
            }
        }

        /// <summary>
        /// Update an existing patient
        /// </summary>
        /// <param name="id">Patient ID</param>
        /// <param name="request">Patient update request</param>
        /// <returns>Updated patient</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PatientDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<PatientDto>>> Update(Guid id, [FromBody] UpdatePatientRequest request)
        {
            try
            {
                if (id != request.Id)
                {
                    return BadRequest(ApiResponse<PatientDto>.ErrorResponse("ID mismatch"));
                }

                var patient = await _patientRepository.GetByIdAsync(id);
                if (patient == null)
                {
                    return NotFound(ApiResponse<PatientDto>.ErrorResponse("Patient not found"));
                }

                // Update fields
                if (!string.IsNullOrEmpty(request.FirstName))
                    patient.FirstName = request.FirstName;
                if (!string.IsNullOrEmpty(request.LastName))
                    patient.LastName = request.LastName;
                if (!string.IsNullOrEmpty(request.MiddleName))
                    patient.MiddleName = request.MiddleName;
                if (!string.IsNullOrEmpty(request.PreferredName))
                    patient.PreferredName = request.PreferredName;
                
                // Update encrypted fields
                if (!string.IsNullOrEmpty(request.Email))
                    patient.Email = EncryptionHelper.Encrypt(request.Email, _encryptionKey);
                if (!string.IsNullOrEmpty(request.PhoneNumber))
                    patient.PhoneNumber = EncryptionHelper.Encrypt(request.PhoneNumber, _encryptionKey);

                patient.UpdatedBy = GetCurrentUserId();

                var updatedPatient = await _patientRepository.UpdateAsync(patient);
                var patientDto = MapToDto(updatedPatient);
                
                _logger.LogInformation($"Updated patient: {id}");
                
                return Ok(ApiResponse<PatientDto>.SuccessResponse(patientDto, "Patient updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating patient: {id}");
                return StatusCode(500, ApiResponse<PatientDto>.ErrorResponse(
                    "An error occurred while updating the patient"));
            }
        }

        /// <summary>
        /// Delete a patient (soft delete)
        /// </summary>
        /// <param name="id">Patient ID</param>
        /// <returns>Success response</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            try
            {
                var result = await _patientRepository.SoftDeleteAsync(id, GetCurrentUserId());
                
                if (!result)
                {
                    return NotFound(ApiResponse<bool>.ErrorResponse("Patient not found"));
                }
                
                _logger.LogInformation($"Deleted patient: {id}");
                
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Patient deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting patient: {id}");
                return StatusCode(500, ApiResponse<bool>.ErrorResponse(
                    "An error occurred while deleting the patient"));
            }
        }

        /// <summary>
        /// Search patients
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>List of matching patients</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PatientDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PatientDto>>>> Search([FromQuery] string searchTerm)
        {
            try
            {
                var patients = await _patientRepository.SearchPatientsAsync(searchTerm);
                var patientDtos = patients.Select(MapToDto).ToList();
                
                _logger.LogInformation($"Search returned {patientDtos.Count} results for term: {searchTerm}");
                
                return Ok(ApiResponse<IEnumerable<PatientDto>>.SuccessResponse(patientDtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching patients: {searchTerm}");
                return StatusCode(500, ApiResponse<IEnumerable<PatientDto>>.ErrorResponse(
                    "An error occurred while searching patients"));
            }
        }

        /// <summary>
        /// Get family members for a primary account holder
        /// </summary>
        /// <param name="primaryAccountHolderId">Primary account holder ID</param>
        /// <returns>List of family members</returns>
        [HttpGet("family/{primaryAccountHolderId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PatientDto>>), 200)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PatientDto>>>> GetFamilyMembers(Guid primaryAccountHolderId)
        {
            try
            {
                var patients = await _patientRepository.GetFamilyMembersAsync(primaryAccountHolderId);
                var patientDtos = patients.Select(MapToDto).ToList();
                
                _logger.LogInformation($"Retrieved {patientDtos.Count} family members for account: {primaryAccountHolderId}");
                
                return Ok(ApiResponse<IEnumerable<PatientDto>>.SuccessResponse(patientDtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving family members: {primaryAccountHolderId}");
                return StatusCode(500, ApiResponse<IEnumerable<PatientDto>>.ErrorResponse(
                    "An error occurred while retrieving family members"));
            }
        }

        // Helper methods
        private PatientDto MapToDto(Patient patient)
        {
            // Decrypt sensitive fields for display
            var email = patient.Email;
            var phone = patient.PhoneNumber;

            try
            {
                if (!string.IsNullOrEmpty(email))
                    email = EncryptionHelper.Decrypt(email, _encryptionKey);
                if (!string.IsNullOrEmpty(phone))
                    phone = EncryptionHelper.Decrypt(phone, _encryptionKey);
            }
            catch
            {
                // If decryption fails, return masked values
                email = "***@***.***";
                phone = "***-***-****";
            }

            return new PatientDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                MiddleName = patient.MiddleName,
                LastName = patient.LastName,
                PreferredName = patient.PreferredName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                BiologicalSex = patient.BiologicalSex,
                Email = email,
                PhoneNumber = phone,
                ProfileType = patient.ProfileType,
                PrimaryAccountHolderId = patient.PrimaryAccountHolderId,
                IsPrimaryAccountHolder = patient.IsPrimaryAccountHolder,
                RelationshipToPrimary = patient.RelationshipToPrimary,
                IsActive = patient.IsActive,
                IsVerified = patient.IsVerified,
                IsMinor = patient.IsMinor,
                HasConsentedToDataSharing = patient.HasConsentedToDataSharing,
                ConsentDate = patient.ConsentDate,
                ProfilePhotoUrl = patient.ProfilePhotoUrl,
                CreatedAt = patient.CreatedAt,
                UpdatedAt = patient.UpdatedAt
            };
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
