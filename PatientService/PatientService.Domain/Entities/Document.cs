namespace PatientService.Domain.Entities;

/// <summary>
/// Represents patient medical documents
/// Mapped to FHIR DocumentReference resource
/// </summary>
public class Document
{
    public Guid DocumentId { get; set; }
    public Guid PatientId { get; set; }
    
    // Document Details
    public string FileName { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty; // Lab Result, Imaging, Discharge Summary, etc.
    public string? DocumentTypeCode { get; set; } // LOINC code
    public string ContentType { get; set; } = string.Empty; // application/pdf, image/jpeg, etc.
    public long FileSize { get; set; }
    
    // Storage
    public string StorageLocation { get; set; } = string.Empty; // Azure Blob URL
    public string? EncryptionKeyId { get; set; }
    public string? FileHash { get; set; } // SHA-256 for integrity
    
    // Classification
    public string? Category { get; set; } // Clinical, Administrative, Financial
    public string? Specialty { get; set; }
    public string? FacilityType { get; set; }
    
    // Clinical Context
    public Guid? EncounterId { get; set; }
    public string? ProviderId { get; set; }
    public string? ProviderName { get; set; }
    public DateTime? ServiceDate { get; set; }
    
    // OCR Processing
    public bool OcrProcessed { get; set; } = false;
    public DateTime? OcrProcessedDate { get; set; }
    public string? OcrText { get; set; }
    public string? OcrConfidence { get; set; }
    public bool FhirMapped { get; set; } = false;
    public string? FhirMappingResult { get; set; } // JSON
    
    // Status
    public string Status { get; set; } = "Current"; // Current, Superseded, Entered-in-error
    public string? Description { get; set; }
    
    // Security
    public string SecurityLevel { get; set; } = "Standard"; // Standard, Restricted, Very-Restricted
    public bool RequiresConsent { get; set; } = true;
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    // FHIR Integration
    public string? FhirDocumentReferenceId { get; set; }
    public DateTime? LastFhirSync { get; set; }
    
    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Encounter? Encounter { get; set; }
}

