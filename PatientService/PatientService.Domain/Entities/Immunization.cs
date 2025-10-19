namespace PatientService.Domain.Entities;

/// <summary>
/// Represents an immunization/vaccination record for a patient
/// </summary>
public class Immunization
{
    public Guid ImmunizationId { get; set; }
    public Guid PatientId { get; set; }
    
    public string VaccineName { get; set; } = string.Empty;
    public DateTime AdministeredDate { get; set; }
    public int? DoseNumber { get; set; }
    public string? LotNumber { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Site { get; set; } // Injection site
    public string? Route { get; set; } // Route of administration
    
    // Database has these columns with different names:
    // ProviderName (not Provider), AdministeredBy, FacilityName
    public string? ProviderName { get; set; }
    public string? AdministeredBy { get; set; }
    public string? FacilityName { get; set; }
    public string? DoseQuantity { get; set; }
    public int? SeriesDoses { get; set; }
    public string? StatusReason { get; set; }
    
    // FHIR Integration Fields
    public string? FhirImmunizationId { get; set; }
    public string? VaccineCode { get; set; } // CVX code
    public string? CodeSystem { get; set; }
    public string Status { get; set; } = "Completed"; // For FHIR: completed, not-done
    public DateTime? LastFhirSync { get; set; }
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    
    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
}
