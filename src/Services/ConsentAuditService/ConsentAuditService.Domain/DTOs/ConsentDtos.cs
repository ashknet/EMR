using System;
using System.ComponentModel.DataAnnotations;

namespace ConsentAuditService.Domain.DTOs
{
    public class ConsentDto
    {
        public Guid? Id { get; set; }
        public Guid PatientId { get; set; }
        public string ConsentType { get; set; } = string.Empty;
        public string ConsentScope { get; set; } = string.Empty;
        public string Status { get; set; } = "active";
        public DateTime EffectiveDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool CanView { get; set; }
        public bool CanShare { get; set; }
        public bool CanExport { get; set; }
        public string? GrantedToName { get; set; }
        public string? GrantedToOrganization { get; set; }
        public bool IsRevoked { get; set; }
        public string? QRCodeData { get; set; }
        public DateTime? QRCodeExpiration { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CreateConsentRequest
    {
        [Required]
        public Guid PatientId { get; set; }
        
        [Required]
        public string ConsentType { get; set; } = string.Empty;
        
        [Required]
        public string ConsentScope { get; set; } = string.Empty;
        
        public DateTime? ExpirationDate { get; set; }
        public bool CanView { get; set; } = true;
        public bool CanShare { get; set; }
        public bool CanExport { get; set; }
        public string? GrantedToOrganization { get; set; }
        public bool GenerateQRCode { get; set; }
    }

    public class AuditLogDto
    {
        public Guid Id { get; set; }
        public DateTime EventTimestamp { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string EventAction { get; set; } = string.Empty;
        public string EventOutcome { get; set; } = string.Empty;
        public string ActorId { get; set; } = string.Empty;
        public string? ActorName { get; set; }
        public string TargetType { get; set; } = string.Empty;
        public Guid? PatientId { get; set; }
        public string? IPAddress { get; set; }
        public bool IsAuthorized { get; set; }
        public bool IsAnomaly { get; set; }
    }

    public class DataSharingEventDto
    {
        public Guid? Id { get; set; }
        public Guid PatientId { get; set; }
        public string ShareMethod { get; set; } = string.Empty;
        public string SharedWith { get; set; } = string.Empty;
        public DateTime ShareInitiatedAt { get; set; }
        public DateTime? ShareExpiresAt { get; set; }
        public string Status { get; set; } = "active";
        public int AccessCount { get; set; }
        public string? QRCodeToken { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
