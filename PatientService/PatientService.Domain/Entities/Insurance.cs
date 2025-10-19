namespace PatientService.Domain.Entities;

/// <summary>
/// Represents patient insurance coverage
/// Mapped to FHIR Coverage resource
/// </summary>
public class Insurance
{
    public Guid InsuranceId { get; set; }
    public Guid PatientId { get; set; }
    
    // Insurance Plan Details
    public string PayerName { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public string PlanType { get; set; } = string.Empty; // HMO, PPO, EPO, POS, Medicare, Medicaid
    
    // Member Information
    public string MemberId { get; set; } = string.Empty;
    public string? GroupNumber { get; set; }
    public string? GroupName { get; set; }
    
    // Coverage Period
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsPrimary { get; set; } = true;
    public int Priority { get; set; } = 1; // 1=Primary, 2=Secondary, etc.
    
    // Subscriber Information
    public string SubscriberRelationship { get; set; } = "Self"; // Self, Spouse, Child, Other
    public string? SubscriberName { get; set; }
    public DateTime? SubscriberDateOfBirth { get; set; }
    public string? SubscriberSsnEncrypted { get; set; }
    
    // Contact Information
    public string? PayerPhone { get; set; }
    public string? PayerWebsite { get; set; }
    public string? CustomerServicePhone { get; set; }
    
    // Financial Details
    public decimal? Deductible { get; set; }
    public decimal? DeductibleMet { get; set; }
    public decimal? OutOfPocketMax { get; set; }
    public decimal? OutOfPocketMet { get; set; }
    public decimal? Copay { get; set; }
    public decimal? CoinsurancePercentage { get; set; }
    
    // Status
    public string Status { get; set; } = "Active"; // Active, Cancelled, Expired
    public bool IsVerified { get; set; } = false;
    public DateTime? LastVerifiedDate { get; set; }
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    // FHIR Integration
    public string? FhirCoverageId { get; set; }
    public DateTime? LastFhirSync { get; set; }
    
    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual ICollection<InsuranceDocument> Documents { get; set; } = new List<InsuranceDocument>();
}

