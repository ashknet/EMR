namespace PatientService.Domain.Entities;

/// <summary>
/// Represents a healthcare provider associated with a patient
/// </summary>
public class PatientProvider
{
    public Guid PatientProviderId { get; set; }
    public Guid PatientId { get; set; }
    
    // Provider Type
    public int ProviderTypeId { get; set; } // Reference to ProviderType lookup
    public string ProviderTypeName { get; set; } = string.Empty; // e.g., "Primary Care", "Dental", "Eye Care"
    
    // Provider Details
    public string ProviderName { get; set; } = string.Empty;
    public string? Specialty { get; set; }
    public string? NPI { get; set; } // National Provider Identifier
    
    // Contact Information
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    
    // Address
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
    
    // Additional Details
    public string? PracticeName { get; set; }
    public string? Website { get; set; }
    public string? Notes { get; set; }
    
    // Status
    public bool IsPrimary { get; set; } // Is this the primary provider of this type?
    public bool IsAcceptingPatients { get; set; } = true;
    public string Status { get; set; } = "Active"; // Active, Inactive, Retired
    
    // Dates
    public DateTime? FirstVisitDate { get; set; }
    public DateTime? LastVisitDate { get; set; }
    
    // FHIR Integration
    public string? FhirPractitionerId { get; set; }
    public string? FhirOrganizationId { get; set; }
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation Properties
    public Patient Patient { get; set; } = null!;
}

/// <summary>
/// Lookup table for provider types
/// </summary>
public class ProviderType
{
    public int ProviderTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

