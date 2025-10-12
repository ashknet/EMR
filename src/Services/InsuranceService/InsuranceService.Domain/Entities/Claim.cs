using Shared.Common.Models;
using System;

namespace InsuranceService.Domain.Entities
{
    /// <summary>
    /// Insurance claim entity - maps to FHIR Claim resource
    /// Tracks insurance claims and their status
    /// </summary>
    public class Claim : BaseEntity
    {
        public Guid PatientId { get; set; }
        public Guid InsurancePolicyId { get; set; }
        
        // Claim Information
        public string ClaimNumber { get; set; } = string.Empty;
        public string ClaimType { get; set; } = "professional";
        
        // Dates
        public DateTime ServiceDate { get; set; }
        public DateTime ClaimDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
        
        // Provider Information
        public string? ProviderName { get; set; }
        public string? ProviderNPI { get; set; }
        public string? FacilityName { get; set; }
        public string? FacilityNPI { get; set; }
        
        // Financial Information
        public decimal TotalCharges { get; set; }
        public decimal? AllowedAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? PatientResponsibility { get; set; }
        public decimal? Deductible { get; set; }
        public decimal? Coinsurance { get; set; }
        public decimal? Copay { get; set; }
        
        // Status
        public string Status { get; set; } = "submitted";
        public string? DenialReason { get; set; }
        public string? DenialCode { get; set; }
        
        // Diagnosis and Services
        public string? PrimaryDiagnosisCode { get; set; }
        public string? PrimaryDiagnosisDescription { get; set; }
        public string? ProcedureCodes { get; set; }
        
        // EOB
        public string? EOBDocumentUrl { get; set; }
        public DateTime? EOBReceivedDate { get; set; }
        
        // Appeals
        public bool IsAppealed { get; set; }
        public DateTime? AppealDate { get; set; }
        public string? AppealStatus { get; set; }
        public string? AppealOutcome { get; set; }
        
        // FHIR Compliance
        public string? FHIRResourceId { get; set; }
        public string? FHIRResourceJson { get; set; }
        
        // Navigation
        public virtual InsurancePolicy? InsurancePolicy { get; set; }
    }
}
