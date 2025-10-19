namespace PatientService.Domain.Entities;

/// <summary>
/// Junction table linking patients to chronic conditions
/// </summary>
public class PatientChronicCondition
{
    public Guid PatientId { get; set; }
    public Guid ConditionId { get; set; }
    
    public DateTime? DiagnosedDate { get; set; }
    public string? Notes { get; set; }
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    
    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Condition Condition { get; set; } = null!;
}

