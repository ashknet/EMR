using System;
using System.ComponentModel.DataAnnotations;

namespace PatientService.Domain.DTOs
{
    /// <summary>
    /// Data Transfer Object for Patient information
    /// Used for API requests/responses - excludes sensitive fields
    /// </summary>
    public class PatientDto
    {
        public Guid? Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? MiddleName { get; set; }
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? PreferredName { get; set; }
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        public string Gender { get; set; } = string.Empty;
        
        public string? BiologicalSex { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
        
        [Phone]
        public string? PhoneNumber { get; set; }
        
        public string? AlternatePhoneNumber { get; set; }
        
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        
        // Partial SSN (last 4 digits only for display)
        public string? SSNLast4 { get; set; }
        
        public string? MedicalRecordNumber { get; set; }
        public string? Race { get; set; }
        public string? Ethnicity { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? MaritalStatus { get; set; }
        
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactRelationship { get; set; }
        public string? EmergencyContactPhone { get; set; }
        
        public Guid? PrimaryAccountHolderId { get; set; }
        public bool IsPrimaryAccountHolder { get; set; }
        public string ProfileType { get; set; } = "Self";
        public string? RelationshipToPrimary { get; set; }
        
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }
        public bool IsMinor { get; set; }
        
        public bool HasConsentedToDataSharing { get; set; }
        public DateTime? ConsentDate { get; set; }
        
        public string? ProfilePhotoUrl { get; set; }
        
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreatePatientRequest
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        public string? MiddleName { get; set; }
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        public DateTime DateOfBirth { get; set; }
        
        [Required]
        public string Gender { get; set; } = string.Empty;
        
        [EmailAddress]
        public string? Email { get; set; }
        
        [Phone]
        public string? PhoneNumber { get; set; }
        
        public string ProfileType { get; set; } = "Self";
        public Guid? PrimaryAccountHolderId { get; set; }
        public string? RelationshipToPrimary { get; set; }
    }

    public class UpdatePatientRequest
    {
        [Required]
        public Guid Id { get; set; }
        
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? PreferredName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactRelationship { get; set; }
        public string? EmergencyContactPhone { get; set; }
    }
}
