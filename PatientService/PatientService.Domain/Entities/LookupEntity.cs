namespace PatientService.Domain.Entities;

/// <summary>
/// Base class for simple lookup tables (e.g., Gender, MaritalStatus)
/// </summary>
public abstract class LookupEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}

public class Gender : LookupEntity { }
public class MaritalStatus : LookupEntity { }
public class Race : LookupEntity { }
public class Language : LookupEntity { }
public class RelationshipType : LookupEntity { }
public class InsuranceProvider : LookupEntity { public string? Phone { get; set; } }
public class AddressType : LookupEntity { }
public class PhoneType : LookupEntity { }
public class SmokingStatus : LookupEntity { }
public class AlcoholUse : LookupEntity { }
public class DrugUse : LookupEntity { }
public class LivingSituation : LookupEntity { }
public class CommunicationPreference : LookupEntity { }
public class ChronicConditionLookup : LookupEntity { }
public class AllergyType : LookupEntity { }
public class MedicationLookup : LookupEntity { }
public class SurgeryType : LookupEntity { }
public class ImmunizationType : LookupEntity { }
public class ConditionLookup : LookupEntity { }
