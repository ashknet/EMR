namespace PatientService.Domain.Entities;

/// <summary>
/// Represents patient surgical history
/// </summary>
public class PatientSurgery
{
    public int SurgeryId { get; set; }
    public Guid PatientId { get; set; }
    
    public string SurgeryType { get; set; } = string.Empty;
    public DateTime? SurgeryDate { get; set; }
    public string? Notes { get; set; }
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    
    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
}

