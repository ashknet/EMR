namespace PatientService.Domain.Entities;

/// <summary>
/// Represents family relationships and proxy/guardian permissions
/// </summary>
public class FamilyRelation
{
    public Guid RelationId { get; set; }
    public Guid PatientId { get; set; }
    public Guid RelatedPatientId { get; set; }
    
    // Relationship type: Spouse, Child, Parent, Sibling, Guardian, Proxy, Other
    public string RelationType { get; set; } = string.Empty;
    
    // Permissions
    public bool IsGuardian { get; set; } = false;
    public bool IsProxy { get; set; } = false;
    public bool CanViewRecords { get; set; } = false;
    public bool CanManageRecords { get; set; } = false;
    public bool CanGrantConsent { get; set; } = false;
    
    // Legal Documentation
    public string? LegalDocumentType { get; set; } // Power of Attorney, Guardianship, etc.
    public string? LegalDocumentNumber { get; set; }
    public DateTime? LegalDocumentExpiryDate { get; set; }
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
    public virtual Patient RelatedPatient { get; set; } = null!;
}

