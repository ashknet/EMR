using Shared.Common.Models;
using System;

namespace HealthHistoryService.Domain.Entities
{
    /// <summary>
    /// Allergy entity - maps to FHIR AllergyIntolerance resource
    /// Critical for patient safety
    /// </summary>
    public class Allergy : BaseEntity
    {
        public Guid PatientId { get; set; }
        
        // Allergy Information
        public string AllergenName { get; set; } = string.Empty;
        public string? AllergenCode { get; set; }
        public string? CodingSystem { get; set; }
        
        // Type and Category
        public string AllergyType { get; set; } = "allergy"; // allergy, intolerance
        public string Category { get; set; } = string.Empty; // food, medication, environment, biologic
        
        // Severity and Criticality
        public string? Criticality { get; set; } // low, high, unable-to-assess
        public string? Severity { get; set; } // mild, moderate, severe
        
        // Clinical Status
        public string ClinicalStatus { get; set; } = "active";
        public string VerificationStatus { get; set; } = "confirmed";
        
        // Reaction Information
        public string? ReactionDescription { get; set; }
        public string? ReactionManifestations { get; set; } // JSON array
        public DateTime? ReactionOnset { get; set; }
        
        // Dates
        public DateTime? OnsetDate { get; set; }
        public DateTime RecordedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastOccurrence { get; set; }
        
        // Source
        public string? RecordedBy { get; set; }
        public string SourceType { get; set; } = "patient-reported";
        
        // Additional Information
        public string? Notes { get; set; }
        
        // FHIR Compliance
        public string? FHIRResourceId { get; set; }
        public string? FHIRResourceJson { get; set; }
    }
}
