namespace PatientService.Domain.Entities;

/// <summary>
/// Represents insurance-related documents (cards, EOBs, etc.)
/// </summary>
public class InsuranceDocument
{
    public Guid InsuranceDocumentId { get; set; }
    public Guid InsuranceId { get; set; }
    
    // Document Details
    public string FileName { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty; // Insurance Card, EOB, Pre-Auth, etc.
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    
    // Storage
    public string StorageLocation { get; set; } = string.Empty;
    public string? EncryptionKeyId { get; set; }
    public string? FileHash { get; set; }
    
    // Validity
    public DateTime? EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; } = true;
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    
    // Navigation Properties
    public virtual Insurance Insurance { get; set; } = null!;
}

