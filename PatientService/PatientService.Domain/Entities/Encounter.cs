namespace PatientService.Domain.Entities;

/// <summary>
/// Represents patient visits and encounters
/// Mapped to FHIR Encounter resource
/// </summary>
public class Encounter
{
    public Guid EncounterId { get; set; }
    public Guid PatientId { get; set; }
    
    // Encounter Details
    public string EncounterType { get; set; } = string.Empty; // Inpatient, Outpatient, Emergency, Virtual
    public string? EncounterClass { get; set; } // AMB, EMER, IMP, etc.
    public string Status { get; set; } = "Planned"; // Planned, Arrived, In-Progress, Finished, Cancelled
    
    // Timeline
    public DateTime? PlannedStartDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? DurationMinutes { get; set; }
    
    // Provider Information
    public string? ProviderId { get; set; }
    public string? ProviderName { get; set; }
    public string? ProviderSpecialty { get; set; }
    public string? ProviderNpi { get; set; }
    
    // Facility Information
    public string? FacilityId { get; set; }
    public string? FacilityName { get; set; }
    public string? Department { get; set; }
    public string? Room { get; set; }
    
    // Clinical Details
    public string? ChiefComplaint { get; set; }
    public string? ReasonForVisit { get; set; }
    public string? ReasonCode { get; set; } // ICD-10, SNOMED
    public string? Priority { get; set; } // Routine, Urgent, Emergency
    
    // Diagnosis
    public string? PrimaryDiagnosisCode { get; set; }
    public string? PrimaryDiagnosisDescription { get; set; }
    public string? SecondaryDiagnoses { get; set; } // JSON array
    
    // Disposition
    public string? DischargeDisposition { get; set; } // Home, Admitted, Transferred, etc.
    public string? DischargeInstructions { get; set; }
    
    // Billing
    public string? BillingCode { get; set; }
    public decimal? EstimatedCost { get; set; }
    
    // Notes
    public string? ClinicalNotes { get; set; }
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    // FHIR Integration
    public string? FhirEncounterId { get; set; }
    public DateTime? LastFhirSync { get; set; }
    
    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
}

