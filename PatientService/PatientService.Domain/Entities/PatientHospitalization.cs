namespace PatientService.Domain.Entities;

/// <summary>
/// Represents patient hospitalization history
/// </summary>
public class PatientHospitalization
{
    public int HospitalizationId { get; set; }
    public Guid PatientId { get; set; }
    
    public string? HospitalName { get; set; }
    public string? Reason { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public DateTime? DischargeDate { get; set; }
    public string? Notes { get; set; }
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    
    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
}

