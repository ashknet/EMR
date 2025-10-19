CREATE TABLE [ag].[TransferLogs] (
    [LogId]          UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [AgentId]        UNIQUEIDENTIFIER NOT NULL,
    [TransferType]   NVARCHAR (50)    NOT NULL,
    [TrackingNumber] NVARCHAR (50)    NULL,
    [Status]         NVARCHAR (50)    NOT NULL,
    [LogTime]        DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [Details]        NVARCHAR (MAX)   NULL,
    PRIMARY KEY CLUSTERED ([LogId] ASC),
    FOREIGN KEY ([AgentId]) REFERENCES [ag].[HospitalAgents] ([AgentId])
);


GO
CREATE NONCLUSTERED INDEX [IX_TrackingNumber]
    ON [ag].[TransferLogs]([TrackingNumber] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_AgentId]
    ON [ag].[TransferLogs]([AgentId] ASC);

