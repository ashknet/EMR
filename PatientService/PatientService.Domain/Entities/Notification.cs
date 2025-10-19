namespace PatientService.Domain.Entities;

/// <summary>
/// Represents system notifications for patients
/// </summary>
public class Notification
{
    public Guid NotificationId { get; set; }
    public Guid PatientId { get; set; }
    
    // Notification Details
    public string NotificationType { get; set; } = string.Empty; // Reminder, Alert, Info, Error
    public string Category { get; set; } = string.Empty; // Appointment, Insurance, Consent, Transfer, Document
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? ActionUrl { get; set; }
    public string? ActionLabel { get; set; }
    
    // Priority
    public string Priority { get; set; } = "Normal"; // Low, Normal, High, Urgent
    
    // Status
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public bool IsArchived { get; set; } = false;
    public DateTime? ArchivedAt { get; set; }
    
    // Delivery
    public bool SendEmail { get; set; } = false;
    public bool EmailSent { get; set; } = false;
    public DateTime? EmailSentAt { get; set; }
    public bool SendSms { get; set; } = false;
    public bool SmsSent { get; set; } = false;
    public DateTime? SmsSentAt { get; set; }
    
    // Scheduling
    public DateTime? ScheduledFor { get; set; }
    public DateTime? ExpiresAt { get; set; }
    
    // Related Entities
    public string? RelatedEntityType { get; set; } // Encounter, Transfer, Consent, etc.
    public Guid? RelatedEntityId { get; set; }
    
    // System Fields
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    
    // Navigation Properties
    public virtual Patient Patient { get; set; } = null!;
}

