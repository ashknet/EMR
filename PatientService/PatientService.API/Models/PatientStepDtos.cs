using System.ComponentModel.DataAnnotations;

namespace PatientService.API.Models
{
    // Step 1 - Basic Information
    public class PatientBasicInfoDto
    {
        [Required]
        public Guid PatientId { get; set; }
        
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? Suffix { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int? GenderId { get; set; }
        public string? SsnEncrypted { get; set; }
        public int? MaritalStatusId { get; set; }
        public int? RaceId { get; set; }
        public string? Ethnicity { get; set; }
        public int? PrimaryLanguageId { get; set; }
        public int? PreferredLanguageId { get; set; }
        public int? CommunicationPrefId { get; set; }
        public bool? InterpreterRequired { get; set; }
        public bool? MobilityAssistance { get; set; }
        public string? ReligiousConsiderations { get; set; }
        public string? PrimaryCarePhysician { get; set; }
        public string? PcpPhoneNumber { get; set; }
        public string? ReferringPhysician { get; set; }
        public string? PreferredPharmacyName { get; set; }
        public string? PreferredPharmacyLocation { get; set; }
        public string? PhotoIdPath { get; set; }
        public string? ElectronicSignature { get; set; }
        public DateTime? SignedDate { get; set; }
    }

    // Step 2 - Contact Information
    public class PatientContactInfoDto
    {
        [Required]
        public Guid PatientId { get; set; }
        
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AlternatePhone { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? EmergencyContactRelationship { get; set; }
    }

    // Step 3 - Address Information
    public class PatientAddressDto
    {
        [Required]
        public Guid PatientId { get; set; }
        
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
    }

    // Step 4 - Emergency Contacts
    public class PatientEmergencyContactsDto
    {
        [Required]
        public Guid PatientId { get; set; }
        
        public List<EmergencyContactDto>? EmergencyContacts { get; set; }
    }

    // Step 5 - Social History
    public class PatientSocialHistoryDto
    {
        [Required]
        public Guid PatientId { get; set; }
        
        public int? SmokingStatusId { get; set; }
        public int? AlcoholUseId { get; set; }
        public int? DrugUseId { get; set; }
        public string? Occupation { get; set; }
        public int? LivingSituationId { get; set; }
    }

    // Step 6 - Allergies
    public class PatientAllergiesDto
    {
        [Required]
        public Guid PatientId { get; set; }
        
        public List<AllergyRecordDto>? Allergies { get; set; }
    }

    // Step 7 - Medications, Chronic Conditions & Immunizations
    public class PatientMedicationsDto
    {
        [Required]
        public Guid PatientId { get; set; }
        
        public List<MedicationRecordDto>? Medications { get; set; }
        public List<ChronicConditionRecordDto>? ChronicConditions { get; set; }
        public List<ImmunizationRecordDto>? Immunizations { get; set; }
    }

    // Step 8 - Medical History (Complete)
    public class PatientMedicalHistoryDto
    {
        [Required]
        public Guid PatientId { get; set; }
        
        public List<AllergyRecordDto>? Allergies { get; set; }
        public List<MedicationRecordDto>? Medications { get; set; }
        public List<ChronicConditionRecordDto>? ChronicConditions { get; set; }
        public List<ImmunizationRecordDto>? Immunizations { get; set; }
        public List<SurgeryRecordDto>? Surgeries { get; set; }
        public List<HospitalizationRecordDto>? Hospitalizations { get; set; }
        public List<FamilyHistoryRecordDto>? FamilyMedicalHistory { get; set; }
    }

    // Step 9 - Insurance
    public class PatientInsuranceDto
    {
        [Required]
        public Guid PatientId { get; set; }
        
        public List<InsurancePolicyDto>? InsurancePolicies { get; set; }
    }

    // Step 10 - Legal Consents
    public class PatientLegalConsentsDto
    {
        [Required]
        public Guid PatientId { get; set; }
        
        public bool? HipaaAgreed { get; set; }
        public bool? ConsentToTreat { get; set; }
        public bool? AdvanceDirectives { get; set; }
        public string? AdvanceDirectivesPath { get; set; }
        public bool? AssignmentOfBenefits { get; set; }
        public bool? FinancialResponsibility { get; set; }
        public DateTime? SignedOnUtc { get; set; }
        public string? SignaturePath { get; set; }
    }
}
