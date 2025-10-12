using Shared.Common.Models;
using System;

namespace HealthHistoryService.Domain.Entities
{
    /// <summary>
    /// Immunization entity - maps to FHIR Immunization resource
    /// Vaccination record tracking
    /// </summary>
    public class Immunization : BaseEntity
    {
        public Guid PatientId { get; set; }
        
        // Vaccine Information
        public string VaccineName { get; set; } = string.Empty;
        public string? VaccineCode { get; set; } // CVX code
        public string? LotNumber { get; set; }
        public string? Manufacturer { get; set; }
        
        // Administration
        public DateTime AdministrationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? DoseNumber { get; set; }
        public int? SeriesCount { get; set; }
        
        // Status
        public string Status { get; set; } = "completed";
        
        // Site and Route
        public string? BodySite { get; set; }
        public string? Route { get; set; }
        public string? DoseQuantity { get; set; }
        
        // Provider Information
        public string? AdministeredBy { get; set; }
        public string? AdministeredAt { get; set; }
        
        // Reaction/Notes
        public string? ReactionDescription { get; set; }
        public string? Notes { get; set; }
        
        // Source
        public string SourceType { get; set; } = "provider-entered";
        public DateTime RecordedDate { get; set; } = DateTime.UtcNow;
        
        // FHIR Compliance
        public string? FHIRResourceId { get; set; }
        public string? FHIRResourceJson { get; set; }
    }
}
