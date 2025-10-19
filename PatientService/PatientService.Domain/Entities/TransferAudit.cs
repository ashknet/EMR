namespace PatientService.Domain.Entities;

/// <summary>
/// Audit trail for record transfer operations
/// </summary>
public class TransferAudit
{
    public Guid AuditId { get; set; }
    public Guid TransferId { get; set; }
    
    // Audit Details
    public string Action { get; set; } = string.Empty; // Requested, Approved, Sent, Received, Failed
    public string? ActionDetails { get; set; }
    public DateTime ActionDate { get; set; }
    
    // Actor Information
    public string? ActorId { get; set; }
    public string? ActorName { get; set; }
    public string? ActorRole { get; set; }
    
    // Technical Details
    public string? IpAddress { get; set; }
    public string? SessionId { get; set; }
    public string? ErrorMessage { get; set; }
    public string? StackTrace { get; set; }
    
    // Navigation Properties
    public virtual RecordTransfer Transfer { get; set; } = null!;
}

