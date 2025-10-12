using Shared.Common.Models;
using System;

namespace HealthHistoryService.Domain.Entities
{
    /// <summary>
    /// Medical condition entity - maps to FHIR Condition resource
    /// Represents diagnoses, problems, and chronic conditions
    /// </summary>
    public class Condition : BaseEntity
    {
        public Guid PatientId { get; set; }
        
        // Condition Information
        public string ConditionName { get; set; } = string.Empty;
        public string? ConditionCode { get; set; } // ICD-10, SNOMED CT
        public string? CodingSystem { get; set; }
        public string? Category { get; set; }
        
        // Clinical Status
        public string ClinicalStatus { get; set; } = "active";
        public string VerificationStatus { get; set; } = "confirmed";
        
        // Severity
        public string? Severity { get; set; }
        
        // Dates
        public DateTime? OnsetDate { get; set; }
        public DateTime? AbatementDate { get; set; }
        public DateTime RecordedDate { get; set; } = DateTime.UtcNow;
        
        // Additional Information
        public string? BodySite { get; set; }
        public string? Notes { get; set; }
        
        // Source
        public string? RecordedBy { get; set; }
        public string? DiagnosedBy { get; set; }
        public string SourceType { get; set; } = "patient-reported";
        
        // FHIR Compliance
        public string? FHIRResourceId { get; set; }
        public string? FHIRResourceJson { get; set; }
    }
}
