using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService.Domain.Entities;
using PatientService.Infrastructure.Data;

namespace PatientService.API.Controllers;

[ApiController]
[Route("api/family")]
public class FamilyRelationsController : ControllerBase
{
    private readonly PatientDbContext _context;
    private readonly ILogger<FamilyRelationsController> _logger;

    public FamilyRelationsController(PatientDbContext context, ILogger<FamilyRelationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/family/patient/{patientId}
    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<IEnumerable<FamilyMemberDto>>> GetPatientFamilyMembers(Guid patientId)
    {
        try
        {
            var familyMembers = await _context.FamilyRelations
                .Where(f => f.PatientId == patientId && f.IsActive)
                .Include(f => f.RelatedPatient)
                .OrderBy(f => f.RelationType)
                .ThenBy(f => f.RelatedPatient.LastName)
                .ToListAsync();

            var dtos = familyMembers.Select(f => new FamilyMemberDto
            {
                RelationId = f.RelationId,
                PatientId = f.PatientId,
                RelatedPatientId = f.RelatedPatientId,
                RelationType = f.RelationType,
                FirstName = f.RelatedPatient.FirstName,
                LastName = f.RelatedPatient.LastName,
                MiddleName = f.RelatedPatient.MiddleName,
                DateOfBirth = f.RelatedPatient.DateOfBirth ?? DateTime.MinValue,
                Gender = f.RelatedPatient.Gender,
                Email = f.RelatedPatient.Email,
                Phone = f.RelatedPatient.PhoneNumber,
                Address = f.RelatedPatient.AddressLine1,
                City = f.RelatedPatient.City,
                State = f.RelatedPatient.State,
                ZipCode = f.RelatedPatient.ZipCode,
                IsGuardian = f.IsGuardian,
                IsProxy = f.IsProxy,
                CanViewRecords = f.CanViewRecords,
                CanManageRecords = f.CanManageRecords,
                CanGrantConsent = f.CanGrantConsent,
                LegalDocumentType = f.LegalDocumentType,
                LegalDocumentNumber = f.LegalDocumentNumber,
                LegalDocumentExpiryDate = f.LegalDocumentExpiryDate
            }).ToList();

            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving family members for patient {PatientId}", patientId);
            return StatusCode(500, new { message = "An error occurred while retrieving family members" });
        }
    }

    // GET: api/family/{relationId}
    [HttpGet("{relationId}")]
    public async Task<ActionResult<FamilyMemberDto>> GetFamilyMember(Guid relationId)
    {
        try
        {
            var relation = await _context.FamilyRelations
                .Include(f => f.RelatedPatient)
                .FirstOrDefaultAsync(f => f.RelationId == relationId && f.IsActive);

            if (relation == null)
            {
                return NotFound(new { message = "Family member not found" });
            }

            var dto = new FamilyMemberDto
            {
                RelationId = relation.RelationId,
                PatientId = relation.PatientId,
                RelatedPatientId = relation.RelatedPatientId,
                RelationType = relation.RelationType,
                FirstName = relation.RelatedPatient.FirstName,
                LastName = relation.RelatedPatient.LastName,
                MiddleName = relation.RelatedPatient.MiddleName,
                DateOfBirth = relation.RelatedPatient.DateOfBirth ?? DateTime.MinValue,
                Gender = relation.RelatedPatient.Gender,
                Email = relation.RelatedPatient.Email,
                Phone = relation.RelatedPatient.PhoneNumber,
                Address = relation.RelatedPatient.AddressLine1,
                City = relation.RelatedPatient.City,
                State = relation.RelatedPatient.State,
                ZipCode = relation.RelatedPatient.ZipCode,
                IsGuardian = relation.IsGuardian,
                IsProxy = relation.IsProxy,
                CanViewRecords = relation.CanViewRecords,
                CanManageRecords = relation.CanManageRecords,
                CanGrantConsent = relation.CanGrantConsent,
                LegalDocumentType = relation.LegalDocumentType,
                LegalDocumentNumber = relation.LegalDocumentNumber,
                LegalDocumentExpiryDate = relation.LegalDocumentExpiryDate
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving family member {RelationId}", relationId);
            return StatusCode(500, new { message = "An error occurred while retrieving the family member" });
        }
    }

    // POST: api/family
    [HttpPost]
    public async Task<ActionResult<FamilyMemberDto>> CreateFamilyMember([FromBody] CreateFamilyMemberRequest request)
    {
        try
        {
            // Check if related patient already exists by email
            var existingPatient = await _context.Patients
                .FirstOrDefaultAsync(p => p.Email == request.Email && p.IsActive);

            Patient relatedPatient;

            if (existingPatient != null)
            {
                // Use existing patient
                relatedPatient = existingPatient;
            }
            else
            {
                // Create new patient record for family member
                relatedPatient = new Patient
                {
                    PatientId = Guid.NewGuid(),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    MiddleName = request.MiddleName,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender ?? "Unknown",
                    Email = request.Email,
                    PhoneNumber = request.Phone ?? string.Empty,
                    AddressLine1 = request.Address ?? string.Empty,
                    City = request.City ?? string.Empty,
                    State = request.State ?? string.Empty,
                    ZipCode = request.ZipCode ?? string.Empty,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    // Set minimal required field
                    SsnHash = string.Empty
                };

                _context.Patients.Add(relatedPatient);
            }

            // Create family relation
            var familyRelation = new FamilyRelation
            {
                RelationId = Guid.NewGuid(),
                PatientId = request.PatientId,
                RelatedPatientId = relatedPatient.PatientId,
                RelationType = request.RelationType,
                IsGuardian = request.IsGuardian,
                IsProxy = request.IsProxy,
                CanViewRecords = request.CanViewRecords,
                CanManageRecords = request.CanManageRecords,
                CanGrantConsent = request.CanGrantConsent,
                LegalDocumentType = request.LegalDocumentType,
                LegalDocumentNumber = request.LegalDocumentNumber,
                LegalDocumentExpiryDate = request.LegalDocumentExpiryDate,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System", // TODO: Get from auth context
                IsActive = true
            };

            _context.FamilyRelations.Add(familyRelation);
            await _context.SaveChangesAsync();

            var dto = new FamilyMemberDto
            {
                RelationId = familyRelation.RelationId,
                PatientId = familyRelation.PatientId,
                RelatedPatientId = relatedPatient.PatientId,
                RelationType = familyRelation.RelationType,
                FirstName = relatedPatient.FirstName,
                LastName = relatedPatient.LastName,
                MiddleName = relatedPatient.MiddleName,
                DateOfBirth = relatedPatient.DateOfBirth ?? DateTime.MinValue,
                Gender = relatedPatient.Gender,
                Email = relatedPatient.Email,
                Phone = relatedPatient.PhoneNumber,
                Address = relatedPatient.AddressLine1,
                City = relatedPatient.City,
                State = relatedPatient.State,
                ZipCode = relatedPatient.ZipCode,
                IsGuardian = familyRelation.IsGuardian,
                IsProxy = familyRelation.IsProxy,
                CanViewRecords = familyRelation.CanViewRecords,
                CanManageRecords = familyRelation.CanManageRecords,
                CanGrantConsent = familyRelation.CanGrantConsent,
                LegalDocumentType = familyRelation.LegalDocumentType,
                LegalDocumentNumber = familyRelation.LegalDocumentNumber,
                LegalDocumentExpiryDate = familyRelation.LegalDocumentExpiryDate
            };

            return CreatedAtAction(nameof(GetFamilyMember), new { relationId = familyRelation.RelationId }, dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating family member");
            return StatusCode(500, new { message = "An error occurred while creating the family member" });
        }
    }

    // PUT: api/family/{relationId}
    [HttpPut("{relationId}")]
    public async Task<IActionResult> UpdateFamilyMember(Guid relationId, [FromBody] UpdateFamilyMemberRequest request)
    {
        try
        {
            var relation = await _context.FamilyRelations
                .Include(f => f.RelatedPatient)
                .FirstOrDefaultAsync(f => f.RelationId == relationId && f.IsActive);

            if (relation == null)
            {
                return NotFound(new { message = "Family member not found" });
            }

            // Update relation details
            relation.RelationType = request.RelationType ?? relation.RelationType;
            relation.IsGuardian = request.IsGuardian ?? relation.IsGuardian;
            relation.IsProxy = request.IsProxy ?? relation.IsProxy;
            relation.CanViewRecords = request.CanViewRecords ?? relation.CanViewRecords;
            relation.CanManageRecords = request.CanManageRecords ?? relation.CanManageRecords;
            relation.CanGrantConsent = request.CanGrantConsent ?? relation.CanGrantConsent;
            relation.LegalDocumentType = request.LegalDocumentType ?? relation.LegalDocumentType;
            relation.LegalDocumentNumber = request.LegalDocumentNumber ?? relation.LegalDocumentNumber;
            relation.LegalDocumentExpiryDate = request.LegalDocumentExpiryDate ?? relation.LegalDocumentExpiryDate;
            relation.UpdatedAt = DateTime.UtcNow;

            // Update related patient details if provided
            if (request.FirstName != null) relation.RelatedPatient.FirstName = request.FirstName;
            if (request.LastName != null) relation.RelatedPatient.LastName = request.LastName;
            if (request.MiddleName != null) relation.RelatedPatient.MiddleName = request.MiddleName;
            if (request.DateOfBirth.HasValue) relation.RelatedPatient.DateOfBirth = request.DateOfBirth.Value;
            if (request.Gender != null) relation.RelatedPatient.Gender = request.Gender;
            if (request.Email != null) relation.RelatedPatient.Email = request.Email;
            if (request.Phone != null) relation.RelatedPatient.PhoneNumber = request.Phone;
            if (request.Address != null) relation.RelatedPatient.AddressLine1 = request.Address;
            if (request.City != null) relation.RelatedPatient.City = request.City;
            if (request.State != null) relation.RelatedPatient.State = request.State;
            if (request.ZipCode != null) relation.RelatedPatient.ZipCode = request.ZipCode;

            await _context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating family member {RelationId}", relationId);
            return StatusCode(500, new { message = "An error occurred while updating the family member" });
        }
    }

    // DELETE: api/family/{relationId}
    [HttpDelete("{relationId}")]
    public async Task<IActionResult> DeleteFamilyMember(Guid relationId)
    {
        try
        {
            var relation = await _context.FamilyRelations.FindAsync(relationId);

            if (relation == null)
            {
                return NotFound(new { message = "Family member not found" });
            }

            // Soft delete
            relation.IsActive = false;
            relation.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting family member {RelationId}", relationId);
            return StatusCode(500, new { message = "An error occurred while deleting the family member" });
        }
    }
}

public class FamilyMemberDto
{
    public Guid RelationId { get; set; }
    public Guid PatientId { get; set; }
    public Guid RelatedPatientId { get; set; }
    public string RelationType { get; set; } = string.Empty;
    
    // Related Patient Info
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    
    // Permissions
    public bool IsGuardian { get; set; }
    public bool IsProxy { get; set; }
    public bool CanViewRecords { get; set; }
    public bool CanManageRecords { get; set; }
    public bool CanGrantConsent { get; set; }
    
    // Legal Documentation
    public string? LegalDocumentType { get; set; }
    public string? LegalDocumentNumber { get; set; }
    public DateTime? LegalDocumentExpiryDate { get; set; }
}

public class CreateFamilyMemberRequest
{
    public Guid PatientId { get; set; }
    public string RelationType { get; set; } = string.Empty;
    
    // Related Patient Info
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    
    // Permissions
    public bool IsGuardian { get; set; }
    public bool IsProxy { get; set; }
    public bool CanViewRecords { get; set; }
    public bool CanManageRecords { get; set; }
    public bool CanGrantConsent { get; set; }
    
    // Legal Documentation
    public string? LegalDocumentType { get; set; }
    public string? LegalDocumentNumber { get; set; }
    public DateTime? LegalDocumentExpiryDate { get; set; }
}

public class UpdateFamilyMemberRequest
{
    public string? RelationType { get; set; }
    
    // Related Patient Info
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    
    // Permissions
    public bool? IsGuardian { get; set; }
    public bool? IsProxy { get; set; }
    public bool? CanViewRecords { get; set; }
    public bool? CanManageRecords { get; set; }
    public bool? CanGrantConsent { get; set; }
    
    // Legal Documentation - empty strings will be converted to null
    private string? _legalDocumentType;
    public string? LegalDocumentType 
    { 
        get => _legalDocumentType;
        set => _legalDocumentType = string.IsNullOrWhiteSpace(value) ? null : value;
    }
    
    private string? _legalDocumentNumber;
    public string? LegalDocumentNumber 
    { 
        get => _legalDocumentNumber;
        set => _legalDocumentNumber = string.IsNullOrWhiteSpace(value) ? null : value;
    }
    
    private DateTime? _legalDocumentExpiryDate;
    private string? _legalDocumentExpiryDateString;
    
    [System.Text.Json.Serialization.JsonIgnore]
    public DateTime? LegalDocumentExpiryDate 
    { 
        get => _legalDocumentExpiryDate;
        set => _legalDocumentExpiryDate = value;
    }
    
    // Accept the date as string and convert to DateTime?
    [System.Text.Json.Serialization.JsonPropertyName("legalDocumentExpiryDate")]
    public string? LegalDocumentExpiryDateString
    {
        get => _legalDocumentExpiryDateString;
        set
        {
            _legalDocumentExpiryDateString = value;
            if (string.IsNullOrWhiteSpace(value))
            {
                _legalDocumentExpiryDate = null;
            }
            else if (DateTime.TryParse(value, out var date))
            {
                _legalDocumentExpiryDate = date;
            }
            else
            {
                _legalDocumentExpiryDate = null;
            }
        }
    }
}

