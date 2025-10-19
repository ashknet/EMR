CREATE TABLE [pt].[ChronicConditions] (
    [ConditionId]   UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [PatientId]     UNIQUEIDENTIFIER NOT NULL,
    [ConditionName] NVARCHAR (200)   NOT NULL,
    [DiagnosedDate] DATE             NULL,
    [Status]        NVARCHAR (50)    DEFAULT ('Active') NOT NULL,
    [Notes]         NVARCHAR (MAX)   NULL,
    [CreatedAt]     DATETIME2 (7)    DEFAULT (getutcdate()) NOT NULL,
    [UpdatedAt]     DATETIME2 (7)    NULL,
    [CreatedBy]     NVARCHAR (100)   DEFAULT (suser_sname()) NOT NULL,
    [UpdatedBy]     NVARCHAR (100)   NULL,
    [IsActive]      BIT              DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_ChronicConditions] PRIMARY KEY CLUSTERED ([ConditionId] ASC),
    CONSTRAINT [FK_ChronicConditions_Patient] FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ChronicConditions_PatientId]
    ON [pt].[ChronicConditions]([PatientId] ASC)
    INCLUDE([ConditionName], [Status], [IsActive]);

