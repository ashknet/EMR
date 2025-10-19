CREATE TABLE [ag].[AgentSessions] (
    [SessionId] UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [AgentId]   UNIQUEIDENTIFIER NOT NULL,
    [UserName]  NVARCHAR (200)   NOT NULL,
    [StartTime] DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [EndTime]   DATETIME2 (7)    NULL,
    [IpAddress] NVARCHAR (50)    NULL,
    PRIMARY KEY CLUSTERED ([SessionId] ASC),
    FOREIGN KEY ([AgentId]) REFERENCES [ag].[HospitalAgents] ([AgentId])
);


GO
CREATE NONCLUSTERED INDEX [IX_AgentId]
    ON [ag].[AgentSessions]([AgentId] ASC);

