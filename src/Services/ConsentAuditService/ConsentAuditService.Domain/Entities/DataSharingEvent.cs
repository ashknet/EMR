using Shared.Common.Models;
using System;

namespace ConsentAuditService.Domain.Entities
{
    /// <summary>
    /// Data sharing event - tracks QR code sharing and direct transfers
    /// </summary>
    public class DataSharingEvent : BaseEntity
    {
        public Guid PatientId { get; set; }
        public Guid? ConsentId { get; set; }
        
        // Sharing Details
        public string ShareMethod { get; set; } = string.Empty;
        public string SharedWith { get; set; } = string.Empty;
        public string? SharedWithType { get; set; }
        
        // What was shared
        public string? DataShared { get; set; }
        public string? ShareScope { get; set; }
        
        // Time window
        public DateTime ShareInitiatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ShareAccessedAt { get; set; }
        public DateTime? ShareExpiresAt { get; set; }
        
        // Status
        public string Status { get; set; } = "active";
        public int AccessCount { get; set; }
        public int? MaxAccessCount { get; set; }
        
        // QR Code
        public string? QRCodeToken { get; set; }
        public string? QRCodeScannedBy { get; set; }
        public DateTime? QRCodeScannedAt { get; set; }
        
        // Location
        public string? SharedAtLocation { get; set; }
        public string? IPAddress { get; set; }
        
        // Purpose
        public string? SharePurpose { get; set; }
    }
}
