CREATE TABLE [ag].[HospitalAgents] (
    [AgentId]       UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [MachineName]   NVARCHAR (200)   NOT NULL,
    [HospitalName]  NVARCHAR (300)   NOT NULL,
    [HospitalNpi]   NVARCHAR (20)    NULL,
    [Department]    NVARCHAR (200)   NULL,
    [Version]       NVARCHAR (50)    NOT NULL,
    [Status]        NVARCHAR (50)    DEFAULT ('Active') NULL,
    [RegisteredAt]  DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [LastHeartbeat] DATETIME2 (7)    NULL,
    [IpAddress]     NVARCHAR (50)    NULL,
    [IsActive]      BIT              DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([AgentId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_HospitalName]
    ON [ag].[HospitalAgents]([HospitalName] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MachineName]
    ON [ag].[HospitalAgents]([MachineName] ASC);

