using Shared.Common.Models;
using System;
using System.Collections.Generic;

namespace PatientService.Domain.Entities
{
    /// <summary>
    /// Patient entity representing an individual person in the healthcare system
    /// Supports multi-person profiles (self, family members, dependents)
    /// All PHI fields are encrypted at rest using AES-256
    /// </summary>
    public class Patient : BaseEntity
    {
        // Identity Information
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string? PreferredName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty; // Male, Female, Other, Unknown
        public string? BiologicalSex { get; set; }
        
        // Contact Information (Encrypted)
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AlternatePhoneNumber { get; set; }
        
        // Address Information (Encrypted)
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; } = "USA";
        
        // Identification Numbers (Encrypted)
        public string? SSN { get; set; } // Social Security Number
        public string? SSNHash { get; set; } // Hashed SSN for searching
        public string? MedicalRecordNumber { get; set; }
        public string? DriverLicenseNumber { get; set; }
        public string? PassportNumber { get; set; }
        
        // Demographics
        public string? Race { get; set; }
        public string? Ethnicity { get; set; }
        public string? PreferredLanguage { get; set; } = "English";
        public string? MaritalStatus { get; set; }
        
        // Emergency Contact (Encrypted)
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactRelationship { get; set; }
        public string? EmergencyContactPhone { get; set; }
        
        // Account Information
        public Guid? PrimaryAccountHolderId { get; set; } // Reference to the account holder
        public bool IsPrimaryAccountHolder { get; set; }
        public string ProfileType { get; set; } = "Self"; // Self, Spouse, Child, Parent, Sibling, Other
        public string? RelationshipToPrimary { get; set; }
        
        // FHIR Compliance
        public string? FHIRPatientId { get; set; }
        public string? FHIRResourceJson { get; set; } // Cached FHIR Patient resource
        
        // Status and Flags
        public bool IsActive { get; set; } = true;
        public bool IsVerified { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public bool IsMinor { get; set; }
        public DateTime? DeceasedDate { get; set; }
        
        // Privacy and Consent
        public bool HasConsentedToDataSharing { get; set; }
        public DateTime? ConsentDate { get; set; }
        public string? ConsentVersion { get; set; }
        
        // Audit Trail
        public string? LastAccessedBy { get; set; }
        public DateTime? LastAccessedAt { get; set; }
        
        // Photo/Avatar
        public string? ProfilePhotoUrl { get; set; }
        
        // Navigation Properties
        public virtual ICollection<PatientDocument> Documents { get; set; } = new List<PatientDocument>();
        public virtual ICollection<PatientNote> Notes { get; set; } = new List<PatientNote>();
    }
}
