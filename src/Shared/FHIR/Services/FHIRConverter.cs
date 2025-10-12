using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using System;
using System.Threading.Tasks;

namespace Shared.FHIR.Services
{
    /// <summary>
    /// FHIR resource conversion service for JSON and XML serialization
    /// Supports FHIR R4 specification
    /// </summary>
    public class FHIRConverter
    {
        private readonly FhirJsonSerializer _jsonSerializer;
        private readonly FhirXmlSerializer _xmlSerializer;
        private readonly FhirJsonParser _jsonParser;
        private readonly FhirXmlParser _xmlParser;

        public FHIRConverter()
        {
            var settings = new SerializerSettings
            {
                Pretty = true
            };

            _jsonSerializer = new FhirJsonSerializer(settings);
            _xmlSerializer = new FhirXmlSerializer(settings);
            _jsonParser = new FhirJsonParser();
            _xmlParser = new FhirXmlParser();
        }

        public string ToJson(Resource resource)
        {
            try
            {
                return _jsonSerializer.SerializeToString(resource);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to serialize FHIR resource to JSON: {ex.Message}", ex);
            }
        }

        public string ToXml(Resource resource)
        {
            try
            {
                return _xmlSerializer.SerializeToString(resource);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to serialize FHIR resource to XML: {ex.Message}", ex);
            }
        }

        public T FromJson<T>(string json) where T : Resource
        {
            try
            {
                return _jsonParser.Parse<T>(json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse FHIR resource from JSON: {ex.Message}", ex);
            }
        }

        public T FromXml<T>(string xml) where T : Resource
        {
            try
            {
                return _xmlParser.Parse<T>(xml);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse FHIR resource from XML: {ex.Message}", ex);
            }
        }

        public Bundle CreateBundle(BundleType type = BundleType.Collection)
        {
            return new Bundle
            {
                Id = Guid.NewGuid().ToString(),
                Type = type,
                Timestamp = DateTimeOffset.UtcNow
            };
        }

        public Bundle AddResourceToBundle(Bundle bundle, Resource resource)
        {
            bundle.Entry.Add(new Bundle.EntryComponent
            {
                FullUrl = $"urn:uuid:{resource.Id}",
                Resource = resource
            });
            return bundle;
        }
    }
}
