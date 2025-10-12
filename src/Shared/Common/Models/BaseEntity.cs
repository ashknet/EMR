using System;

namespace Shared.Common.Models
{
    /// <summary>
    /// Base entity class for all domain entities with audit fields
    /// Ensures HIPAA compliance with tracking of all data modifications
    /// </summary>
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }
        
        /// <summary>
        /// Row version for optimistic concurrency control
        /// </summary>
        public byte[]? RowVersion { get; set; }
    }
}
