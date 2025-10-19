namespace PatientService.Domain.Entities;

/// <summary>
/// Represents patient allergies and intolerances
/// Mapped to FHIR AllergyIntolerance resource
/// </summary>
public class Allergy
{
    public Guid AllergyId { get; set; }
    public Guid PatientId { get; set; }
    
    // Allergy Details
    public string AllergenName { get; set; } = string.Empty;
    public string AllergenType { get; set; } = string.Empty; // Drug, Food, Environment, Biologic
    public string? AllergenCode { get; set; } // RxNorm, SNOMED, etc.
    public string? CodeSystem { get; set; }
    
    // Clinical Information
    public string Severity { get; set; } = string.Empty; // Mild, Moderate, Severe
    public string ClinicalStatus { get; set; } = "Active"; // Active, Inactive, Resolved
    public string? Reaction { get; set; }
    public DateTime? OnsetDate { get; set; }
    
    // Source
    public string? ReportedBy { get; set; }
    public string? VerificationStatus { get; set; } // Confirmed, Unconfirmed, Refuted
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    // FHIR Integration
    public string? FhirAllergyIntoleranceId { get; set; }
    public DateTime? LastFhirSync { get; set; }
    
    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
}

