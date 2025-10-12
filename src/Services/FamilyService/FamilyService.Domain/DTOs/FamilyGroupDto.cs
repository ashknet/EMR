using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FamilyService.Domain.DTOs
{
    public class FamilyGroupDto
    {
        public Guid? Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string FamilyName { get; set; } = string.Empty;
        
        [Required]
        public Guid PrimaryAccountHolderId { get; set; }
        
        [EmailAddress]
        public string? PrimaryEmail { get; set; }
        
        [Phone]
        public string? PrimaryPhone { get; set; }
        
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        
        public bool IsActive { get; set; }
        public int MemberCount { get; set; }
        
        public List<FamilyMemberDto>? Members { get; set; }
        
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class FamilyMemberDto
    {
        public Guid? Id { get; set; }
        public Guid FamilyGroupId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string RelationshipType { get; set; } = string.Empty;
        public string? RelationshipToHead { get; set; }
        public bool IsHead { get; set; }
        public bool CanManageFamilyData { get; set; }
        public bool CanViewAllRecords { get; set; }
        public bool CanShareOnBehalf { get; set; }
        public bool IsActive { get; set; }
        public DateTime JoinedDate { get; set; }
    }

    public class CreateFamilyGroupRequest
    {
        [Required]
        public string FamilyName { get; set; } = string.Empty;
        
        [Required]
        public Guid PrimaryAccountHolderId { get; set; }
        
        [EmailAddress]
        public string? PrimaryEmail { get; set; }
        
        public string? AddressLine1 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
    }

    public class AddFamilyMemberRequest
    {
        [Required]
        public Guid FamilyGroupId { get; set; }
        
        [Required]
        public Guid PatientId { get; set; }
        
        [Required]
        public string RelationshipType { get; set; } = string.Empty;
        
        public string? RelationshipToHead { get; set; }
        public bool CanManageFamilyData { get; set; }
        public bool CanViewAllRecords { get; set; }
        public bool CanShareOnBehalf { get; set; }
    }

    public class ProxyAuthorizationDto
    {
        public Guid? Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid ProxyPatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string ProxyName { get; set; } = string.Empty;
        public string AuthorizationType { get; set; } = string.Empty;
        
        public bool CanViewMedicalRecords { get; set; }
        public bool CanUpdateDemographics { get; set; }
        public bool CanShareRecords { get; set; }
        public bool CanMakeMedicalDecisions { get; set; }
        public bool CanAccessInsurance { get; set; }
        public bool CanManageConsents { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsIndefinite { get; set; }
        
        public string? LegalDocumentUrl { get; set; }
        public string Status { get; set; } = "Active";
        
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateProxyAuthorizationRequest
    {
        [Required]
        public Guid PatientId { get; set; }
        
        [Required]
        public Guid ProxyPatientId { get; set; }
        
        [Required]
        public string AuthorizationType { get; set; } = string.Empty;
        
        public bool CanViewMedicalRecords { get; set; }
        public bool CanUpdateDemographics { get; set; }
        public bool CanShareRecords { get; set; }
        public bool CanMakeMedicalDecisions { get; set; }
        public bool CanAccessInsurance { get; set; }
        public bool CanManageConsents { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsIndefinite { get; set; }
        
        public string? LegalDocumentUrl { get; set; }
    }
}
