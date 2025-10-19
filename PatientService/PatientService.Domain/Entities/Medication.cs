namespace PatientService.Domain.Entities;

/// <summary>
/// Represents patient medications
/// </summary>
public class Medication
{
    public Guid MedicationId { get; set; }
    public Guid PatientId { get; set; }
    
    public string MedicationName { get; set; } = string.Empty;
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public string? Prescriber { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    
    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
}

