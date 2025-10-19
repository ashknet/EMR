namespace PatientService.Domain.Entities;

/// <summary>
/// Comprehensive audit logging for HIPAA compliance
/// Tracks all PHI access and modifications
/// </summary>
public class AuditLog
{
    public Guid AuditLogId { get; set; }
    public Guid? PatientId { get; set; }
    
    // Audit Event
    public string EventType { get; set; } = string.Empty; // Create, Read, Update, Delete, Export, Share
    public string EntityType { get; set; } = string.Empty; // Patient, Allergy, Document, etc.
    public Guid? EntityId { get; set; }
    public DateTime EventTimestamp { get; set; }
    
    // Actor Information
    public string ActorId { get; set; } = string.Empty;
    public string ActorName { get; set; } = string.Empty;
    public string? ActorRole { get; set; }
    public string? ActorEmail { get; set; }
    
    // Action Details
    public string Action { get; set; } = string.Empty;
    public string? ActionDescription { get; set; }
    public string? OldValues { get; set; } // JSON
    public string? NewValues { get; set; } // JSON
    
    // Request Information
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? SessionId { get; set; }
    public string? RequestId { get; set; }
    public string? CorrelationId { get; set; }
    
    // Result
    public bool Success { get; set; } = true;
    public string? ErrorMessage { get; set; }
    public int? ResponseCode { get; set; }
    
    // Compliance
    public string? AccessReason { get; set; }
    public string? ConsentId { get; set; }
    public bool PhiAccessed { get; set; } = false;
    public string? DataClassification { get; set; } // Public, Internal, Confidential, Restricted
    
    // System Fields
    public string? ApplicationName { get; set; }
    public string? Environment { get; set; }
    
    // Navigation Properties
    public virtual Patient? Patient { get; set; }
}

