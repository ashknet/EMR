using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;

namespace Shared.FHIR.Models
{
    /// <summary>
    /// FHIR R4 Patient resource wrapper for US Core compliance
    /// Implements US Core Patient Profile requirements
    /// </summary>
    public class FHIRPatientBuilder
    {
        private readonly Patient _patient;

        public FHIRPatientBuilder()
        {
            _patient = new Patient
            {
                Meta = new Meta
                {
                    Profile = new[] { "http://hl7.org/fhir/us/core/StructureDefinition/us-core-patient" }
                }
            };
        }

        public FHIRPatientBuilder WithIdentifier(string value, string system = "urn:oid:2.16.840.1.113883.4.1")
        {
            _patient.Identifier.Add(new Identifier
            {
                System = system,
                Value = value,
                Use = Identifier.IdentifierUse.Official
            });
            return this;
        }

        public FHIRPatientBuilder WithName(string family, string given, string? middle = null)
        {
            var name = new HumanName
            {
                Use = HumanName.NameUse.Official,
                Family = family,
                Given = new[] { given }
            };
            
            if (!string.IsNullOrEmpty(middle))
            {
                name.Given = new[] { given, middle };
            }

            _patient.Name.Add(name);
            return this;
        }

        public FHIRPatientBuilder WithGender(AdministrativeGender gender)
        {
            _patient.Gender = gender;
            return this;
        }

        public FHIRPatientBuilder WithBirthDate(string birthDate)
        {
            _patient.BirthDate = birthDate;
            return this;
        }

        public FHIRPatientBuilder WithAddress(string line, string city, string state, string postalCode, string country = "US")
        {
            _patient.Address.Add(new Address
            {
                Use = Address.AddressUse.Home,
                Type = Address.AddressType.Physical,
                Line = new[] { line },
                City = city,
                State = state,
                PostalCode = postalCode,
                Country = country
            });
            return this;
        }

        public FHIRPatientBuilder WithTelecom(string value, ContactPoint.ContactPointSystem system, ContactPoint.ContactPointUse use)
        {
            _patient.Telecom.Add(new ContactPoint
            {
                System = system,
                Value = value,
                Use = use
            });
            return this;
        }

        public FHIRPatientBuilder WithRace(string raceCode, string raceDisplay)
        {
            var raceExtension = new Extension
            {
                Url = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-race",
                Extension = new List<Extension>
                {
                    new Extension
                    {
                        Url = "ombCategory",
                        Value = new Coding
                        {
                            System = "urn:oid:2.16.840.1.113883.6.238",
                            Code = raceCode,
                            Display = raceDisplay
                        }
                    },
                    new Extension
                    {
                        Url = "text",
                        Value = new FhirString(raceDisplay)
                    }
                }
            };

            _patient.Extension.Add(raceExtension);
            return this;
        }

        public FHIRPatientBuilder WithEthnicity(string ethnicityCode, string ethnicityDisplay)
        {
            var ethnicityExtension = new Extension
            {
                Url = "http://hl7.org/fhir/us/core/StructureDefinition/us-core-ethnicity",
                Extension = new List<Extension>
                {
                    new Extension
                    {
                        Url = "ombCategory",
                        Value = new Coding
                        {
                            System = "urn:oid:2.16.840.1.113883.6.238",
                            Code = ethnicityCode,
                            Display = ethnicityDisplay
                        }
                    },
                    new Extension
                    {
                        Url = "text",
                        Value = new FhirString(ethnicityDisplay)
                    }
                }
            };

            _patient.Extension.Add(ethnicityExtension);
            return this;
        }

        public Patient Build()
        {
            return _patient;
        }
    }
}
