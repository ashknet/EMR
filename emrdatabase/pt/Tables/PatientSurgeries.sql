CREATE TABLE [pt].[PatientSurgeries] (
    [SurgeryId]   INT              IDENTITY (1, 1) NOT NULL,
    [PatientId]   UNIQUEIDENTIFIER NOT NULL,
    [SurgeryType] NVARCHAR (200)   NOT NULL,
    [SurgeryDate] DATE             NULL,
    [Notes]       NVARCHAR (MAX)   NULL,
    [CreatedAt]   DATETIME2 (7)    DEFAULT (getutcdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([SurgeryId] ASC),
    CONSTRAINT [FK_PTPatientSurgeries_Patient] FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_PatientSurgeries_PatientId]
    ON [pt].[PatientSurgeries]([PatientId] ASC)
    INCLUDE([SurgeryType], [SurgeryDate], [Notes]);

