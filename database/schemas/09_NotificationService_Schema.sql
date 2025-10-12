-- =============================================
-- Healthcare Platform - Notification Service Database Schema
-- Alerts, reminders, and notification management
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'notification')
BEGIN
    EXEC('CREATE SCHEMA notification')
END
GO

CREATE TABLE notification.Notifications (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    
    -- Recipient
    PatientId UNIQUEIDENTIFIER NOT NULL,
    RecipientEmail NVARCHAR(255) NULL,
    RecipientPhone NVARCHAR(50) NULL,
    
    -- Notification Information
    NotificationType NVARCHAR(50) NOT NULL, -- Reminder, Alert, Information, Emergency
    Subject NVARCHAR(255) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    Priority NVARCHAR(20) DEFAULT 'Normal', -- Low, Normal, High, Urgent
    
    -- Delivery Channels
    SendEmail BIT DEFAULT 1,
    SendSMS BIT DEFAULT 0,
    SendPush BIT DEFAULT 0,
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'Pending', -- Pending, Sent, Delivered, Failed, Cancelled
    ScheduledFor DATETIME2 NULL,
    SentAt DATETIME2 NULL,
    DeliveredAt DATETIME2 NULL,
    ReadAt DATETIME2 NULL,
    
    -- Error Handling
    ErrorMessage NVARCHAR(MAX) NULL,
    RetryCount INT DEFAULT 0,
    
    -- Related Entity
    RelatedEntityType NVARCHAR(100) NULL,
    RelatedEntityId UNIQUEIDENTIFIER NULL,
    
    INDEX IX_Notifications_PatientId (PatientId),
    INDEX IX_Notifications_Status (Status),
    INDEX IX_Notifications_ScheduledFor (ScheduledFor)
)
GO
