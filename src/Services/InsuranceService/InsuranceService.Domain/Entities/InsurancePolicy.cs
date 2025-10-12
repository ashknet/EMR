using Shared.Common.Models;
using System;

namespace InsuranceService.Domain.Entities
{
    /// <summary>
    /// Insurance policy entity - maps to FHIR Coverage resource
    /// Tracks patient insurance coverage and benefits
    /// </summary>
    public class InsurancePolicy : BaseEntity
    {
        public Guid PatientId { get; set; }
        
        // Policy Information (Encrypted)
        public string PolicyNumber { get; set; } = string.Empty;
        public string? GroupNumber { get; set; }
        public string? SubscriberId { get; set; }
        
        // Insurance Company
        public string InsuranceCompanyName { get; set; } = string.Empty;
        public string? InsuranceCompanyCode { get; set; }
        public string? CompanyPhone { get; set; }
        public string? CompanyWebsite { get; set; }
        
        // Policy Type
        public string PolicyType { get; set; } = "Medical";
        public string CoverageType { get; set; } = "Individual";
        public string? PlanName { get; set; }
        public string? PlanNetwork { get; set; }
        
        // Subscriber Information
        public bool IsSubscriber { get; set; } = true;
        public Guid? SubscriberPatientId { get; set; }
        public string? RelationshipToSubscriber { get; set; }
        
        // Coverage Period
        public DateTime EffectiveDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        
        // Coverage Details
        public decimal? AnnualDeductible { get; set; }
        public decimal DeductibleMet { get; set; }
        public decimal? OutOfPocketMax { get; set; }
        public decimal OutOfPocketMet { get; set; }
        public decimal? CopayPrimaryCare { get; set; }
        public decimal? CopaySpecialist { get; set; }
        public decimal? CopayEmergency { get; set; }
        public decimal? CoinsurancePercentage { get; set; }
        
        // Status
        public string Status { get; set; } = "active";
        public bool IsPrimary { get; set; } = true;
        public int Priority { get; set; } = 1;
        
        // Documents
        public string? InsuranceCardFrontUrl { get; set; }
        public string? InsuranceCardBackUrl { get; set; }
        public string? PolicyDocumentUrl { get; set; }
        
        // Verification
        public bool IsVerified { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? VerifiedBy { get; set; }
        public DateTime? LastEligibilityCheck { get; set; }
        
        // FHIR Compliance
        public string? FHIRResourceId { get; set; }
        public string? FHIRResourceJson { get; set; }
    }
}
