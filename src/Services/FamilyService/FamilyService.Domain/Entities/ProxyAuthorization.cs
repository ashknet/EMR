using Shared.Common.Models;
using System;

namespace FamilyService.Domain.Entities
{
    /// <summary>
    /// Proxy and guardian authorizations for managing patient data
    /// HIPAA-compliant authorization management
    /// </summary>
    public class ProxyAuthorization : BaseEntity
    {
        public Guid PatientId { get; set; } // Person being represented
        public Guid ProxyPatientId { get; set; } // Person authorized to act
        public string AuthorizationType { get; set; } = string.Empty; // Guardian, PowerOfAttorney, MedicalProxy, Temporary
        
        // Scope of Authorization
        public bool CanViewMedicalRecords { get; set; }
        public bool CanUpdateDemographics { get; set; }
        public bool CanShareRecords { get; set; }
        public bool CanMakeMedicalDecisions { get; set; }
        public bool CanAccessInsurance { get; set; }
        public bool CanManageConsents { get; set; }
        
        // Authorization Period
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public bool IsIndefinite { get; set; }
        
        // Legal Documentation
        public string? LegalDocumentUrl { get; set; }
        public string? DocumentType { get; set; }
        public string? IssuedBy { get; set; }
        public DateTime? IssuedDate { get; set; }
        
        // Status
        public bool IsActive { get; set; } = true;
        public string Status { get; set; } = "Active"; // Active, Expired, Revoked, Pending
        public DateTime? RevokedAt { get; set; }
        public string? RevokedBy { get; set; }
        public string? RevocationReason { get; set; }
    }
}
