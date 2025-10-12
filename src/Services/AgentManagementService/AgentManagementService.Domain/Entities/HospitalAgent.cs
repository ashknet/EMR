using Shared.Common.Models;
using System;

namespace AgentManagementService.Domain.Entities
{
    public class HospitalAgent : BaseEntity
    {
        public string HospitalName { get; set; } = string.Empty;
        public string? HospitalNPI { get; set; }
        public string? HospitalAddress { get; set; }
        public string AgentVersion { get; set; } = string.Empty;
        public DateTime InstallationDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdateDate { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public string? MachineName { get; set; }
        public string? IPAddress { get; set; }
        public string? WindowsVersion { get; set; }
        public string Status { get; set; } = "Active";
        public bool IsOnline { get; set; }
        public DateTime? LastHeartbeat { get; set; }
        public DateTime? LastActivityAt { get; set; }
        public string? ApiKey { get; set; }
        public string? CertificateThumbprint { get; set; }
        public int TotalScansProcessed { get; set; }
        public int TotalDocumentsIngested { get; set; }
        public int TotalTransfersCompleted { get; set; }
    }
}
