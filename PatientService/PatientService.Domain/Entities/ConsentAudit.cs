namespace PatientService.Domain.Entities;

/// <summary>
/// Audit trail for consent access and modifications
/// </summary>
public class ConsentAudit
{
    public Guid AuditId { get; set; }
    public Guid ConsentId { get; set; }
    
    // Audit Details
    public string Action { get; set; } = string.Empty; // Created, Accessed, Modified, Revoked
    public string? ActionDetails { get; set; }
    public DateTime ActionDate { get; set; }
    
    // Actor Information
    public string ActorId { get; set; } = string.Empty;
    public string ActorName { get; set; } = string.Empty;
    public string? ActorRole { get; set; }
    public string? ActorOrganization { get; set; }
    
    // Technical Details
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? SessionId { get; set; }
    
    // Data Accessed
    public string? DataTypesAccessed { get; set; } // JSON array of data types
    public string? ResourcesAccessed { get; set; } // JSON array of resource IDs
    
    // Navigation Properties
    public virtual Consent Consent { get; set; } = null!;
}

