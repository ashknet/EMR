using Shared.Common.Models;
using System;
using System.Collections.Generic;

namespace FamilyService.Domain.Entities
{
    /// <summary>
    /// Family group representing a household
    /// Manages family-wide settings and relationships
    /// </summary>
    public class FamilyGroup : BaseEntity
    {
        public string FamilyName { get; set; } = string.Empty;
        public Guid PrimaryAccountHolderId { get; set; }
        
        // Contact Information
        public string? PrimaryEmail { get; set; }
        public string? PrimaryPhone { get; set; }
        
        // Address
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string Country { get; set; } = "USA";
        
        // Status
        public bool IsActive { get; set; } = true;
        
        // Navigation Properties
        public virtual ICollection<FamilyMember> Members { get; set; } = new List<FamilyMember>();
    }
}
