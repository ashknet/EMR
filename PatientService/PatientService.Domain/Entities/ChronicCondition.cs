namespace PatientService.Domain.Entities;

/// <summary>
/// Represents a chronic condition for a patient
/// </summary>
public class ChronicCondition
{
    public Guid ConditionId { get; set; }
    public Guid PatientId { get; set; }
    
    public string ConditionName { get; set; } = string.Empty;
    public DateTime? DiagnosedDate { get; set; }
    public string Status { get; set; } = "Active"; // Active, Inactive, Resolved
    public string? Notes { get; set; }
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
}

