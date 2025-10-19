using System.ComponentModel.DataAnnotations.Schema;

namespace PatientService.Domain.Entities;

public class PatientAddress
{
    public int Id { get; set; }
    public Guid PatientId { get; set; }
    public int AddressTypeId { get; set; }
    public string Line1 { get; set; } = string.Empty;
    public string? Line2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }

    [ForeignKey(nameof(PatientId))]
    public Patient Patient { get; set; } = null!;

    [ForeignKey(nameof(AddressTypeId))]
    public AddressType AddressType { get; set; } = null!;
}

public class PatientPhone
{
    public int Id { get; set; }
    public Guid PatientId { get; set; }
    public int PhoneTypeId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;

    [ForeignKey(nameof(PatientId))]
    public Patient Patient { get; set; } = null!;
    [ForeignKey(nameof(PhoneTypeId))]
    public PhoneType PhoneType { get; set; } = null!;
}

public class EmergencyContact
{
    public int Id { get; set; }
    public Guid PatientId { get; set; }
    public int RelationshipTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? AltPhone { get; set; }

    [ForeignKey(nameof(PatientId))]
    public Patient Patient { get; set; } = null!;
    public RelationshipType RelationshipType { get; set; } = null!;
}

public class InsurancePolicy
{
    public int Id { get; set; }
    public Guid PatientId { get; set; }
    public int ProviderId { get; set; }
    public string? GroupNumber { get; set; }
    public string? PolicyNumber { get; set; }
    public string? PolicyHolderName { get; set; }
    public string? ProviderPhone { get; set; }
    public string? MedicareMedicaidId { get; set; }
    public string? CardImagePath { get; set; }

    public Patient Patient { get; set; } = null!;
    public InsuranceProvider Provider { get; set; } = null!;
}

public class SocialHistory
{
    public Guid PatientId { get; set; }
    public int? SmokingStatusId { get; set; }
    public int? AlcoholUseId { get; set; }
    public int? DrugUseId { get; set; }
    public string? Occupation { get; set; }
    public int? LivingSituationId { get; set; }
    public string? ExerciseFrequency { get; set; }
    public string? Diet { get; set; }
    public string? StressLevel { get; set; }
    public int? SleepHours { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Patient Patient { get; set; } = null!;
}

public class LegalConsent
{
    public Guid PatientId { get; set; }
    public Guid ConsentId { get; set; }
    public bool HipaaAgreed { get; set; }
    public bool ConsentToTreat { get; set; }
    public bool AdvanceDirectives { get; set; }
    public string? AdvanceDirectivesPath { get; set; }
    public bool AssignmentOfBenefits { get; set; }
    public bool FinancialResponsibility { get; set; }
    public DateTime? SignedOnUtc { get; set; }
    public string? SignaturePath { get; set; }

    public Patient Patient { get; set; } = null!;
}
