CREATE TABLE [pt].[InsurancePolicies] (
    [Id]                 INT              IDENTITY (1, 1) NOT NULL,
    [PatientId]          UNIQUEIDENTIFIER NOT NULL,
    [ProviderId]         INT              NOT NULL,
    [GroupNumber]        NVARCHAR (50)    NULL,
    [PolicyNumber]       NVARCHAR (50)    NULL,
    [PolicyHolderName]   NVARCHAR (150)   NULL,
    [ProviderPhone]      NVARCHAR (25)    NULL,
    [MedicareMedicaidId] NVARCHAR (50)    NULL,
    [CardImagePath]      NVARCHAR (260)   NULL,
    [CreatedAt]          DATETIME2 (7)    DEFAULT (getutcdate()) NOT NULL,
    [UpdatedAt]          DATETIME2 (7)    NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PTInsurancePolicies_Patient] FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_InsurancePolicies_PatientId]
    ON [pt].[InsurancePolicies]([PatientId] ASC)
    INCLUDE([ProviderId], [PolicyNumber], [GroupNumber]);

