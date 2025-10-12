using System;

namespace ConsentAuditService.Domain.Entities
{
    /// <summary>
    /// Comprehensive HIPAA-compliant audit log
    /// IMMUTABLE - Cannot be modified or deleted
    /// </summary>
    public class AuditLog
    {
        public Guid Id { get; set; }
        
        // Timestamp (immutable)
        public DateTime EventTimestamp { get; set; } = DateTime.UtcNow;
        
        // Event Information
        public string EventType { get; set; } = string.Empty;
        public string EventCategory { get; set; } = string.Empty;
        public string EventAction { get; set; } = string.Empty;
        public string EventOutcome { get; set; } = string.Empty;
        
        // Actor
        public string ActorId { get; set; } = string.Empty;
        public string ActorType { get; set; } = string.Empty;
        public string? ActorName { get; set; }
        public string? ActorRole { get; set; }
        
        // Target
        public string TargetType { get; set; } = string.Empty;
        public Guid? TargetId { get; set; }
        public Guid? PatientId { get; set; }
        
        // Source
        public string? SourceType { get; set; }
        public string? SourceIdentifier { get; set; }
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Location { get; set; }
        public string? DeviceId { get; set; }
        
        // Event Details
        public string? EventDescription { get; set; }
        public string? DataAccessed { get; set; }
        public string? ChangesApplied { get; set; }
        
        // Authorization
        public Guid? ConsentId { get; set; }
        public bool HasValidConsent { get; set; } = true;
        public string? AuthorizationMethod { get; set; }
        
        // Security
        public bool IsAuthorized { get; set; } = true;
        public bool IsAnomaly { get; set; }
        public int? RiskScore { get; set; }
        
        // Compliance
        public string? ComplianceFlags { get; set; }
        public bool RequiresReview { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewedBy { get; set; }
        
        // FHIR
        public string? FHIRResourceId { get; set; }
        public string? FHIRResourceJson { get; set; }
        
        // Correlation
        public Guid? CorrelationId { get; set; }
        public string? SessionId { get; set; }
    }
}
