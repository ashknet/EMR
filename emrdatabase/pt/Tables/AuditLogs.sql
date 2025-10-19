CREATE TABLE [pt].[AuditLogs] (
    [AuditLogId]         UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [PatientId]          UNIQUEIDENTIFIER NULL,
    [EventType]          NVARCHAR (100)   NOT NULL,
    [EntityType]         NVARCHAR (100)   NOT NULL,
    [EntityId]           UNIQUEIDENTIFIER NULL,
    [EventTimestamp]     DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [ActorId]            NVARCHAR (100)   NOT NULL,
    [ActorName]          NVARCHAR (200)   NOT NULL,
    [ActorRole]          NVARCHAR (100)   NULL,
    [ActorEmail]         NVARCHAR (255)   NULL,
    [Action]             NVARCHAR (200)   NOT NULL,
    [ActionDescription]  NVARCHAR (MAX)   NULL,
    [OldValues]          NVARCHAR (MAX)   NULL,
    [NewValues]          NVARCHAR (MAX)   NULL,
    [IpAddress]          NVARCHAR (50)    NULL,
    [UserAgent]          NVARCHAR (500)   NULL,
    [SessionId]          NVARCHAR (100)   NULL,
    [RequestId]          NVARCHAR (100)   NULL,
    [CorrelationId]      NVARCHAR (100)   NULL,
    [Success]            BIT              DEFAULT ((1)) NULL,
    [ErrorMessage]       NVARCHAR (MAX)   NULL,
    [ResponseCode]       INT              NULL,
    [AccessReason]       NVARCHAR (500)   NULL,
    [ConsentId]          NVARCHAR (100)   NULL,
    [PhiAccessed]        BIT              DEFAULT ((0)) NULL,
    [DataClassification] NVARCHAR (50)    NULL,
    [ApplicationName]    NVARCHAR (100)   NULL,
    [Environment]        NVARCHAR (50)    NULL,
    PRIMARY KEY CLUSTERED ([AuditLogId] ASC),
    FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId])
);


GO
CREATE NONCLUSTERED INDEX [IX_EventType_EntityType]
    ON [pt].[AuditLogs]([EventType] ASC, [EntityType] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_EventTimestamp]
    ON [pt].[AuditLogs]([EventTimestamp] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PatientId]
    ON [pt].[AuditLogs]([PatientId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AuditLogs_PatientId_Timestamp]
    ON [pt].[AuditLogs]([PatientId] ASC, [EventTimestamp] DESC)
    INCLUDE([EventType], [Action], [ActorName]);


GO
CREATE NONCLUSTERED INDEX [IX_AuditLogs_EventType]
    ON [pt].[AuditLogs]([EventType] ASC, [EventTimestamp] DESC);

