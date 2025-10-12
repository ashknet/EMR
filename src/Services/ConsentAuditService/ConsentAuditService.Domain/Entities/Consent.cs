using Shared.Common.Models;
using System;

namespace ConsentAuditService.Domain.Entities
{
    /// <summary>
    /// Patient consent entity - maps to FHIR Consent resource
    /// Critical for HIPAA compliance and data sharing authorization
    /// </summary>
    public class Consent : BaseEntity
    {
        public Guid PatientId { get; set; }
        
        // Consent Information
        public string ConsentType { get; set; } = string.Empty;
        public string ConsentScope { get; set; } = string.Empty;
        public string? ConsentCategory { get; set; }
        
        // Status
        public string Status { get; set; } = "active";
        
        // Period
        public DateTime EffectiveDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpirationDate { get; set; }
        public bool IsIndefinite { get; set; }
        
        // Granular Permissions
        public string PolicyRule { get; set; } = "permit";
        public bool CanView { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanShare { get; set; }
        public bool CanExport { get; set; }
        
        // Data Categories
        public string CoverageScope { get; set; } = "all";
        public string? DataCategories { get; set; }
        
        // Actors
        public string? GrantedToType { get; set; }
        public Guid? GrantedToId { get; set; }
        public string? GrantedToName { get; set; }
        public string? GrantedToOrganization { get; set; }
        
        // Purpose
        public string? Purpose { get; set; }
        public string? PurposeDescription { get; set; }
        
        // Signature
        public bool IsSignedElectronically { get; set; }
        public string? SignatureData { get; set; }
        public DateTime? SignedDate { get; set; }
        public string? WitnessName { get; set; }
        
        // Documentation
        public string? ConsentFormUrl { get; set; }
        public string? ConsentFormVersion { get; set; }
        
        // Source
        public string SourceType { get; set; } = "patient";
        public Guid? SourceReferenceId { get; set; }
        
        // QR Code
        public string? QRCodeData { get; set; }
        public DateTime? QRCodeExpiration { get; set; }
        public int QRCodeUsageCount { get; set; }
        public int? QRCodeMaxUsage { get; set; }
        
        // Revocation
        public bool IsRevoked { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? RevokedBy { get; set; }
        public string? RevocationReason { get; set; }
        
        // FHIR Compliance
        public string? FHIRResourceId { get; set; }
        public string? FHIRResourceJson { get; set; }
    }
}
