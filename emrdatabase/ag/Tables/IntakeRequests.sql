CREATE TABLE [ag].[IntakeRequests] (
    [RequestId]   UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [AgentId]     UNIQUEIDENTIFIER NOT NULL,
    [RequestType] NVARCHAR (50)    NOT NULL,
    [QrToken]     NVARCHAR (200)   NULL,
    [PatientData] NVARCHAR (MAX)   NULL,
    [Status]      NVARCHAR (50)    DEFAULT ('Pending') NULL,
    [RequestedAt] DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [ProcessedAt] DATETIME2 (7)    NULL,
    [ProcessedBy] NVARCHAR (200)   NULL,
    PRIMARY KEY CLUSTERED ([RequestId] ASC),
    FOREIGN KEY ([AgentId]) REFERENCES [ag].[HospitalAgents] ([AgentId])
);


GO
CREATE NONCLUSTERED INDEX [IX_Status]
    ON [ag].[IntakeRequests]([Status] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AgentId]
    ON [ag].[IntakeRequests]([AgentId] ASC);

