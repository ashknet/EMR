namespace PatientService.Domain.Entities;

/// <summary>
/// Represents medical record transfer requests
/// </summary>
public class RecordTransfer
{
    public Guid TransferId { get; set; }
    public Guid PatientId { get; set; }
    public Guid? ConsentId { get; set; }
    
    // Transfer Type
    public string TransferType { get; set; } = string.Empty; // Outbound, Inbound
    public string TransferMethod { get; set; } = string.Empty; // FHIR, Direct, Manual, Portal
    
    // Recipient/Sender Information
    public string? RecipientOrganization { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientEmail { get; set; }
    public string? RecipientFhirEndpoint { get; set; }
    public string? RecipientNpi { get; set; }
    
    public string? SenderOrganization { get; set; }
    public string? SenderName { get; set; }
    public string? SenderEmail { get; set; }
    
    // Transfer Details
    public string? Purpose { get; set; }
    public string? RequestedBy { get; set; }
    public DateTime RequestedDate { get; set; }
    
    // Status
    public string Status { get; set; } = "Requested"; // Requested, Approved, Processing, Completed, Failed, Cancelled
    public string? StatusReason { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime? CancelledDate { get; set; }
    public string? CancellationReason { get; set; }
    
    // Data Scope
    public bool IncludeDemographics { get; set; } = true;
    public bool IncludeAllergies { get; set; } = true;
    public bool IncludeConditions { get; set; } = true;
    public bool IncludeImmunizations { get; set; } = true;
    public bool IncludeMedications { get; set; } = true;
    public bool IncludeEncounters { get; set; } = true;
    public bool IncludeDocuments { get; set; } = true;
    public bool IncludeInsurance { get; set; } = false;
    
    // Date Range
    public DateTime? DataStartDate { get; set; }
    public DateTime? DataEndDate { get; set; }
    
    // FHIR Bundle
    public string? FhirBundleUrl { get; set; }
    public string? FhirBundleId { get; set; }
    public int? ResourceCount { get; set; }
    
    // Security
    public string? AccessToken { get; set; }
    public DateTime? TokenExpiryDate { get; set; }
    public string? EncryptionKeyId { get; set; }
    
    // Tracking
    public string? TrackingNumber { get; set; }
    public string? ExternalTransferId { get; set; }
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    
    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Consent? Consent { get; set; }
    public virtual ICollection<TransferAudit> TransferAudits { get; set; } = new List<TransferAudit>();
}

