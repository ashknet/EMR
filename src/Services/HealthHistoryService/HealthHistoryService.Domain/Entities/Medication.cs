using Shared.Common.Models;
using System;

namespace HealthHistoryService.Domain.Entities
{
    /// <summary>
    /// Medication entity - maps to FHIR MedicationStatement resource
    /// Tracks current and historical medications
    /// </summary>
    public class Medication : BaseEntity
    {
        public Guid PatientId { get; set; }
        
        // Medication Information
        public string MedicationName { get; set; } = string.Empty;
        public string? MedicationCode { get; set; } // RxNorm
        public string? GenericName { get; set; }
        public string? BrandName { get; set; }
        
        // Dosage
        public string? Dosage { get; set; }
        public string? DosageForm { get; set; }
        public string? Route { get; set; }
        public string? Frequency { get; set; }
        
        // Status
        public string Status { get; set; } = "active";
        
        // Dates
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime RecordedDate { get; set; } = DateTime.UtcNow;
        
        // Prescriber Information
        public string? PrescribedBy { get; set; }
        public DateTime? PrescribedDate { get; set; }
        public string? Pharmacy { get; set; }
        
        // Reason
        public string? ReasonCode { get; set; }
        public string? ReasonDescription { get; set; }
        
        // Additional Information
        public string? Instructions { get; set; }
        public string? Notes { get; set; }
        public bool IsOverTheCounter { get; set; }
        
        // Source
        public string SourceType { get; set; } = "patient-reported";
        
        // FHIR Compliance
        public string? FHIRResourceId { get; set; }
        public string? FHIRResourceJson { get; set; }
    }
}
