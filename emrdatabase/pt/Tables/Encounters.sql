CREATE TABLE [pt].[Encounters] (
    [EncounterId]                 UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [PatientId]                   UNIQUEIDENTIFIER NOT NULL,
    [EncounterType]               NVARCHAR (100)   NOT NULL,
    [EncounterClass]              NVARCHAR (50)    NULL,
    [Status]                      NVARCHAR (50)    DEFAULT ('Planned') NULL,
    [PlannedStartDate]            DATETIME2 (7)    NULL,
    [ActualStartDate]             DATETIME2 (7)    NULL,
    [EndDate]                     DATETIME2 (7)    NULL,
    [DurationMinutes]             INT              NULL,
    [ProviderId]                  NVARCHAR (100)   NULL,
    [ProviderName]                NVARCHAR (200)   NULL,
    [ProviderSpecialty]           NVARCHAR (100)   NULL,
    [ProviderNpi]                 NVARCHAR (20)    NULL,
    [FacilityId]                  NVARCHAR (100)   NULL,
    [FacilityName]                NVARCHAR (200)   NULL,
    [Department]                  NVARCHAR (100)   NULL,
    [Room]                        NVARCHAR (50)    NULL,
    [ChiefComplaint]              NVARCHAR (500)   NULL,
    [ReasonForVisit]              NVARCHAR (500)   NULL,
    [ReasonCode]                  NVARCHAR (50)    NULL,
    [Priority]                    NVARCHAR (50)    NULL,
    [PrimaryDiagnosisCode]        NVARCHAR (50)    NULL,
    [PrimaryDiagnosisDescription] NVARCHAR (500)   NULL,
    [SecondaryDiagnoses]          NVARCHAR (MAX)   NULL,
    [DischargeDisposition]        NVARCHAR (100)   NULL,
    [DischargeInstructions]       NVARCHAR (MAX)   NULL,
    [BillingCode]                 NVARCHAR (50)    NULL,
    [EstimatedCost]               DECIMAL (18, 2)  NULL,
    [ClinicalNotes]               NVARCHAR (MAX)   NULL,
    [FhirEncounterId]             NVARCHAR (100)   NULL,
    [LastFhirSync]                DATETIME2 (7)    NULL,
    [CreatedAt]                   DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [UpdatedAt]                   DATETIME2 (7)    NULL,
    [CreatedBy]                   NVARCHAR (100)   NOT NULL,
    [IsActive]                    BIT              DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([EncounterId] ASC),
    FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ActualStartDate]
    ON [pt].[Encounters]([ActualStartDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PatientId]
    ON [pt].[Encounters]([PatientId] ASC);

