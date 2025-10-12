using Shared.Common.Models;
using System;

namespace FamilyService.Domain.Entities
{
    /// <summary>
    /// Emergency contact information for patients
    /// All contact info is encrypted at rest
    /// </summary>
    public class EmergencyContact : BaseEntity
    {
        public Guid PatientId { get; set; }
        
        // Contact Information (Encrypted)
        public string ContactName { get; set; } = string.Empty;
        public string? Relationship { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string? AlternatePhone { get; set; }
        public string? Email { get; set; }
        
        // Address
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        
        // Priority
        public int ContactPriority { get; set; } = 1; // 1=Primary, 2=Secondary, etc.
        
        // Permissions
        public bool CanReceiveMedicalInfo { get; set; } = true;
        public bool CanMakeDecisions { get; set; }
        
        // Status
        public bool IsActive { get; set; } = true;
    }
}
