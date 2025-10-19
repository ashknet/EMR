using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using PatientService.Domain.Entities;
using System.Text.Json;
using Condition = Hl7.Fhir.Model.Condition;

namespace PatientService.Infrastructure.Services;

/// <summary>
/// Service for FHIR R4 resource mapping (US Core profiles)
/// </summary>
public interface IFhirService
{
    Hl7.Fhir.Model.Patient MapToFhirPatient(Domain.Entities.Patient patient);
    AllergyIntolerance MapToFhirAllergyIntolerance(Allergy allergy);
    Hl7.Fhir.Model.Condition MapToFhirCondition(Domain.Entities.Condition condition);
    Hl7.Fhir.Model.Immunization MapToFhirImmunization(Domain.Entities.Immunization immunization);
    Coverage MapToFhirCoverage(Insurance insurance);
    Bundle CreateTransferBundle(Domain.Entities.Patient patient, List<Allergy> allergies, 
        List<Domain.Entities.Condition> conditions, List<Domain.Entities.Immunization> immunizations);
    string SerializeToJson(Resource resource);
}

public class FhirService : IFhirService
{
    private readonly FhirJsonSerializer _serializer;

    public FhirService()
    {
        _serializer = new FhirJsonSerializer(new SerializerSettings { Pretty = true });
    }

    public Hl7.Fhir.Model.Patient MapToFhirPatient(Domain.Entities.Patient patient)
    {
        var fhirPatient = new Hl7.Fhir.Model.Patient
        {
            Id = patient.FhirPatientId ?? Guid.NewGuid().ToString(),
            Meta = new Meta
            {
                Profile = new[] { "http://hl7.org/fhir/us/core/StructureDefinition/us-core-patient" }
            },
            Identifier = new List<Identifier>
            {
                new Identifier
                {
                    System = "urn:oid:2.16.840.1.113883.4.1", // SSN
                    Value = "***-**-****" // Masked for FHIR
                }
            },
            Name = new List<HumanName>
            {
                new HumanName
                {
                    Use = HumanName.NameUse.Official,
                    Family = patient.LastName,
                    Given = new[] { patient.FirstName, patient.MiddleName }.Where(s => !string.IsNullOrEmpty(s))
                }
            },
            Gender = patient.Gender.ToLower() switch
            {
                "male" => AdministrativeGender.Male,
                "female" => AdministrativeGender.Female,
                "other" => AdministrativeGender.Other,
                _ => AdministrativeGender.Unknown
            },
            BirthDate = patient.DateOfBirth?.ToString("yyyy-MM-dd"),
            Address = new List<Address>
            {
                new Address
                {
                    Use = Address.AddressUse.Home,
                    Line = new[] { patient.AddressLine1, patient.AddressLine2 }.Where(s => !string.IsNullOrEmpty(s)),
                    City = patient.City,
                    State = patient.State,
                    PostalCode = patient.ZipCode,
                    Country = patient.Country
                }
            },
            Telecom = new List<ContactPoint>
            {
                new ContactPoint
                {
                    System = ContactPoint.ContactPointSystem.Phone,
                    Value = patient.PhoneNumber,
                    Use = ContactPoint.ContactPointUse.Mobile
                },
                new ContactPoint
                {
                    System = ContactPoint.ContactPointSystem.Email,
                    Value = patient.Email
                }
            }
        };

        // Add US Core race and ethnicity extensions
        if (!string.IsNullOrEmpty(patient.Race))
        {
            fhirPatient.Extension.Add(new Extension
            {
                Url = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-race",
                Extension = new List<Extension>
                {
                    new Extension("text", new FhirString(patient.Race))
                }
            });
        }

        if (!string.IsNullOrEmpty(patient.Ethnicity))
        {
            fhirPatient.Extension.Add(new Extension
            {
                Url = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-ethnicity",
                Extension = new List<Extension>
                {
                    new Extension("text", new FhirString(patient.Ethnicity))
                }
            });
        }

        return fhirPatient;
    }

    public AllergyIntolerance MapToFhirAllergyIntolerance(Allergy allergy)
    {
        return new AllergyIntolerance
        {
            Id = allergy.FhirAllergyIntoleranceId ?? Guid.NewGuid().ToString(),
            Meta = new Meta
            {
                Profile = new[] { "http://hl7.org/fhir/us/core/StructureDefinition/us-core-allergyintolerance" }
            },
            ClinicalStatus = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding("http://terminology.hl7.org/CodeSystem/allergyintolerance-clinical", 
                        allergy.ClinicalStatus.ToLower())
                }
            },
            Code = new CodeableConcept
            {
                Text = allergy.AllergenName,
                Coding = string.IsNullOrEmpty(allergy.AllergenCode) ? null : new List<Coding>
                {
                    new Coding(allergy.CodeSystem, allergy.AllergenCode, allergy.AllergenName)
                }
            },
            Patient = new ResourceReference($"Patient/{allergy.PatientId}"),
            Onset = allergy.OnsetDate.HasValue ? new FhirDateTime(allergy.OnsetDate.Value) : null
        };
    }

    public Condition MapToFhirCondition(Domain.Entities.Condition condition)
    {
        return new Condition
        {
            Id = condition.FhirConditionId ?? Guid.NewGuid().ToString(),
            Meta = new Meta
            {
                Profile = new[] { "http://hl7.org/fhir/us/core/StructureDefinition/us-core-condition" }
            },
            ClinicalStatus = new CodeableConcept
            {
                Coding = new List<Coding>
                {
                    new Coding("http://terminology.hl7.org/CodeSystem/condition-clinical", 
                        condition.ClinicalStatus.ToLower())
                }
            },
            Code = new CodeableConcept
            {
                Text = condition.ConditionName,
                Coding = string.IsNullOrEmpty(condition.ConditionCode) ? null : new List<Coding>
                {
                    new Coding(condition.CodeSystem ?? "http://hl7.org/fhir/sid/icd-10", 
                        condition.ConditionCode, condition.ConditionName)
                }
            },
            Subject = new ResourceReference($"Patient/{condition.PatientId}"),
            Onset = condition.OnsetDate.HasValue ? new FhirDateTime(condition.OnsetDate.Value) : null
        };
    }

    public Hl7.Fhir.Model.Immunization MapToFhirImmunization(Domain.Entities.Immunization immunization)
    {
        return new Hl7.Fhir.Model.Immunization
        {
            Id = immunization.FhirImmunizationId ?? Guid.NewGuid().ToString(),
            Meta = new Meta
            {
                Profile = new[] { "http://hl7.org/fhir/us/core/StructureDefinition/us-core-immunization" }
            },
            Status = immunization.Status.ToLower() switch
            {
                "completed" => Hl7.Fhir.Model.Immunization.ImmunizationStatusCodes.Completed,
                "not-done" => Hl7.Fhir.Model.Immunization.ImmunizationStatusCodes.NotDone,
                _ => Hl7.Fhir.Model.Immunization.ImmunizationStatusCodes.Completed
            },
            VaccineCode = new CodeableConcept
            {
                Text = immunization.VaccineName,
                Coding = string.IsNullOrEmpty(immunization.VaccineCode) ? null : new List<Coding>
                {
                    new Coding("http://hl7.org/fhir/sid/cvx", immunization.VaccineCode, immunization.VaccineName)
                }
            },
            Patient = new ResourceReference($"Patient/{immunization.PatientId}"),
            Occurrence = new FhirDateTime(immunization.AdministeredDate)
        };
    }

    public Coverage MapToFhirCoverage(Insurance insurance)
    {
        return new Coverage
        {
            Id = insurance.FhirCoverageId ?? Guid.NewGuid().ToString(),
            Status = insurance.Status.ToLower() switch
            {
                "active" => FinancialResourceStatusCodes.Active,
                "cancelled" => FinancialResourceStatusCodes.Cancelled,
                _ => FinancialResourceStatusCodes.Active
            },
            Subscriber = new ResourceReference($"Patient/{insurance.PatientId}"),
            Beneficiary = new ResourceReference($"Patient/{insurance.PatientId}"),
            Payor = new List<ResourceReference>
            {
                new ResourceReference { Display = insurance.PayerName }
            },
            //Class_ = new List<Coverage.ClassComponent>
            //{
            //    new Coverage.ClassComponent
            //    {
            //        Type = new CodeableConcept { Text = "Group" },
            //        Value = insurance.GroupNumber ?? "N/A"
            //    },
            //    new Coverage.ClassComponent
            //    {
            //        Type = new CodeableConcept { Text = "Plan" },
            //        Value = insurance.PlanName
            //    }
            //}
        };
    }

    public Bundle CreateTransferBundle(Domain.Entities.Patient patient, List<Allergy> allergies,
        List<Domain.Entities.Condition> conditions, List<Domain.Entities.Immunization> immunizations)
    {
        var bundle = new Bundle
        {
            Type = Bundle.BundleType.Collection,
            Timestamp = DateTimeOffset.UtcNow,
            Entry = new List<Bundle.EntryComponent>()
        };

        // Add patient
        bundle.Entry.Add(new Bundle.EntryComponent
        {
            FullUrl = $"urn:uuid:{patient.PatientId}",
            Resource = MapToFhirPatient(patient)
        });

        // Add allergies
        foreach (var allergy in allergies)
        {
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = $"urn:uuid:{allergy.AllergyId}",
                Resource = MapToFhirAllergyIntolerance(allergy)
            });
        }

        // Add conditions
        foreach (var condition in conditions)
        {
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = $"urn:uuid:{condition.ConditionId}",
                Resource = MapToFhirCondition(condition)
            });
        }

        // Add immunizations
        foreach (var immunization in immunizations)
        {
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = $"urn:uuid:{immunization.ImmunizationId}",
                Resource = MapToFhirImmunization(immunization)
            });
        }

        return bundle;
    }

    public string SerializeToJson(Resource resource)
    {
        return _serializer.SerializeToString(resource);
    }
}

