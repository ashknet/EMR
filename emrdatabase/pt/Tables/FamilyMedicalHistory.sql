CREATE TABLE [pt].[FamilyMedicalHistory] (
    [FamilyHistoryId] INT              IDENTITY (1, 1) NOT NULL,
    [PatientId]       UNIQUEIDENTIFIER NOT NULL,
    [Relative]        NVARCHAR (100)   NOT NULL,
    [Condition]       NVARCHAR (200)   NOT NULL,
    [Notes]           NVARCHAR (MAX)   NULL,
    [CreatedAt]       DATETIME2 (7)    DEFAULT (getutcdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([FamilyHistoryId] ASC),
    CONSTRAINT [FK_PTFamilyMedicalHistory_Patient] FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_FamilyMedicalHistory_PatientId]
    ON [pt].[FamilyMedicalHistory]([PatientId] ASC)
    INCLUDE([Relative], [Condition], [Notes]);

