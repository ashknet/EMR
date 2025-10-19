namespace PatientService.Domain.Entities;

/// <summary>
/// Represents patient medical conditions/diagnoses
/// Mapped to FHIR Condition resource
/// </summary>
public class Condition
{
    public Guid ConditionId { get; set; }
    public Guid PatientId { get; set; }
    
    // Condition Details
    public string ConditionName { get; set; } = string.Empty;
    public string? ConditionCode { get; set; } // ICD-10, SNOMED
    public string? CodeSystem { get; set; }
    
    // Clinical Information
    public string ClinicalStatus { get; set; } = "Active"; // Active, Recurrence, Relapse, Inactive, Remission, Resolved
    public string? VerificationStatus { get; set; } // Confirmed, Provisional, Differential, Refuted
    public string? Severity { get; set; } // Mild, Moderate, Severe
    public string? Category { get; set; } // Problem-list-item, encounter-diagnosis
    
    // Timeline
    public DateTime? OnsetDate { get; set; }
    public DateTime? AbatementDate { get; set; }
    public DateTime? RecordedDate { get; set; }
    
    // Notes
    public string? Notes { get; set; }
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    // FHIR Integration
    public string? FhirConditionId { get; set; }
    public DateTime? LastFhirSync { get; set; }
    
    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
}

