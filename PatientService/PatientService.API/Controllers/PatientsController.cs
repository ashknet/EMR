using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Infrastructure.Data;
using PatientService.Infrastructure.Services;
using AutoMapper;
using System.Text.Json;
using PatientService.API.Models;

namespace PatientService.API.Controllers;

/// <summary>
/// Patient Demographics API - manages patient information
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PatientsController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<PatientsController> _logger;
    private readonly IMapper _mapper;

    public PatientsController(
        PatientDbContext context,
        IEncryptionService encryptionService,
        ILogger<PatientsController> logger,
        IMapper mapper)
    {
        _context = context;
        _encryptionService = encryptionService;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Get patient by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> GetPatient(Guid id)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.PatientId == id && !p.IsDeleted);

        if (patient == null)
        {
            return NotFound(new { message = "Patient not found", traceId = HttpContext.TraceIdentifier });
        }

        var dto = await MapToDto(patient);

        await LogAudit(id, "Read", "Patient", "Viewed patient demographics");

        return Ok(dto);
    }

    // GET api/patients/{id}/step/{stepName} - Get step-specific data
    [HttpGet("{id}/step/{stepName}")]
    public async Task<IActionResult> GetPatientStepData(Guid id, string stepName)
    {
        try
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == id && !p.IsDeleted);

            if (patient == null)
            {
                return NotFound(new { message = "Patient not found", traceId = HttpContext.TraceIdentifier });
            }

            object stepData = stepName.ToLower() switch
            {
                "basicinfo" => new PatientBasicInfoDto
                {
                    PatientId = patient.PatientId,
                    FirstName = patient.FirstName,
                    LastName = patient.LastName,
                    MiddleName = patient.MiddleName,
                    Suffix = patient.Suffix,
                    DateOfBirth = patient.DateOfBirth,
                    GenderId = patient.GenderId,
                    SsnEncrypted = patient.SsnEncrypted,
                    MaritalStatusId = patient.MaritalStatusId,
                    RaceId = patient.RaceId,
                    Ethnicity = patient.Ethnicity,
                    PrimaryLanguageId = patient.PrimaryLanguageId,
                    PreferredLanguageId = patient.PreferredLanguageId,
                    CommunicationPrefId = patient.CommunicationPrefId,
                    InterpreterRequired = patient.InterpreterRequired,
                    MobilityAssistance = patient.MobilityAssistance,
                    ReligiousConsiderations = patient.ReligiousConsiderations,
                    PrimaryCarePhysician = patient.PrimaryCarePhysician,
                    PcpPhoneNumber = patient.PCPPhoneNumber,
                    ReferringPhysician = patient.ReferringPhysician,
                    PreferredPharmacyName = patient.PreferredPharmacyName,
                    PreferredPharmacyLocation = patient.PreferredPharmacyLocation,
                    PhotoIdPath = patient.PhotoIdPath,
                    ElectronicSignature = patient.ElectronicSignature,
                    SignedDate = patient.SignedDate
                },
                "contactinfo" => new PatientContactInfoDto
                {
                    PatientId = patient.PatientId,
                    Email = patient.Email,
                    PhoneNumber = patient.PhoneNumber,
                    AlternatePhone = patient.AlternatePhone,
                    EmergencyContactName = patient.EmergencyContactName,
                    EmergencyContactPhone = patient.EmergencyContactPhone,
                    EmergencyContactRelationship = patient.EmergencyContactRelationship
                },
                "address" => new PatientAddressDto
                {
                    PatientId = patient.PatientId,
                    AddressLine1 = patient.AddressLine1,
                    AddressLine2 = patient.AddressLine2,
                    City = patient.City,
                    State = patient.State,
                    ZipCode = patient.ZipCode,
                    Country = patient.Country
                },
                "emergencycontacts" => new PatientEmergencyContactsDto
                {
                    PatientId = patient.PatientId,
                    EmergencyContacts = await _context.EmergencyContacts
                        .Where(ec => ec.PatientId == id)
                        .Select(ec => new Models.EmergencyContactDto(
                            ec.Name,
                            ec.RelationshipTypeId,
                            ec.Phone,
                            ec.AltPhone
                        )).ToListAsync()
                },
                "socialhistory" => new PatientSocialHistoryDto
                {
                    PatientId = patient.PatientId,
                    SmokingStatusId = (await _context.SocialHistories.FirstOrDefaultAsync(sh => sh.PatientId == id))?.SmokingStatusId,
                    AlcoholUseId = (await _context.SocialHistories.FirstOrDefaultAsync(sh => sh.PatientId == id))?.AlcoholUseId,
                    DrugUseId = (await _context.SocialHistories.FirstOrDefaultAsync(sh => sh.PatientId == id))?.DrugUseId,
                    Occupation = (await _context.SocialHistories.FirstOrDefaultAsync(sh => sh.PatientId == id))?.Occupation,
                    LivingSituationId = (await _context.SocialHistories.FirstOrDefaultAsync(sh => sh.PatientId == id))?.LivingSituationId
                },
                "allergies" => new PatientAllergiesDto
                {
                    PatientId = patient.PatientId,
                    Allergies = await _context.Allergies
                        .Where(a => a.PatientId == id)
                        .Select(a => new Models.AllergyDto(
                            a.AllergenName,
                            a.AllergenType,
                            a.Severity,
                            a.Reaction,
                            a.OnsetDate
                        )).ToListAsync()
                },
                "medications" => new PatientMedicationsDto
                {
                    PatientId = patient.PatientId,
                    Medications = await _context.Medications
                        .Where(m => m.PatientId == id)
                        .Select(m => new Models.MedicationDto(
                            m.MedicationName,
                            m.Dosage,
                            m.Frequency,
                            m.Prescriber,
                            m.StartDate
                        )).ToListAsync()
                },
                "medicalhistory" => new PatientMedicalHistoryDto
                {
                    PatientId = patient.PatientId,
                    Allergies = await _context.Allergies
                        .Where(a => a.PatientId == id)
                        .Select(a => new Models.AllergyDto(
                            a.AllergenName,
                            a.AllergenType,
                            a.Severity,
                            a.Reaction,
                            a.OnsetDate
                        )).ToListAsync(),
                    Medications = await _context.Medications
                        .Where(m => m.PatientId == id)
                        .Select(m => new Models.MedicationDto(
                            m.MedicationName,
                            m.Dosage,
                            m.Frequency,
                            m.Prescriber,
                            m.StartDate
                        )).ToListAsync(),
                    ChronicConditions = await _context.ChronicConditions
                        .Where(c => c.PatientId == id)
                        .Select(c => new Models.ChronicConditionDto(
                            c.ConditionName,
                            c.DiagnosedDate,
                            c.Status,
                            c.Notes
                        )).ToListAsync(),
                    Immunizations = await _context.Immunizations
                        .Where(i => i.PatientId == id)
                        .Select(i => new Models.ImmunizationDto(
                            i.VaccineName,
                            i.AdministeredDate,
                            i.DoseNumber,
                            i.ProviderName, // Changed from Provider
                            i.LotNumber,
                            i.ExpirationDate,
                            i.Site,
                            i.Route,
                            null // Notes column doesn't exist in database
                        )).ToListAsync(),
                    Surgeries = await _context.PatientSurgeries
                        .Where(s => s.PatientId == id)
                        .Select(s => new Models.SurgeryDto(
                            s.SurgeryType,
                            s.SurgeryDate,
                            s.Notes
                        )).ToListAsync(),
                    Hospitalizations = await _context.PatientHospitalizations
                        .Where(h => h.PatientId == id)
                        .Select(h => new Models.HospitalizationDto(
                            h.HospitalName,
                            h.Reason,
                            h.AdmissionDate,
                            h.DischargeDate,
                            h.Notes
                        )).ToListAsync(),
                    FamilyMedicalHistory = await _context.FamilyMedicalHistories
                        .Where(f => f.PatientId == id)
                        .Select(f => new Models.FamilyHistoryDto(
                            f.Relative,
                            f.Condition,
                            f.Notes
                        )).ToListAsync()
                },
                "insurance" => new PatientInsuranceDto
                {
                    PatientId = patient.PatientId,
                    InsurancePolicies = await _context.InsurancePolicies
                        .Where(ip => ip.PatientId == id)
                        .Select(ip => new Models.InsurancePolicyDto(
                            ip.ProviderId,
                            ip.GroupNumber,
                            ip.PolicyNumber,
                            ip.PolicyHolderName,
                            ip.ProviderPhone,
                            ip.MedicareMedicaidId,
                            ip.CardImagePath
                        )).ToListAsync()
                },
                "legalconsents" => new PatientLegalConsentsDto
                {
                    PatientId = patient.PatientId,
                    HipaaAgreed = (await _context.LegalConsents.FirstOrDefaultAsync(lc => lc.PatientId == id))?.HipaaAgreed,
                    ConsentToTreat = (await _context.LegalConsents.FirstOrDefaultAsync(lc => lc.PatientId == id))?.ConsentToTreat,
                    AdvanceDirectives = (await _context.LegalConsents.FirstOrDefaultAsync(lc => lc.PatientId == id))?.AdvanceDirectives,
                    AdvanceDirectivesPath = (await _context.LegalConsents.FirstOrDefaultAsync(lc => lc.PatientId == id))?.AdvanceDirectivesPath,
                    AssignmentOfBenefits = (await _context.LegalConsents.FirstOrDefaultAsync(lc => lc.PatientId == id))?.AssignmentOfBenefits,
                    FinancialResponsibility = (await _context.LegalConsents.FirstOrDefaultAsync(lc => lc.PatientId == id))?.FinancialResponsibility,
                    SignedOnUtc = (await _context.LegalConsents.FirstOrDefaultAsync(lc => lc.PatientId == id))?.SignedOnUtc,
                    SignaturePath = (await _context.LegalConsents.FirstOrDefaultAsync(lc => lc.PatientId == id))?.SignaturePath
                },
                _ => throw new ArgumentException($"Invalid step name: {stepName}")
            };

            await LogAudit(id, "Read", $"Patient{stepName}", $"Viewed {stepName} data");

            return Ok(stepData);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message, traceId = HttpContext.TraceIdentifier });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving {StepName} data for patient {PatientId}", stepName, id);
            return StatusCode(500, new { message = "An error occurred while retrieving the step data", traceId = HttpContext.TraceIdentifier });
        }
    }

    /// <summary>
    /// Create new patient
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PatientDto>> CreatePatient([FromBody] CreatePatientRequest request)
    {
        // Validate email uniqueness
        if (await _context.Patients.AnyAsync(p => p.Email == request.Email && !p.IsDeleted))
        {
            return BadRequest(new { message = "Email already exists", traceId = HttpContext.TraceIdentifier });
        }

        var patient = new Patient
        {
            PatientId = Guid.NewGuid(),
            FirstName = request.FirstName,
            MiddleName = request.MiddleName ?? string.Empty,
            LastName = request.LastName,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            AlternatePhone = request.AlternatePhone,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
            Country = request.Country ?? "USA",
            Race = request.Race,
            Ethnicity = request.Ethnicity,
            PreferredLanguage = request.PreferredLanguage,
            MaritalStatus = request.MaritalStatus,
            EmergencyContactName = request.EmergencyContactName,
            EmergencyContactPhone = request.EmergencyContactPhone,
            EmergencyContactRelationship = request.EmergencyContactRelationship,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = GetCurrentUserId(),
            IsActive = true
        };

        // Encrypt SSN if provided
        if (!string.IsNullOrEmpty(request.Ssn))
        {
            patient.SsnEncrypted = await _encryptionService.EncryptAsync(request.Ssn);
            patient.SsnHash = _encryptionService.ComputeHash(request.Ssn);
        }

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        await LogAudit(patient.PatientId, "Create", "Patient", "Created new patient");

        _logger.LogInformation("Created patient {PatientId}", patient.PatientId);

        var dto = await MapToDto(patient);

        return CreatedAtAction(nameof(GetPatient), new { id = patient.PatientId }, dto);
    }

    /// <summary>
    /// Update patient demographics
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PatientDto>> UpdatePatient(Guid id, [FromBody] UpdatePatientRequest request)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == id && !p.IsDeleted);

        if (patient == null)
        {
            return NotFound(new { message = "Patient not found", traceId = HttpContext.TraceIdentifier });
        }

        var oldValues = System.Text.Json.JsonSerializer.Serialize(new
        {
            patient.FirstName,
            patient.LastName,
            patient.Email,
            patient.PhoneNumber
        });

        // Update fields
        patient.FirstName = request.FirstName;
        patient.MiddleName = request.MiddleName ?? string.Empty;
        patient.LastName = request.LastName;
        patient.Email = request.Email;
        patient.PhoneNumber = request.PhoneNumber;
        patient.AlternatePhone = request.AlternatePhone;
        patient.AddressLine1 = request.AddressLine1;
        patient.AddressLine2 = request.AddressLine2;
        patient.City = request.City;
        patient.State = request.State;
        patient.ZipCode = request.ZipCode;
        patient.Race = request.Race;
        patient.Ethnicity = request.Ethnicity;
        patient.PreferredLanguage = request.PreferredLanguage;
        patient.MaritalStatus = request.MaritalStatus;
        patient.EmergencyContactName = request.EmergencyContactName;
        patient.EmergencyContactPhone = request.EmergencyContactPhone;
        patient.EmergencyContactRelationship = request.EmergencyContactRelationship;
        patient.UpdatedAt = DateTime.UtcNow;
        patient.UpdatedBy = GetCurrentUserId();

        await _context.SaveChangesAsync();

        await LogAudit(id, "Update", "Patient", "Updated patient demographics", oldValues);

        _logger.LogInformation("Updated patient {PatientId}", id);

        var dto = await MapToDto(patient);

        return Ok(dto);
    }

    /// <summary>
    /// Delete patient (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePatient(Guid id)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == id && !p.IsDeleted);

        if (patient == null)
        {
            return NotFound(new { message = "Patient not found", traceId = HttpContext.TraceIdentifier });
        }

        patient.IsDeleted = true;
        patient.IsActive = false;
        patient.UpdatedAt = DateTime.UtcNow;
        patient.UpdatedBy = GetCurrentUserId();

        await _context.SaveChangesAsync();

        await LogAudit(id, "Delete", "Patient", "Soft deleted patient");

        _logger.LogWarning("Deleted patient {PatientId}", id);

        return NoContent();
    }

    // PUT api/patients/step1 - Basic Information
    [HttpPut("step1")]
    public async Task<IActionResult> SaveStep1([FromBody] PatientBasicInfoDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            Patient? patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == dto.PatientId && !p.IsDeleted);
            
            if (patient == null)
            {
                return NotFound(new { message = "Patient not found", traceId = HttpContext.TraceIdentifier });
            }

            // Update basic patient information
            if (!string.IsNullOrEmpty(dto.FirstName)) patient.FirstName = dto.FirstName;
            if (!string.IsNullOrEmpty(dto.LastName)) patient.LastName = dto.LastName;
            if (dto.MiddleName != null) patient.MiddleName = dto.MiddleName;
            if (dto.Suffix != null) patient.Suffix = dto.Suffix;
            if (dto.DateOfBirth.HasValue) patient.DateOfBirth = dto.DateOfBirth;
            if (dto.GenderId.HasValue) patient.GenderId = dto.GenderId;
            if (!string.IsNullOrEmpty(dto.SsnEncrypted)) patient.SsnEncrypted = dto.SsnEncrypted;
            if (dto.MaritalStatusId.HasValue) patient.MaritalStatusId = dto.MaritalStatusId;
            if (dto.RaceId.HasValue) patient.RaceId = dto.RaceId;
            if (!string.IsNullOrEmpty(dto.Ethnicity)) patient.Ethnicity = dto.Ethnicity;
            if (dto.PrimaryLanguageId.HasValue) patient.PrimaryLanguageId = dto.PrimaryLanguageId;
            if (dto.PreferredLanguageId.HasValue) patient.PreferredLanguageId = dto.PreferredLanguageId;
            if (dto.CommunicationPrefId.HasValue) patient.CommunicationPrefId = dto.CommunicationPrefId;
            if (dto.InterpreterRequired.HasValue) patient.InterpreterRequired = dto.InterpreterRequired.Value;
            if (dto.MobilityAssistance.HasValue) patient.MobilityAssistance = dto.MobilityAssistance.Value;
            if (!string.IsNullOrEmpty(dto.ReligiousConsiderations)) patient.ReligiousConsiderations = dto.ReligiousConsiderations;
            if (!string.IsNullOrEmpty(dto.PrimaryCarePhysician)) patient.PrimaryCarePhysician = dto.PrimaryCarePhysician;
            if (!string.IsNullOrEmpty(dto.PcpPhoneNumber)) patient.PCPPhoneNumber = dto.PcpPhoneNumber;
            if (!string.IsNullOrEmpty(dto.ReferringPhysician)) patient.ReferringPhysician = dto.ReferringPhysician;
            if (!string.IsNullOrEmpty(dto.PreferredPharmacyName)) patient.PreferredPharmacyName = dto.PreferredPharmacyName;
            if (!string.IsNullOrEmpty(dto.PreferredPharmacyLocation)) patient.PreferredPharmacyLocation = dto.PreferredPharmacyLocation;
            if (!string.IsNullOrEmpty(dto.PhotoIdPath)) patient.PhotoIdPath = dto.PhotoIdPath;
            if (!string.IsNullOrEmpty(dto.ElectronicSignature)) patient.ElectronicSignature = dto.ElectronicSignature;
            if (dto.SignedDate.HasValue) patient.SignedDate = dto.SignedDate;

            patient.UpdatedAt = DateTime.UtcNow;
            patient.UpdatedBy = GetCurrentUserId();

            await _context.SaveChangesAsync();
            await LogAudit(patient.PatientId, "Update", "PatientBasicInfo", "Updated basic information", JsonSerializer.Serialize(dto));

            return Ok(new { message = "Step 1 saved successfully", patientId = patient.PatientId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving step 1 for patient {PatientId}", dto.PatientId);
            return StatusCode(500, new { message = "An error occurred while saving step 1", traceId = HttpContext.TraceIdentifier });
        }
    }

    // PUT api/patients/step2 - Contact Information
    [HttpPut("step2")]
    public async Task<IActionResult> SaveStep2([FromBody] PatientContactInfoDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            Patient? patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == dto.PatientId && !p.IsDeleted);
            
            if (patient == null)
            {
                return NotFound(new { message = "Patient not found", traceId = HttpContext.TraceIdentifier });
            }

            // Update contact information
            if (!string.IsNullOrEmpty(dto.Email)) patient.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.PhoneNumber)) patient.PhoneNumber = dto.PhoneNumber;
            if (dto.AlternatePhone != null) patient.AlternatePhone = dto.AlternatePhone;
            if (!string.IsNullOrEmpty(dto.EmergencyContactName)) patient.EmergencyContactName = dto.EmergencyContactName;
            if (!string.IsNullOrEmpty(dto.EmergencyContactPhone)) patient.EmergencyContactPhone = dto.EmergencyContactPhone;
            if (!string.IsNullOrEmpty(dto.EmergencyContactRelationship)) patient.EmergencyContactRelationship = dto.EmergencyContactRelationship;

            patient.UpdatedAt = DateTime.UtcNow;
            patient.UpdatedBy = GetCurrentUserId();

            await _context.SaveChangesAsync();
            await LogAudit(patient.PatientId, "Update", "PatientContactInfo", "Updated contact information", JsonSerializer.Serialize(dto));

            return Ok(new { message = "Step 2 saved successfully", patientId = patient.PatientId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving step 2 for patient {PatientId}", dto.PatientId);
            return StatusCode(500, new { message = "An error occurred while saving step 2", traceId = HttpContext.TraceIdentifier });
        }
    }

    // PUT api/patients/step3 - Address Information
    [HttpPut("step3")]
    public async Task<IActionResult> SaveStep3([FromBody] PatientAddressDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            Patient? patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == dto.PatientId && !p.IsDeleted);
            
            if (patient == null)
            {
                return NotFound(new { message = "Patient not found", traceId = HttpContext.TraceIdentifier });
            }

            // Update address information
            if (!string.IsNullOrEmpty(dto.AddressLine1)) patient.AddressLine1 = dto.AddressLine1;
            if (dto.AddressLine2 != null) patient.AddressLine2 = dto.AddressLine2;
            if (!string.IsNullOrEmpty(dto.City)) patient.City = dto.City;
            if (!string.IsNullOrEmpty(dto.State)) patient.State = dto.State;
            if (!string.IsNullOrEmpty(dto.ZipCode)) patient.ZipCode = dto.ZipCode;
            if (!string.IsNullOrEmpty(dto.Country)) patient.Country = dto.Country;

            patient.UpdatedAt = DateTime.UtcNow;
            patient.UpdatedBy = GetCurrentUserId();

            await _context.SaveChangesAsync();
            await LogAudit(patient.PatientId, "Update", "PatientAddress", "Updated address information", JsonSerializer.Serialize(dto));

            return Ok(new { message = "Step 3 saved successfully", patientId = patient.PatientId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving step 3 for patient {PatientId}", dto.PatientId);
            return StatusCode(500, new { message = "An error occurred while saving step 3", traceId = HttpContext.TraceIdentifier });
        }
    }

    // PUT api/patients/step4 - Emergency Contacts
    [HttpPut("step4")]
    public async Task<IActionResult> SaveStep4([FromBody] PatientEmergencyContactsDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            Patient? patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == dto.PatientId && !p.IsDeleted);
            
            if (patient == null)
            {
                return NotFound(new { message = "Patient not found", traceId = HttpContext.TraceIdentifier });
            }

            // Remove existing emergency contacts
            var existingContacts = await _context.EmergencyContacts.Where(ec => ec.PatientId == dto.PatientId).ToListAsync();
            _context.EmergencyContacts.RemoveRange(existingContacts);

            // Add new emergency contacts
            if (dto.EmergencyContacts != null && dto.EmergencyContacts.Any())
            {
                var emergencyContacts = dto.EmergencyContacts.Select(ec => new EmergencyContact
                {
                    PatientId = dto.PatientId,
                    Name = ec.Name,
                    RelationshipTypeId = ec.RelationshipTypeId,
                    Phone = ec.Phone,
                    AltPhone = ec.AltPhone
                }).ToList();

                _context.EmergencyContacts.AddRange(emergencyContacts);
            }

            patient.UpdatedAt = DateTime.UtcNow;
            patient.UpdatedBy = GetCurrentUserId();

            await _context.SaveChangesAsync();
            await LogAudit(patient.PatientId, "Update", "PatientEmergencyContacts", "Updated emergency contacts", JsonSerializer.Serialize(dto));

            return Ok(new { message = "Step 4 saved successfully", patientId = patient.PatientId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving step 4 for patient {PatientId}", dto.PatientId);
            return StatusCode(500, new { message = "An error occurred while saving step 4", traceId = HttpContext.TraceIdentifier });
        }
    }

    // PUT api/patients/step5 - Social History
    [HttpPut("step5")]
    public async Task<IActionResult> SaveStep5([FromBody] PatientSocialHistoryDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            Patient? patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == dto.PatientId && !p.IsDeleted);
            
            if (patient == null)
            {
                return NotFound(new { message = "Patient not found", traceId = HttpContext.TraceIdentifier });
            }

            // Update or create social history
            var socialHistory = await _context.SocialHistories.FirstOrDefaultAsync(sh => sh.PatientId == dto.PatientId);
            
            if (socialHistory == null)
            {
                socialHistory = new SocialHistory
                {
                    PatientId = dto.PatientId
                };
                _context.SocialHistories.Add(socialHistory);
            }

            if (dto.SmokingStatusId.HasValue) socialHistory.SmokingStatusId = dto.SmokingStatusId;
            if (dto.AlcoholUseId.HasValue) socialHistory.AlcoholUseId = dto.AlcoholUseId;
            if (dto.DrugUseId.HasValue) socialHistory.DrugUseId = dto.DrugUseId;
            if (!string.IsNullOrEmpty(dto.Occupation)) socialHistory.Occupation = dto.Occupation;
            if (dto.LivingSituationId.HasValue) socialHistory.LivingSituationId = dto.LivingSituationId;

            patient.UpdatedAt = DateTime.UtcNow;
            patient.UpdatedBy = GetCurrentUserId();

            await _context.SaveChangesAsync();
            await LogAudit(patient.PatientId, "Update", "PatientSocialHistory", "Updated social history", JsonSerializer.Serialize(dto));

            return Ok(new { message = "Step 5 saved successfully", patientId = patient.PatientId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving step 5 for patient {PatientId}", dto.PatientId);
            return StatusCode(500, new { message = "An error occurred while saving step 5", traceId = HttpContext.TraceIdentifier });
        }
    }

    // PUT api/patients/step6 - Allergies
    [HttpPut("step6")]
    public async Task<IActionResult> SaveStep6([FromBody] PatientAllergiesDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            Patient? patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == dto.PatientId && !p.IsDeleted);
            
            if (patient == null)
            {
                return NotFound(new { message = "Patient not found", traceId = HttpContext.TraceIdentifier });
            }

            // Remove existing allergies
            var existingAllergies = await _context.Allergies.Where(a => a.PatientId == dto.PatientId).ToListAsync();
            _context.Allergies.RemoveRange(existingAllergies);

            // Add new allergies
            if (dto.Allergies != null && dto.Allergies.Any())
            {
                var allergies = dto.Allergies.Select(a => new Allergy
                {
                    AllergyId = Guid.NewGuid(),
                    PatientId = dto.PatientId,
                    AllergenName = a.AllergenName,
                    AllergenType = a.AllergenType,
                    Severity = a.Severity,
                    Reaction = a.Reaction,
                    OnsetDate = a.OnsetDate,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = GetCurrentUserId(),
                    IsActive = true
                }).ToList();

                _context.Allergies.AddRange(allergies);
            }

            patient.UpdatedAt = DateTime.UtcNow;
            patient.UpdatedBy = GetCurrentUserId();

            await _context.SaveChangesAsync();
            await LogAudit(patient.PatientId, "Update", "PatientAllergies", "Updated allergies", JsonSerializer.Serialize(dto));

            return Ok(new { message = "Step 6 saved successfully", patientId = patient.PatientId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving step 6 for patient {PatientId}", dto.PatientId);
            return StatusCode(500, new { message = "An error occurred while saving step 6", traceId = HttpContext.TraceIdentifier });
        }
    }

    // PUT api/patients/step7 - Medications
    [HttpPut("step7")]
    public async Task<IActionResult> SaveStep7([FromBody] PatientMedicationsDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            Patient? patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == dto.PatientId && !p.IsDeleted);
            
            if (patient == null)
            {
                return NotFound(new { message = "Patient not found", traceId = HttpContext.TraceIdentifier });
            }

            // Remove existing medications
            var existingMedications = await _context.Medications.Where(m => m.PatientId == dto.PatientId).ToListAsync();
            _context.Medications.RemoveRange(existingMedications);

            // Add new medications
            if (dto.Medications != null && dto.Medications.Any())
            {
                var medications = dto.Medications.Select(m => new Medication
                {
                    MedicationId = Guid.NewGuid(),
                    PatientId = dto.PatientId,
                    MedicationName = m.MedicationName,
                    Dosage = m.Dosage,
                    Frequency = m.Frequency,
                    Prescriber = m.Prescriber,
                    StartDate = m.StartDate,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = GetCurrentUserId(),
                    IsActive = true
                }).ToList();

                _context.Medications.AddRange(medications);
            }

            // Remove existing chronic conditions
            var existingConditions = await _context.ChronicConditions.Where(c => c.PatientId == dto.PatientId).ToListAsync();
            _context.ChronicConditions.RemoveRange(existingConditions);

            // Add new chronic conditions
            if (dto.ChronicConditions != null && dto.ChronicConditions.Any())
            {
                var conditions = dto.ChronicConditions.Select(c => new ChronicCondition
                {
                    ConditionId = Guid.NewGuid(),
                    PatientId = dto.PatientId,
                    ConditionName = c.ConditionName,
                    DiagnosedDate = c.DiagnosedDate,
                    Status = c.Status ?? "Active",
                    Notes = c.Notes,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = GetCurrentUserId(),
                    IsActive = true
                }).ToList();

                _context.ChronicConditions.AddRange(conditions);
            }

            // Remove existing immunizations
            var existingImmunizations = await _context.Immunizations.Where(i => i.PatientId == dto.PatientId).ToListAsync();
            _context.Immunizations.RemoveRange(existingImmunizations);

            // Add new immunizations
            if (dto.Immunizations != null && dto.Immunizations.Any())
            {
                var immunizations = dto.Immunizations.Select(i => new Immunization
                {
                    ImmunizationId = Guid.NewGuid(),
                    PatientId = dto.PatientId,
                    VaccineName = i.VaccineName,
                    AdministeredDate = i.AdministeredDate,
                    DoseNumber = i.DoseNumber,
                    ProviderName = i.Provider, // DTO has Provider property, Entity has ProviderName
                    LotNumber = i.LotNumber,
                    ExpirationDate = i.ExpirationDate,
                    Site = i.Site,
                    Route = i.Route,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = GetCurrentUserId()
                }).ToList();

                _context.Immunizations.AddRange(immunizations);
            }

            patient.UpdatedAt = DateTime.UtcNow;
            patient.UpdatedBy = GetCurrentUserId();

            await _context.SaveChangesAsync();
            await LogAudit(patient.PatientId, "Update", "PatientMedications", "Updated medications, chronic conditions, and immunizations", JsonSerializer.Serialize(dto));

            return Ok(new { message = "Step 7 saved successfully", patientId = patient.PatientId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving step 7 for patient {PatientId}", dto.PatientId);
            return StatusCode(500, new { message = "An error occurred while saving step 7", traceId = HttpContext.TraceIdentifier });
        }
    }

    // PUT api/patients/step8 - Medical History
    [HttpPut("step8")]
    public async Task<IActionResult> SaveStep8([FromBody] PatientMedicalHistoryDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            Patient? patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == dto.PatientId && !p.IsDeleted);
            
            if (patient == null)
            {
                return NotFound(new { message = "Patient not found", traceId = HttpContext.TraceIdentifier });
            }

            // Remove existing surgeries
            var existingSurgeries = await _context.PatientSurgeries.Where(s => s.PatientId == dto.PatientId).ToListAsync();
            _context.PatientSurgeries.RemoveRange(existingSurgeries);

            // Remove existing hospitalizations
            var existingHospitalizations = await _context.PatientHospitalizations.Where(h => h.PatientId == dto.PatientId).ToListAsync();
            _context.PatientHospitalizations.RemoveRange(existingHospitalizations);

            // Remove existing family medical history
            var existingFamilyHistory = await _context.FamilyMedicalHistories.Where(f => f.PatientId == dto.PatientId).ToListAsync();
            _context.FamilyMedicalHistories.RemoveRange(existingFamilyHistory);

            // Add new surgeries
            if (dto.Surgeries != null && dto.Surgeries.Any())
            {
                var surgeries = dto.Surgeries.Select(s => new PatientSurgery
                {
                    PatientId = dto.PatientId,
                    SurgeryType = s.SurgeryType,
                    SurgeryDate = s.SurgeryDate,
                    Notes = s.Notes,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                _context.PatientSurgeries.AddRange(surgeries);
            }

            // Add new hospitalizations
            if (dto.Hospitalizations != null && dto.Hospitalizations.Any())
            {
                var hospitalizations = dto.Hospitalizations.Select(h => new PatientHospitalization
                {
                    PatientId = dto.PatientId,
                    HospitalName = h.HospitalName,
                    Reason = h.Reason,
                    AdmissionDate = h.AdmissionDate,
                    DischargeDate = h.DischargeDate,
                    Notes = h.Notes,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                _context.PatientHospitalizations.AddRange(hospitalizations);
            }

            // Add new family medical history
            if (dto.FamilyMedicalHistory != null && dto.FamilyMedicalHistory.Any())
            {
                var familyHistory = dto.FamilyMedicalHistory.Select(f => new FamilyMedicalHistory
                {
                    PatientId = dto.PatientId,
                    Relative = f.Relative,
                    Condition = f.Condition,
                    Notes = f.Notes,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                _context.FamilyMedicalHistories.AddRange(familyHistory);
            }

            patient.UpdatedAt = DateTime.UtcNow;
            patient.UpdatedBy = GetCurrentUserId();

            await _context.SaveChangesAsync();
            await LogAudit(patient.PatientId, "Update", "PatientMedicalHistory", "Updated medical history", JsonSerializer.Serialize(dto));

            return Ok(new { message = "Step 8 saved successfully", patientId = patient.PatientId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving step 8 for patient {PatientId}", dto.PatientId);
            return StatusCode(500, new { message = "An error occurred while saving step 8", traceId = HttpContext.TraceIdentifier });
        }
    }

    // PUT api/patients/step9 - Insurance
    [HttpPut("step9")]
    public async Task<IActionResult> SaveStep9([FromBody] PatientInsuranceDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            Patient? patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == dto.PatientId && !p.IsDeleted);
            
            if (patient == null)
            {
                return NotFound(new { message = "Patient not found", traceId = HttpContext.TraceIdentifier });
            }

            // Remove existing insurance policies
            var existingPolicies = await _context.InsurancePolicies.Where(ip => ip.PatientId == dto.PatientId).ToListAsync();
            _context.InsurancePolicies.RemoveRange(existingPolicies);

            // Add new insurance policies
            if (dto.InsurancePolicies != null && dto.InsurancePolicies.Any())
            {
                var insurancePolicies = dto.InsurancePolicies.Select(ip => new InsurancePolicy
                {
                    PatientId = dto.PatientId,
                    PolicyNumber = ip.PolicyNumber,
                    GroupNumber = ip.GroupNumber,
                    PolicyHolderName = ip.PolicyHolderName,
                    ProviderPhone = ip.ProviderPhone
                }).ToList();

                _context.InsurancePolicies.AddRange(insurancePolicies);
            }

            patient.UpdatedAt = DateTime.UtcNow;
            patient.UpdatedBy = GetCurrentUserId();

            await _context.SaveChangesAsync();
            await LogAudit(patient.PatientId, "Update", "PatientInsurance", "Updated insurance information", JsonSerializer.Serialize(dto));

            return Ok(new { message = "Step 9 saved successfully", patientId = patient.PatientId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving step 9 for patient {PatientId}", dto.PatientId);
            return StatusCode(500, new { message = "An error occurred while saving step 9", traceId = HttpContext.TraceIdentifier });
        }
    }

    // PUT api/patients/step10 - Legal Consents
    [HttpPut("step10")]
    public async Task<IActionResult> SaveStep10([FromBody] PatientLegalConsentsDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            Patient? patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == dto.PatientId && !p.IsDeleted);
            
            if (patient == null)
            {
                return NotFound(new { message = "Patient not found", traceId = HttpContext.TraceIdentifier });
            }

            // Update or create legal consents
            var legalConsent = await _context.LegalConsents.FirstOrDefaultAsync(lc => lc.PatientId == dto.PatientId);
            
            if (legalConsent == null)
            {
                legalConsent = new LegalConsent
                {
                    PatientId = dto.PatientId,
                    ConsentId = Guid.NewGuid()
                };
                _context.LegalConsents.Add(legalConsent);
            }

            if (dto.HipaaAgreed.HasValue) legalConsent.HipaaAgreed = dto.HipaaAgreed.Value;
            if (dto.ConsentToTreat.HasValue) legalConsent.ConsentToTreat = dto.ConsentToTreat.Value;
            if (dto.AdvanceDirectives.HasValue) legalConsent.AdvanceDirectives = dto.AdvanceDirectives.Value;
            if (!string.IsNullOrEmpty(dto.AdvanceDirectivesPath)) legalConsent.AdvanceDirectivesPath = dto.AdvanceDirectivesPath;
            if (dto.AssignmentOfBenefits.HasValue) legalConsent.AssignmentOfBenefits = dto.AssignmentOfBenefits.Value;
            if (dto.FinancialResponsibility.HasValue) legalConsent.FinancialResponsibility = dto.FinancialResponsibility.Value;
            if (dto.SignedOnUtc.HasValue) legalConsent.SignedOnUtc = dto.SignedOnUtc;
            if (!string.IsNullOrEmpty(dto.SignaturePath)) legalConsent.SignaturePath = dto.SignaturePath;

            patient.UpdatedAt = DateTime.UtcNow;
            patient.UpdatedBy = GetCurrentUserId();

            await _context.SaveChangesAsync();
            await LogAudit(patient.PatientId, "Update", "PatientLegalConsents", "Updated legal consents", JsonSerializer.Serialize(dto));

            return Ok(new { message = "Step 10 saved successfully", patientId = patient.PatientId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving step 10 for patient {PatientId}", dto.PatientId);
            return StatusCode(500, new { message = "An error occurred while saving step 10", traceId = HttpContext.TraceIdentifier });
        }
    }

    // PUT api/patients/intake
    [HttpPut("intake")]
    public async Task<IActionResult> UpsertPatientIntake([FromBody] PatientIntakeDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Check if patient exists
        Patient? patient;
        Guid patientId = dto.Patient.Id;
        bool isNew = patientId == Guid.Empty;

        if (isNew)
        {
            // Create new patient
            patient = _mapper.Map<Patient>(dto.Patient);
            patient.PatientId = Guid.NewGuid();
            patient.CreatedAt = DateTime.UtcNow;
            patient.CreatedBy = GetCurrentUserId();
            patient.IsActive = true;
            patient.IsDeleted = false;
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            patientId = patient.PatientId;
        }
        else
        {
            // Update existing patient
            patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == patientId && !p.IsDeleted);
            
            if (patient == null)
            {
                return NotFound(new { message = "Patient not found", traceId = HttpContext.TraceIdentifier });
            }

            // Update only provided fields
            if (!string.IsNullOrEmpty(dto.Patient.FirstName)) patient.FirstName = dto.Patient.FirstName;
            if (!string.IsNullOrEmpty(dto.Patient.LastName)) patient.LastName = dto.Patient.LastName;
            if (dto.Patient.MiddleName != null) patient.MiddleName = dto.Patient.MiddleName;
            if (dto.Patient.Suffix != null) patient.Suffix = dto.Patient.Suffix;
            if (dto.Patient.DateOfBirth.HasValue) patient.DateOfBirth = dto.Patient.DateOfBirth;
            if (dto.Patient.GenderId.HasValue) patient.GenderId = dto.Patient.GenderId;
            if (!string.IsNullOrEmpty(dto.Patient.SsnEncrypted)) patient.SsnEncrypted = dto.Patient.SsnEncrypted;
            if (dto.Patient.MaritalStatusId.HasValue) patient.MaritalStatusId = dto.Patient.MaritalStatusId;
            if (dto.Patient.RaceId.HasValue) patient.RaceId = dto.Patient.RaceId;
            if (dto.Patient.PrimaryLanguageId.HasValue) patient.PrimaryLanguageId = dto.Patient.PrimaryLanguageId;
            if (dto.Patient.PreferredLanguageId.HasValue) patient.PreferredLanguageId = dto.Patient.PreferredLanguageId;
            if (!string.IsNullOrEmpty(dto.Patient.Email)) patient.Email = dto.Patient.Email;
            
            // Accessibility
            patient.InterpreterRequired = dto.Patient.InterpreterRequired;
            patient.MobilityAssistance = dto.Patient.MobilityAssistance;
            if (dto.Patient.CommunicationPrefId.HasValue) patient.CommunicationPrefId = dto.Patient.CommunicationPrefId;
            if (dto.Patient.ReligiousConsiderations != null) patient.ReligiousConsiderations = dto.Patient.ReligiousConsiderations;
            
            // Provider Info
            if (dto.Patient.PrimaryCarePhysician != null) patient.PrimaryCarePhysician = dto.Patient.PrimaryCarePhysician;
            if (dto.Patient.PCPPhoneNumber != null) patient.PCPPhoneNumber = dto.Patient.PCPPhoneNumber;
            if (dto.Patient.ReferringPhysician != null) patient.ReferringPhysician = dto.Patient.ReferringPhysician;
            if (dto.Patient.PreferredPharmacyName != null) patient.PreferredPharmacyName = dto.Patient.PreferredPharmacyName;
            if (dto.Patient.PreferredPharmacyLocation != null) patient.PreferredPharmacyLocation = dto.Patient.PreferredPharmacyLocation;
            
            // Administrative
            if (dto.Patient.PhotoIdPath != null) patient.PhotoIdPath = dto.Patient.PhotoIdPath;
            if (dto.Patient.ElectronicSignature != null) patient.ElectronicSignature = dto.Patient.ElectronicSignature;

            patient.UpdatedAt = DateTime.UtcNow;
            patient.UpdatedBy = GetCurrentUserId();
            
            await _context.SaveChangesAsync();
        }

        // Only update collections if data is actually provided (not just empty arrays)
        // Addresses - only if array has items
        if (dto.Addresses?.Any() == true)
        {
            var existingAddresses = await _context.PatientAddresses.Where(e => e.PatientId == patientId).ToListAsync();
            _context.PatientAddresses.RemoveRange(existingAddresses);
            
            var addresses = dto.Addresses.Select(a => { var e = _mapper.Map<PatientAddress>(a); e.PatientId = patientId; return e; });
            await _context.PatientAddresses.AddRangeAsync(addresses);
        }

        // Phones - only if array has items
        if (dto.Phones?.Any() == true)
        {
            var existingPhones = await _context.PatientPhones.Where(e => e.PatientId == patientId).ToListAsync();
            _context.PatientPhones.RemoveRange(existingPhones);
            
            var phones = dto.Phones.Select(p => { var e = _mapper.Map<PatientPhone>(p); e.PatientId = patientId; return e; });
            await _context.PatientPhones.AddRangeAsync(phones);
        }

        // Emergency Contacts - only if array has items
        if (dto.EmergencyContacts?.Any() == true)
        {
            var existingECs = await _context.EmergencyContacts.Where(e => e.PatientId == patientId).ToListAsync();
            _context.EmergencyContacts.RemoveRange(existingECs);
            
            var ecs = dto.EmergencyContacts.Select(c => { var e = _mapper.Map<EmergencyContact>(c); e.PatientId = patientId; return e; });
            await _context.EmergencyContacts.AddRangeAsync(ecs);
        }

        // Insurance Policies - only if array has items
        if (dto.InsurancePolicies?.Any() == true)
        {
            var existingIPs = await _context.InsurancePolicies.Where(e => e.PatientId == patientId).ToListAsync();
            _context.InsurancePolicies.RemoveRange(existingIPs);
            
            var ins = dto.InsurancePolicies.Select(i => { var e = _mapper.Map<InsurancePolicy>(i); e.PatientId = patientId; return e; });
            await _context.InsurancePolicies.AddRangeAsync(ins);
        }

        // Social History (1-1)
        if (dto.SocialHistory is not null)
        {
            var existingSH = await _context.SocialHistories.FirstOrDefaultAsync(sh => sh.PatientId == patientId);
            if (existingSH != null)
            {
                // Update existing
                if (dto.SocialHistory.SmokingStatusId.HasValue) existingSH.SmokingStatusId = dto.SocialHistory.SmokingStatusId;
                if (dto.SocialHistory.AlcoholUseId.HasValue) existingSH.AlcoholUseId = dto.SocialHistory.AlcoholUseId;
                if (dto.SocialHistory.DrugUseId.HasValue) existingSH.DrugUseId = dto.SocialHistory.DrugUseId;
                if (dto.SocialHistory.Occupation != null) existingSH.Occupation = dto.SocialHistory.Occupation;
                if (dto.SocialHistory.LivingSituationId.HasValue) existingSH.LivingSituationId = dto.SocialHistory.LivingSituationId;
            }
            else
            {
                // Create new
                var sh = _mapper.Map<SocialHistory>(dto.SocialHistory);
                sh.PatientId = patientId;
                await _context.SocialHistories.AddAsync(sh);
            }
        }

        // Legal Consents (1-1)
        if (dto.LegalConsents is not null)
        {
            var existingLC = await _context.LegalConsents.FirstOrDefaultAsync(lc => lc.PatientId == patientId);
            if (existingLC != null)
            {
                // Update existing
                existingLC.HipaaAgreed = dto.LegalConsents.HipaaAgreed;
                existingLC.ConsentToTreat = dto.LegalConsents.ConsentToTreat;
                existingLC.AdvanceDirectives = dto.LegalConsents.AdvanceDirectives;
                if (dto.LegalConsents.AdvanceDirectivesPath != null) existingLC.AdvanceDirectivesPath = dto.LegalConsents.AdvanceDirectivesPath;
                existingLC.AssignmentOfBenefits = dto.LegalConsents.AssignmentOfBenefits;
                existingLC.FinancialResponsibility = dto.LegalConsents.FinancialResponsibility;
                if (dto.LegalConsents.SignedOnUtc.HasValue) existingLC.SignedOnUtc = dto.LegalConsents.SignedOnUtc;
                if (dto.LegalConsents.SignaturePath != null) existingLC.SignaturePath = dto.LegalConsents.SignaturePath;
            }
            else
            {
                // Create new
                var lc = _mapper.Map<LegalConsent>(dto.LegalConsents);
                lc.PatientId = patientId;
                lc.ConsentId = Guid.NewGuid();
                await _context.LegalConsents.AddAsync(lc);
            }
        }

        // Allergies
        if (dto.Allergies?.Any() == true)
        {
            var existingAllergies = await _context.Allergies.Where(e => e.PatientId == patientId).ToListAsync();
            _context.Allergies.RemoveRange(existingAllergies);
            
            var allergies = dto.Allergies.Select(a => {
                var allergy = _mapper.Map<Allergy>(a);
                allergy.PatientId = patientId;
                allergy.CreatedAt = DateTime.UtcNow;
                allergy.CreatedBy = GetCurrentUserId();
                return allergy;
            });
            await _context.Allergies.AddRangeAsync(allergies);
        }

        // Medications
        if (dto.Medications?.Any() == true)
        {
            var existingMedications = await _context.Medications.Where(e => e.PatientId == patientId).ToListAsync();
            _context.Medications.RemoveRange(existingMedications);
            
            var medications = dto.Medications.Select(m => {
                var medication = _mapper.Map<Medication>(m);
                medication.PatientId = patientId;
                medication.CreatedAt = DateTime.UtcNow;
                medication.CreatedBy = GetCurrentUserId();
                return medication;
            });
            await _context.Medications.AddRangeAsync(medications);
        }

        // Surgeries
        if (dto.Surgeries?.Any() == true)
        {
            var existingSurgeries = await _context.PatientSurgeries.Where(e => e.PatientId == patientId).ToListAsync();
            _context.PatientSurgeries.RemoveRange(existingSurgeries);
            
            var surgeries = dto.Surgeries.Select(s => {
                var surgery = _mapper.Map<PatientSurgery>(s);
                surgery.PatientId = patientId;
                surgery.CreatedAt = DateTime.UtcNow;
                return surgery;
            });
            await _context.PatientSurgeries.AddRangeAsync(surgeries);
        }

        // Hospitalizations
        if (dto.Hospitalizations?.Any() == true)
        {
            var existingHospitalizations = await _context.PatientHospitalizations.Where(e => e.PatientId == patientId).ToListAsync();
            _context.PatientHospitalizations.RemoveRange(existingHospitalizations);
            
            var hospitalizations = dto.Hospitalizations.Select(h => {
                var hospitalization = _mapper.Map<PatientHospitalization>(h);
                hospitalization.PatientId = patientId;
                hospitalization.CreatedAt = DateTime.UtcNow;
                return hospitalization;
            });
            await _context.PatientHospitalizations.AddRangeAsync(hospitalizations);
        }

        // Family Medical History
        if (dto.FamilyMedicalHistory?.Any() == true)
        {
            var existingFamilyHistory = await _context.FamilyMedicalHistories.Where(e => e.PatientId == patientId).ToListAsync();
            _context.FamilyMedicalHistories.RemoveRange(existingFamilyHistory);
            
            var familyHistories = dto.FamilyMedicalHistory.Select(f => {
                var familyHistory = _mapper.Map<FamilyMedicalHistory>(f);
                familyHistory.PatientId = patientId;
                familyHistory.CreatedAt = DateTime.UtcNow;
                return familyHistory;
            });
            await _context.FamilyMedicalHistories.AddRangeAsync(familyHistories);
        }

        // Chronic Conditions (many-to-many)
        if (dto.ChronicConditionIds?.Any() == true)
        {
            var existingConditions = await _context.PatientChronicConditions.Where(e => e.PatientId == patientId).ToListAsync();
            _context.PatientChronicConditions.RemoveRange(existingConditions);
            
            var chronicConditions = dto.ChronicConditionIds.Select(conditionId => new PatientChronicCondition
            {
                PatientId = patientId,
                ConditionId = conditionId,
                CreatedAt = DateTime.UtcNow
            });
            await _context.PatientChronicConditions.AddRangeAsync(chronicConditions);
        }

        await _context.SaveChangesAsync();
        
        await LogAudit(patientId, "Update", "Patient", "Comprehensive patient intake completed");
        
        return Ok(new { patientId, message = "Patient intake saved successfully" });
    }

    // GET api/patients/{id}/completeness
    [HttpGet("{id}/completeness")]
    [ProducesResponseType(typeof(ProfileCompletenessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfileCompletenessDto>> GetProfileCompleteness(Guid id)
    {
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.PatientId == id && !p.IsDeleted);

        if (patient == null)
        {
            return NotFound(new { message = "Patient not found" });
        }

        // Check allergies
        var hasAllergies = await _context.Allergies.AnyAsync(a => a.PatientId == id);
        
        // Check insurance
        var hasInsurance = await _context.Insurances.AnyAsync(i => i.PatientId == id);
        
        // Check emergency contact
        var hasEmergencyContact = !string.IsNullOrEmpty(patient.EmergencyContactName) && 
                                 !string.IsNullOrEmpty(patient.EmergencyContactPhone);

        var completeness = new ProfileCompletenessDto
        {
            PatientId = id,
            IsComplete = !string.IsNullOrEmpty(patient.PhoneNumber) &&
                        !string.IsNullOrEmpty(patient.Email) &&
                        hasEmergencyContact &&
                        hasAllergies &&
                        hasInsurance,
            HasBasicInfo = !string.IsNullOrEmpty(patient.FirstName) && 
                          !string.IsNullOrEmpty(patient.LastName) &&
                          !string.IsNullOrEmpty(patient.Email),
            HasContactInfo = !string.IsNullOrEmpty(patient.PhoneNumber) && 
                            !string.IsNullOrEmpty(patient.AddressLine1),
            HasEmergencyContact = hasEmergencyContact,
            HasInsurance = hasInsurance,
            HasAllergies = hasAllergies,
            MissingFields = new List<string>()
        };

        // Build list of missing fields
        if (string.IsNullOrEmpty(patient.PhoneNumber)) completeness.MissingFields.Add("Phone Number");
        if (string.IsNullOrEmpty(patient.Email)) completeness.MissingFields.Add("Email");
        if (!hasEmergencyContact) completeness.MissingFields.Add("Emergency Contact");
        if (!hasAllergies) completeness.MissingFields.Add("Allergies");
        if (!hasInsurance) completeness.MissingFields.Add("Insurance");

        return Ok(completeness);
    }

    // PUT api/patients/{id}/personal
    [HttpPut("{id}/personal")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePersonalInfo(Guid id, [FromBody] PersonalInfoUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == id && !p.IsDeleted);
        if (patient == null) return NotFound(new { message = "Patient not found" });

        // Update only provided fields
        if (!string.IsNullOrEmpty(dto.FirstName)) patient.FirstName = dto.FirstName;
        if (!string.IsNullOrEmpty(dto.LastName)) patient.LastName = dto.LastName;
        if (dto.MiddleName != null) patient.MiddleName = dto.MiddleName;
        if (dto.Suffix != null) patient.Suffix = dto.Suffix;
        if (dto.DateOfBirth.HasValue) patient.DateOfBirth = dto.DateOfBirth.Value;
        if (!string.IsNullOrEmpty(dto.Gender)) patient.Gender = dto.Gender;
        if (dto.MaritalStatus != null) patient.MaritalStatus = dto.MaritalStatus;
        if (dto.Race != null) patient.Race = dto.Race;
        if (dto.Ethnicity != null) patient.Ethnicity = dto.Ethnicity;
        if (dto.PreferredLanguage != null) patient.PreferredLanguage = dto.PreferredLanguage;

        patient.UpdatedAt = DateTime.UtcNow;
        patient.UpdatedBy = GetCurrentUserId();

        await _context.SaveChangesAsync();
        await LogAudit(id, "Update", "Patient", "Personal information updated");

        return Ok(new { message = "Personal information updated successfully" });
    }

    // PUT api/patients/{id}/contact
    [HttpPut("{id}/contact")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateContactInfo(Guid id, [FromBody] ContactInfoUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == id && !p.IsDeleted);
        if (patient == null) return NotFound(new { message = "Patient not found" });

        // Update contact information
        if (!string.IsNullOrEmpty(dto.PhoneNumber)) patient.PhoneNumber = dto.PhoneNumber;
        if (dto.AlternatePhone != null) patient.AlternatePhone = dto.AlternatePhone;
        if (!string.IsNullOrEmpty(dto.Email)) patient.Email = dto.Email;
        if (!string.IsNullOrEmpty(dto.AddressLine1)) patient.AddressLine1 = dto.AddressLine1;
        if (dto.AddressLine2 != null) patient.AddressLine2 = dto.AddressLine2;
        if (!string.IsNullOrEmpty(dto.City)) patient.City = dto.City;
        if (!string.IsNullOrEmpty(dto.State)) patient.State = dto.State;
        if (!string.IsNullOrEmpty(dto.ZipCode)) patient.ZipCode = dto.ZipCode;
        if (!string.IsNullOrEmpty(dto.Country)) patient.Country = dto.Country;

        patient.UpdatedAt = DateTime.UtcNow;
        patient.UpdatedBy = GetCurrentUserId();

        await _context.SaveChangesAsync();
        await LogAudit(id, "Update", "Patient", "Contact information updated");

        return Ok(new { message = "Contact information updated successfully" });
    }

    // PUT api/patients/{id}/emergency-contact
    [HttpPut("{id}/emergency-contact")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateEmergencyContact(Guid id, [FromBody] EmergencyContactUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == id && !p.IsDeleted);
        if (patient == null) return NotFound(new { message = "Patient not found" });

        // Update emergency contact
        if (!string.IsNullOrEmpty(dto.Name)) patient.EmergencyContactName = dto.Name;
        if (!string.IsNullOrEmpty(dto.Phone)) patient.EmergencyContactPhone = dto.Phone;
        if (!string.IsNullOrEmpty(dto.Relationship)) patient.EmergencyContactRelationship = dto.Relationship;

        patient.UpdatedAt = DateTime.UtcNow;
        patient.UpdatedBy = GetCurrentUserId();

        await _context.SaveChangesAsync();
        await LogAudit(id, "Update", "Patient", "Emergency contact updated");

        return Ok(new { message = "Emergency contact updated successfully" });
    }

    // PUT api/patients/{id}/provider-info
    [HttpPut("{id}/provider-info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProviderInfo(Guid id, [FromBody] ProviderInfoUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == id && !p.IsDeleted);
        if (patient == null) return NotFound(new { message = "Patient not found" });

        // Update provider and pharmacy information
        if (dto.PrimaryCarePhysician != null) patient.PrimaryCarePhysician = dto.PrimaryCarePhysician;
        if (dto.PCPPhoneNumber != null) patient.PCPPhoneNumber = dto.PCPPhoneNumber;
        if (dto.ReferringPhysician != null) patient.ReferringPhysician = dto.ReferringPhysician;
        if (dto.PreferredPharmacyName != null) patient.PreferredPharmacyName = dto.PreferredPharmacyName;
        if (dto.PreferredPharmacyLocation != null) patient.PreferredPharmacyLocation = dto.PreferredPharmacyLocation;

        patient.UpdatedAt = DateTime.UtcNow;
        patient.UpdatedBy = GetCurrentUserId();

        await _context.SaveChangesAsync();
        await LogAudit(id, "Update", "Patient", "Provider/Pharmacy information updated");

        return Ok(new { message = "Provider/Pharmacy information updated successfully" });
    }

    // PUT api/patients/{id}/accessibility
    [HttpPut("{id}/accessibility")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAccessibilityPreferences(Guid id, [FromBody] AccessibilityUpdateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == id && !p.IsDeleted);
        if (patient == null) return NotFound(new { message = "Patient not found" });

        // Update accessibility preferences
        if (dto.InterpreterRequired.HasValue) patient.InterpreterRequired = dto.InterpreterRequired.Value;
        if (dto.MobilityAssistance.HasValue) patient.MobilityAssistance = dto.MobilityAssistance.Value;
        if (dto.ReligiousConsiderations != null) patient.ReligiousConsiderations = dto.ReligiousConsiderations;
        if (dto.CommunicationPrefId.HasValue) patient.CommunicationPrefId = dto.CommunicationPrefId.Value;

        patient.UpdatedAt = DateTime.UtcNow;
        patient.UpdatedBy = GetCurrentUserId();

        await _context.SaveChangesAsync();
        await LogAudit(id, "Update", "Patient", "Accessibility preferences updated");

        return Ok(new { message = "Accessibility preferences updated successfully" });
    }

    #region Helper Methods

    private async Task<PatientDto> MapToDto(Patient patient)
    {
        return new PatientDto
        {
            PatientId = patient.PatientId,
            FirstName = patient.FirstName,
            MiddleName = patient.MiddleName,
            LastName = patient.LastName,
            DateOfBirth = patient.DateOfBirth,
            Gender = patient.Gender,
            Email = patient.Email,
            PhoneNumber = patient.PhoneNumber,
            AlternatePhone = patient.AlternatePhone,
            AddressLine1 = patient.AddressLine1,
            AddressLine2 = patient.AddressLine2,
            City = patient.City,
            State = patient.State,
            ZipCode = patient.ZipCode,
            Country = patient.Country,
            Race = patient.Race,
            Ethnicity = patient.Ethnicity,
            PreferredLanguage = patient.PreferredLanguage,
            MaritalStatus = patient.MaritalStatus,
            EmergencyContactName = patient.EmergencyContactName,
            EmergencyContactPhone = patient.EmergencyContactPhone,
            EmergencyContactRelationship = patient.EmergencyContactRelationship,
            HasSsn = !string.IsNullOrEmpty(patient.SsnEncrypted)
        };
    }

    private async Task LogAudit(Guid patientId, string eventType, string entityType, string action, string? oldValues = null)
    {
        var audit = new AuditLog
        {
            AuditLogId = Guid.NewGuid(),
            PatientId = patientId,
            EventType = eventType,
            EntityType = entityType,
            EntityId = patientId,
            EventTimestamp = DateTime.UtcNow,
            ActorId = GetCurrentUserId(),
            ActorName = GetCurrentUserName(),
            Action = action,
            OldValues = oldValues,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
            RequestId = HttpContext.TraceIdentifier,
            Success = true,
            PhiAccessed = true,
            DataClassification = "Restricted"
        };

        _context.AuditLogs.Add(audit);
        await _context.SaveChangesAsync();
    }

    private string GetCurrentUserId() => User?.Identity?.Name ?? "system";
    private string GetCurrentUserName() => User?.Identity?.Name ?? "System User";

    #endregion
}

#region DTOs

public class PatientDto
{
    public Guid PatientId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? AlternatePhone { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Race { get; set; }
    public string? Ethnicity { get; set; }
    public string? PreferredLanguage { get; set; }
    public string? MaritalStatus { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmergencyContactRelationship { get; set; }
    public bool HasSsn { get; set; }
}

public class CreatePatientRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? Ssn { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? AlternatePhone { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string? Country { get; set; }
    public string? Race { get; set; }
    public string? Ethnicity { get; set; }
    public string? PreferredLanguage { get; set; }
    public string? MaritalStatus { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmergencyContactRelationship { get; set; }
}

public class UpdatePatientRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? AlternatePhone { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string? Race { get; set; }
    public string? Ethnicity { get; set; }
    public string? PreferredLanguage { get; set; }
    public string? MaritalStatus { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmergencyContactRelationship { get; set; }
}

public class ProfileCompletenessDto
{
    public Guid PatientId { get; set; }
    public bool IsComplete { get; set; }
    public bool HasBasicInfo { get; set; }
    public bool HasContactInfo { get; set; }
    public bool HasEmergencyContact { get; set; }
    public bool HasInsurance { get; set; }
    public bool HasAllergies { get; set; }
    public List<string> MissingFields { get; set; } = new();
}

public class PersonalInfoUpdateDto
{
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? Suffix { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? MaritalStatus { get; set; }
    public string? Race { get; set; }
    public string? Ethnicity { get; set; }
    public string? PreferredLanguage { get; set; }
}

public class ContactInfoUpdateDto
{
    public string? PhoneNumber { get; set; }
    public string? AlternatePhone { get; set; }
    public string? Email { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
}

public class EmergencyContactUpdateDto
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Relationship { get; set; }
}

public class ProviderInfoUpdateDto
{
    public string? PrimaryCarePhysician { get; set; }
    public string? PCPPhoneNumber { get; set; }
    public string? ReferringPhysician { get; set; }
    public string? PreferredPharmacyName { get; set; }
    public string? PreferredPharmacyLocation { get; set; }
}

public class AccessibilityUpdateDto
{
    public bool? InterpreterRequired { get; set; }
    public bool? MobilityAssistance { get; set; }
    public string? ReligiousConsiderations { get; set; }
    public int? CommunicationPrefId { get; set; }
}

#endregion

