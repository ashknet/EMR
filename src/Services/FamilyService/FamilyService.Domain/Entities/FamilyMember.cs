using Shared.Common.Models;
using System;

namespace FamilyService.Domain.Entities
{
    /// <summary>
    /// Link between patient and family group with relationship info
    /// </summary>
    public class FamilyMember : BaseEntity
    {
        public Guid FamilyGroupId { get; set; }
        public Guid PatientId { get; set; }
        
        // Relationship Information
        public string RelationshipType { get; set; } = string.Empty; // Self, Spouse, Child, Parent, Sibling, Guardian, Other
        public string? RelationshipToHead { get; set; }
        
        // Role and Permissions
        public bool IsHead { get; set; }
        public bool CanManageFamilyData { get; set; }
        public bool CanViewAllRecords { get; set; }
        public bool CanShareOnBehalf { get; set; }
        
        // Status
        public bool IsActive { get; set; } = true;
        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LeftDate { get; set; }
        
        // Navigation
        public virtual FamilyGroup? FamilyGroup { get; set; }
    }
}
