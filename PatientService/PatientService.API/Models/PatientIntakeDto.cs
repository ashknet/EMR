using System.ComponentModel.DataAnnotations;

namespace PatientService.API.Models;

/// <summary>
/// Comprehensive Patient Intake DTO
/// </summary>
public class PatientIntakeDto
{
    public PatientCoreDto Patient { get; set; } = new();
    public List<AddressDto> Addresses { get; set; } = new();
    public List<PhoneDto> Phones { get; set; } = new();
    public List<EmergencyContactDto> EmergencyContacts { get; set; } = new();
    public List<InsurancePolicyDto> InsurancePolicies { get; set; } = new();
    public SocialHistoryDto? SocialHistory { get; set; }
    public LegalConsentDto? LegalConsents { get; set; }
    public List<AllergyDto> Allergies { get; set; } = new();
    public List<MedicationDto> Medications { get; set; } = new();
    public List<Guid> ChronicConditionIds { get; set; } = new();
    public List<SurgeryDto> Surgeries { get; set; } = new();
    public List<HospitalizationDto> Hospitalizations { get; set; } = new();
    public List<FamilyHistoryDto> FamilyMedicalHistory { get; set; } = new();
}

/// <summary>
/// Core patient demographic information
/// </summary>
public class PatientCoreDto
{
    public Guid Id { get; set; }
    
    // Personal Information
    // Note: Required attributes removed to allow partial updates from individual forms
    // Validation should be done at the UI level for comprehensive intake
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? Suffix { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? GenderId { get; set; }
    public string? SsnEncrypted { get; set; }
    public int? MaritalStatusId { get; set; }
    public int? RaceId { get; set; }
    public int? PrimaryLanguageId { get; set; }
    public int? PreferredLanguageId { get; set; }
    
    // Contact
    [EmailAddress] public string? Email { get; set; }
    
    // Accessibility & Preferences
    public bool InterpreterRequired { get; set; }
    public bool MobilityAssistance { get; set; }
    public int? CommunicationPrefId { get; set; }
    public string? ReligiousConsiderations { get; set; }
    
    // Provider Information
    public string? PrimaryCarePhysician { get; set; }
    public string? PCPPhoneNumber { get; set; }
    public string? ReferringPhysician { get; set; }
    
    // Pharmacy
    public string? PreferredPharmacyName { get; set; }
    public string? PreferredPharmacyLocation { get; set; }
    
    // Administrative
    public string? PhotoIdPath { get; set; }
    
    // Signature
    public string? ElectronicSignature { get; set; }
    public DateTime? SignedDate { get; set; }
}

// Supporting DTOs
public record AddressDto(int AddressTypeId, string Line1, string? Line2, string? City, string? State, string? PostalCode, string? Country);
public record PhoneDto(int PhoneTypeId, string PhoneNumber);
public record EmergencyContactDto(string Name, int RelationshipTypeId, string Phone, string? AltPhone);
public record InsurancePolicyDto(int ProviderId, string? GroupNumber, string? PolicyNumber, string? PolicyHolderName, string? ProviderPhone, string? MedicareMedicaidId, string? CardImagePath);
public record SocialHistoryDto(int? SmokingStatusId, int? AlcoholUseId, int? DrugUseId, string? Occupation, int? LivingSituationId);
public record LegalConsentDto(bool HipaaAgreed, bool ConsentToTreat, bool AdvanceDirectives, string? AdvanceDirectivesPath, bool AssignmentOfBenefits, bool FinancialResponsibility, DateTime? SignedOnUtc, string? SignaturePath);
public record AllergyDto(string AllergenName, string AllergenType, string Severity, string? Reaction, DateTime? OnsetDate);
public record MedicationDto(string MedicationName, string? Dosage, string? Frequency, string? Prescriber, DateTime? StartDate);
public record ChronicConditionDto(string ConditionName, DateTime? DiagnosedDate, string? Status, string? Notes);
public record ImmunizationDto(string VaccineName, DateTime AdministeredDate, int? DoseNumber, string? Provider, string? LotNumber, DateTime? ExpirationDate, string? Site, string? Route, string? Notes);
public record SurgeryDto(string SurgeryType, DateTime? SurgeryDate, string? Notes);
public record HospitalizationDto(string? HospitalName, string? Reason, DateTime? AdmissionDate, DateTime? DischargeDate, string? Notes);
public record FamilyHistoryDto(string Relative, string Condition, string? Notes);
