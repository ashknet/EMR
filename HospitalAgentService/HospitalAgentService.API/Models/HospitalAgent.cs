namespace HospitalAgentService.API.Models;

public class HospitalAgent
{
    public Guid AgentId { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public string HospitalName { get; set; } = string.Empty;
    public string? HospitalNpi { get; set; }
    public string? Department { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Status { get; set; } = "Active"; // Active, Inactive, Suspended
    public DateTime RegisteredAt { get; set; }
    public DateTime? LastHeartbeat { get; set; }
    public string? IpAddress { get; set; }
    public bool IsActive { get; set; } = true;
}

public class AgentSession
{
    public Guid SessionId { get; set; }
    public Guid AgentId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? IpAddress { get; set; }
    public virtual HospitalAgent Agent { get; set; } = null!;
}

public class IntakeRequest
{
    public Guid RequestId { get; set; }
    public Guid AgentId { get; set; }
    public string RequestType { get; set; } = string.Empty; // QR, Manual, Email, Fax
    public string? QrToken { get; set; }
    public string? PatientData { get; set; } // JSON
    public string Status { get; set; } = "Pending";
    public DateTime RequestedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ProcessedBy { get; set; }
    public virtual HospitalAgent Agent { get; set; } = null!;
}

public class TransferLog
{
    public Guid LogId { get; set; }
    public Guid AgentId { get; set; }
    public string TransferType { get; set; } = string.Empty; // Inbound, Outbound
    public string? TrackingNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime LogTime { get; set; }
    public string? Details { get; set; }
    public virtual HospitalAgent Agent { get; set; } = null!;
}

