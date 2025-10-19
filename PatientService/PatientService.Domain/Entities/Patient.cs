namespace PatientService.Domain.Entities;

/// <summary>
/// Represents a patient in the healthcare system
/// </summary>
public class Patient
{
    public Guid PatientId { get; set; }
    
    // Personal Information
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? Suffix { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty; // Male, Female, Other, Unknown
    public int? GenderId { get; set; } // Reference to lookup table
    
    // Encrypted sensitive data
    public string? SsnEncrypted { get; set; }
    public string? SsnHash { get; set; } // For duplicate checking without decryption
    
    // Contact Information
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? AlternatePhone { get; set; }
    
    // Address (main fields - detailed addresses in PatientAddresses)
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = "USA";
    
    // Demographics
    public string? Race { get; set; }
    public int? RaceId { get; set; } // Reference to lookup table
    public string? Ethnicity { get; set; }
    public string? PreferredLanguage { get; set; }
    public int? PrimaryLanguageId { get; set; } // Reference to lookup table
    public int? PreferredLanguageId { get; set; } // Reference to lookup table
    public string? MaritalStatus { get; set; }
    public int? MaritalStatusId { get; set; } // Reference to lookup table
    
    // Accessibility & Preferences
    public bool InterpreterRequired { get; set; }
    public bool MobilityAssistance { get; set; }
    public int? CommunicationPrefId { get; set; } // Reference to lookup table
    public string? ReligiousConsiderations { get; set; }
    
    // Provider Information
    public string? PrimaryCarePhysician { get; set; }
    public string? PCPPhoneNumber { get; set; }
    public string? ReferringPhysician { get; set; }
    
    // Pharmacy Information
    public string? PreferredPharmacyName { get; set; }
    public string? PreferredPharmacyLocation { get; set; }
    
    // Administrative
    public string? PhotoIdPath { get; set; }
    
    // Electronic Signature
    public string? ElectronicSignature { get; set; }
    public DateTime? SignedDate { get; set; }
    
    // Legacy Emergency Contact (should use EmergencyContacts table)
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string? EmergencyContactRelationship { get; set; }
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    
    // FHIR Integration
    public string? FhirPatientId { get; set; }
    public DateTime? LastFhirSync { get; set; }
    
    // Navigation Properties
    public virtual ICollection<FamilyRelation> FamilyRelations { get; set; } = new List<FamilyRelation>();
    public virtual ICollection<FamilyRelation> RelatedToPatients { get; set; } = new List<FamilyRelation>();
    public virtual ICollection<Allergy> Allergies { get; set; } = new List<Allergy>();
    public virtual ICollection<Condition> Conditions { get; set; } = new List<Condition>();
    public virtual ICollection<Immunization> Immunizations { get; set; } = new List<Immunization>();
    public virtual ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();
    public virtual ICollection<Consent> Consents { get; set; } = new List<Consent>();
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
    public virtual ICollection<Encounter> Encounters { get; set; } = new List<Encounter>();
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}

