CREATE TABLE [pt].[PatientHospitalizations] (
    [HospitalizationId] INT              IDENTITY (1, 1) NOT NULL,
    [PatientId]         UNIQUEIDENTIFIER NOT NULL,
    [HospitalName]      NVARCHAR (200)   NULL,
    [Reason]            NVARCHAR (500)   NULL,
    [AdmissionDate]     DATE             NULL,
    [DischargeDate]     DATE             NULL,
    [Notes]             NVARCHAR (MAX)   NULL,
    [CreatedAt]         DATETIME2 (7)    DEFAULT (getutcdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([HospitalizationId] ASC),
    CONSTRAINT [FK_PTPatientHospitalizations_Patient] FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_PatientHospitalizations_PatientId]
    ON [pt].[PatientHospitalizations]([PatientId] ASC)
    INCLUDE([HospitalName], [Reason], [AdmissionDate], [DischargeDate]);

