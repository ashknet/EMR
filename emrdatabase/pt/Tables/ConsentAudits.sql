CREATE TABLE [pt].[ConsentAudits] (
    [AuditId]           UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [ConsentId]         UNIQUEIDENTIFIER NOT NULL,
    [Action]            NVARCHAR (100)   NOT NULL,
    [ActionDetails]     NVARCHAR (MAX)   NULL,
    [ActionDate]        DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [ActorId]           NVARCHAR (100)   NOT NULL,
    [ActorName]         NVARCHAR (200)   NOT NULL,
    [ActorRole]         NVARCHAR (100)   NULL,
    [ActorOrganization] NVARCHAR (200)   NULL,
    [IpAddress]         NVARCHAR (50)    NULL,
    [UserAgent]         NVARCHAR (500)   NULL,
    [SessionId]         NVARCHAR (100)   NULL,
    [DataTypesAccessed] NVARCHAR (MAX)   NULL,
    [ResourcesAccessed] NVARCHAR (MAX)   NULL,
    PRIMARY KEY CLUSTERED ([AuditId] ASC),
    FOREIGN KEY ([ConsentId]) REFERENCES [pt].[Consents] ([ConsentId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ActionDate]
    ON [pt].[ConsentAudits]([ActionDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ConsentId]
    ON [pt].[ConsentAudits]([ConsentId] ASC);

