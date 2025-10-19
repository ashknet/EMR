namespace PatientService.Domain.Entities;

/// <summary>
/// Represents patient consent for data sharing and access
/// Supports QR code sharing and granular permissions
/// </summary>
public class Consent
{
    public Guid ConsentId { get; set; }
    public Guid PatientId { get; set; }
    
    // Consent Details
    public string ConsentType { get; set; } = string.Empty; // Share, Transfer, Research, Marketing
    public string Status { get; set; } = "Active"; // Active, Revoked, Expired, Rejected
    public string Scope { get; set; } = string.Empty; // Full, Demographics, Medical, Insurance
    
    // Granular Permissions
    public bool AllowDemographics { get; set; } = false;
    public bool AllowAllergies { get; set; } = false;
    public bool AllowConditions { get; set; } = false;
    public bool AllowImmunizations { get; set; } = false;
    public bool AllowMedications { get; set; } = false;
    public bool AllowInsurance { get; set; } = false;
    public bool AllowDocuments { get; set; } = false;
    public bool AllowEncounters { get; set; } = false;
    
    // Recipient Information
    public string? RecipientName { get; set; }
    public string? RecipientOrganization { get; set; }
    public string? RecipientType { get; set; } // Provider, Hospital, Insurance, Family, Other
    public string? RecipientEmail { get; set; }
    
    // QR Code Sharing
    public string? ShareToken { get; set; } // Unique token for QR code
    public DateTime? TokenExpiryDate { get; set; }
    public int? MaxAccessCount { get; set; }
    public int AccessCount { get; set; } = 0;
    
    // Time Period
    public DateTime GrantedDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? RevokedDate { get; set; }
    public string? RevocationReason { get; set; }
    
    // Legal
    public string? LegalBasis { get; set; } // HIPAA Authorization, State Law, etc.
    public string? ConsentFormUrl { get; set; }
    public bool SignatureObtained { get; set; } = false;
    public DateTime? SignatureDate { get; set; }
    
    // Purpose
    public string? Purpose { get; set; }
    public string? Notes { get; set; }
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
    
    // Audit Trail
    public string? LastAccessedBy { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    
    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual ICollection<ConsentAudit> ConsentAudits { get; set; } = new List<ConsentAudit>();
}

