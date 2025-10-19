CREATE TABLE [pt].[TransferAudits] (
    [AuditId]       UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [TransferId]    UNIQUEIDENTIFIER NOT NULL,
    [Action]        NVARCHAR (100)   NOT NULL,
    [ActionDetails] NVARCHAR (MAX)   NULL,
    [ActionDate]    DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [ActorId]       NVARCHAR (100)   NULL,
    [ActorName]     NVARCHAR (200)   NULL,
    [ActorRole]     NVARCHAR (100)   NULL,
    [IpAddress]     NVARCHAR (50)    NULL,
    [SessionId]     NVARCHAR (100)   NULL,
    [ErrorMessage]  NVARCHAR (MAX)   NULL,
    [StackTrace]    NVARCHAR (MAX)   NULL,
    PRIMARY KEY CLUSTERED ([AuditId] ASC),
    FOREIGN KEY ([TransferId]) REFERENCES [pt].[RecordTransfers] ([TransferId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_TransferId]
    ON [pt].[TransferAudits]([TransferId] ASC);

