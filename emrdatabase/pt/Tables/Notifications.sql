CREATE TABLE [pt].[Notifications] (
    [NotificationId]    UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [PatientId]         UNIQUEIDENTIFIER NOT NULL,
    [NotificationType]  NVARCHAR (50)    NOT NULL,
    [Category]          NVARCHAR (100)   NOT NULL,
    [Title]             NVARCHAR (500)   NOT NULL,
    [Message]           NVARCHAR (MAX)   NOT NULL,
    [ActionUrl]         NVARCHAR (500)   NULL,
    [ActionLabel]       NVARCHAR (100)   NULL,
    [Priority]          NVARCHAR (50)    DEFAULT ('Normal') NULL,
    [IsRead]            BIT              DEFAULT ((0)) NULL,
    [ReadAt]            DATETIME2 (7)    NULL,
    [IsArchived]        BIT              DEFAULT ((0)) NULL,
    [ArchivedAt]        DATETIME2 (7)    NULL,
    [SendEmail]         BIT              DEFAULT ((0)) NULL,
    [EmailSent]         BIT              DEFAULT ((0)) NULL,
    [EmailSentAt]       DATETIME2 (7)    NULL,
    [SendSms]           BIT              DEFAULT ((0)) NULL,
    [SmsSent]           BIT              DEFAULT ((0)) NULL,
    [SmsSentAt]         DATETIME2 (7)    NULL,
    [ScheduledFor]      DATETIME2 (7)    NULL,
    [ExpiresAt]         DATETIME2 (7)    NULL,
    [RelatedEntityType] NVARCHAR (100)   NULL,
    [RelatedEntityId]   UNIQUEIDENTIFIER NULL,
    [CreatedAt]         DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [CreatedBy]         NVARCHAR (100)   NOT NULL,
    PRIMARY KEY CLUSTERED ([NotificationId] ASC),
    FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_PatientId_IsRead]
    ON [pt].[Notifications]([PatientId] ASC, [IsRead] ASC);

