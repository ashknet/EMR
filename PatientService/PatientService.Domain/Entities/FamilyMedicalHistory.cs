namespace PatientService.Domain.Entities;

/// <summary>
/// Represents family medical history
/// </summary>
public class FamilyMedicalHistory
{
    public int FamilyHistoryId { get; set; }
    public Guid PatientId { get; set; }
    
    public string Relative { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string? Notes { get; set; }
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    
    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
}

