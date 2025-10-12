using Shared.Common.Models;
using System;

namespace PatientService.Domain.Entities
{
    /// <summary>
    /// Patient notes for tracking important information, reminders, etc.
    /// </summary>
    public class PatientNote : BaseEntity
    {
        public Guid PatientId { get; set; }
        public string NoteType { get; set; } = string.Empty; // Personal, Medical, Administrative
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsImportant { get; set; }
        public bool IsPrivate { get; set; } = true;
        public string? Tags { get; set; } // Comma-separated tags
        public DateTime? ReminderDate { get; set; }
        public bool IsReminderSent { get; set; }
        
        // Navigation
        public virtual Patient? Patient { get; set; }
    }
}
