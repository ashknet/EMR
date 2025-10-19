CREATE TABLE [pt].[PatientChronicConditions] (
    [PatientId]     UNIQUEIDENTIFIER NOT NULL,
    [ConditionId]   UNIQUEIDENTIFIER NOT NULL,
    [DiagnosedDate] DATE             NULL,
    [Notes]         NVARCHAR (MAX)   NULL,
    [CreatedAt]     DATETIME2 (7)    DEFAULT (getutcdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([PatientId] ASC, [ConditionId] ASC),
    CONSTRAINT [FK_PTPatientChronicConditions_Condition] FOREIGN KEY ([ConditionId]) REFERENCES [pt].[Conditions] ([ConditionId]),
    CONSTRAINT [FK_PTPatientChronicConditions_Patient] FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_PatientChronicConditions_PatientId]
    ON [pt].[PatientChronicConditions]([PatientId] ASC)
    INCLUDE([ConditionId], [DiagnosedDate]);

