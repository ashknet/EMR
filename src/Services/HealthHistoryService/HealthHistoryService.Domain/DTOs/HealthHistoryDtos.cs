using System;
using System.ComponentModel.DataAnnotations;

namespace HealthHistoryService.Domain.DTOs
{
    // Condition DTOs
    public class ConditionDto
    {
        public Guid? Id { get; set; }
        public Guid PatientId { get; set; }
        public string ConditionName { get; set; } = string.Empty;
        public string? ConditionCode { get; set; }
        public string? Category { get; set; }
        public string ClinicalStatus { get; set; } = "active";
        public string? Severity { get; set; }
        public DateTime? OnsetDate { get; set; }
        public DateTime? AbatementDate { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateConditionRequest
    {
        [Required]
        public Guid PatientId { get; set; }
        
        [Required]
        [StringLength(255)]
        public string ConditionName { get; set; } = string.Empty;
        
        public string? ConditionCode { get; set; }
        public string? Category { get; set; }
        public string? Severity { get; set; }
        public DateTime? OnsetDate { get; set; }
        public string? Notes { get; set; }
    }

    // Allergy DTOs
    public class AllergyDto
    {
        public Guid? Id { get; set; }
        public Guid PatientId { get; set; }
        public string AllergenName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Criticality { get; set; }
        public string? Severity { get; set; }
        public string ClinicalStatus { get; set; } = "active";
        public string? ReactionDescription { get; set; }
        public DateTime? OnsetDate { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateAllergyRequest
    {
        [Required]
        public Guid PatientId { get; set; }
        
        [Required]
        [StringLength(255)]
        public string AllergenName { get; set; } = string.Empty;
        
        [Required]
        public string Category { get; set; } = string.Empty;
        
        public string? Criticality { get; set; }
        public string? Severity { get; set; }
        public string? ReactionDescription { get; set; }
        public DateTime? OnsetDate { get; set; }
        public string? Notes { get; set; }
    }

    // Medication DTOs
    public class MedicationDto
    {
        public Guid? Id { get; set; }
        public Guid PatientId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public string? GenericName { get; set; }
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public string Status { get; set; } = "active";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? PrescribedBy { get; set; }
        public string? Instructions { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateMedicationRequest
    {
        [Required]
        public Guid PatientId { get; set; }
        
        [Required]
        [StringLength(255)]
        public string MedicationName { get; set; } = string.Empty;
        
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public DateTime? StartDate { get; set; }
        public string? PrescribedBy { get; set; }
        public string? Instructions { get; set; }
    }

    // Immunization DTOs
    public class ImmunizationDto
    {
        public Guid? Id { get; set; }
        public Guid PatientId { get; set; }
        public string VaccineName { get; set; } = string.Empty;
        public DateTime AdministrationDate { get; set; }
        public int? DoseNumber { get; set; }
        public int? SeriesCount { get; set; }
        public string? AdministeredBy { get; set; }
        public string? AdministeredAt { get; set; }
        public string? Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateImmunizationRequest
    {
        [Required]
        public Guid PatientId { get; set; }
        
        [Required]
        [StringLength(255)]
        public string VaccineName { get; set; } = string.Empty;
        
        [Required]
        public DateTime AdministrationDate { get; set; }
        
        public int? DoseNumber { get; set; }
        public int? SeriesCount { get; set; }
        public string? AdministeredBy { get; set; }
        public string? Notes { get; set; }
    }
}
