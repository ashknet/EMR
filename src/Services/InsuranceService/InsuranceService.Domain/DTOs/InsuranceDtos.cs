using System;
using System.ComponentModel.DataAnnotations;

namespace InsuranceService.Domain.DTOs
{
    public class InsurancePolicyDto
    {
        public Guid? Id { get; set; }
        public Guid PatientId { get; set; }
        public string PolicyNumber { get; set; } = string.Empty;
        public string? GroupNumber { get; set; }
        public string InsuranceCompanyName { get; set; } = string.Empty;
        public string PolicyType { get; set; } = "Medical";
        public string CoverageType { get; set; } = "Individual";
        public string? PlanName { get; set; }
        public string? PlanNetwork { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public decimal? AnnualDeductible { get; set; }
        public decimal DeductibleMet { get; set; }
        public decimal? OutOfPocketMax { get; set; }
        public decimal OutOfPocketMet { get; set; }
        public string Status { get; set; } = "active";
        public bool IsPrimary { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateInsurancePolicyRequest
    {
        [Required]
        public Guid PatientId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string PolicyNumber { get; set; } = string.Empty;
        
        public string? GroupNumber { get; set; }
        
        [Required]
        public string InsuranceCompanyName { get; set; } = string.Empty;
        
        public string PolicyType { get; set; } = "Medical";
        public string CoverageType { get; set; } = "Individual";
        public string? PlanName { get; set; }
        public string? PlanNetwork { get; set; }
        
        [Required]
        public DateTime EffectiveDate { get; set; }
        
        public decimal? AnnualDeductible { get; set; }
        public decimal? OutOfPocketMax { get; set; }
    }

    public class ClaimDto
    {
        public Guid? Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid InsurancePolicyId { get; set; }
        public string ClaimNumber { get; set; } = string.Empty;
        public string ClaimType { get; set; } = "professional";
        public DateTime ServiceDate { get; set; }
        public DateTime ClaimDate { get; set; }
        public decimal TotalCharges { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal? PatientResponsibility { get; set; }
        public string Status { get; set; } = "submitted";
        public string? ProviderName { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateClaimRequest
    {
        [Required]
        public Guid PatientId { get; set; }
        
        [Required]
        public Guid InsurancePolicyId { get; set; }
        
        [Required]
        public string ClaimNumber { get; set; } = string.Empty;
        
        [Required]
        public DateTime ServiceDate { get; set; }
        
        [Required]
        public decimal TotalCharges { get; set; }
        
        public string? ProviderName { get; set; }
        public string? PrimaryDiagnosisCode { get; set; }
    }
}
