using Shared.Common.Models;
using System;

namespace DataIntegrationService.Domain.Entities
{
    public class FHIRResource : BaseEntity
    {
        public string ResourceType { get; set; } = string.Empty;
        public string ResourceId { get; set; } = string.Empty;
        public Guid? PatientId { get; set; }
        public string FHIRVersion { get; set; } = "R4";
        public string ResourceJson { get; set; } = string.Empty;
        public string? ResourceXml { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public int VersionId { get; set; } = 1;
        public string? SourceSystem { get; set; }
        public string? SourceId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
