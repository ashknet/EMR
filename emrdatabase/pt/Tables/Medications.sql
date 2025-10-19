CREATE TABLE [pt].[Medications] (
    [MedicationId]   UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [PatientId]      UNIQUEIDENTIFIER NOT NULL,
    [MedicationName] NVARCHAR (200)   NOT NULL,
    [Dosage]         NVARCHAR (100)   NULL,
    [Frequency]      NVARCHAR (100)   NULL,
    [Prescriber]     NVARCHAR (200)   NULL,
    [StartDate]      DATETIME2 (7)    NULL,
    [EndDate]        DATETIME2 (7)    NULL,
    [IsActive]       BIT              DEFAULT ((1)) NULL,
    [Notes]          NVARCHAR (MAX)   NULL,
    [CreatedAt]      DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [UpdatedAt]      DATETIME2 (7)    NULL,
    [CreatedBy]      NVARCHAR (100)   NOT NULL,
    PRIMARY KEY CLUSTERED ([MedicationId] ASC),
    FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Medications_PatientId]
    ON [pt].[Medications]([PatientId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Medications_PatientId_Active]
    ON [pt].[Medications]([PatientId] ASC, [IsActive] ASC)
    INCLUDE([MedicationName], [Dosage], [Frequency]);

