CREATE TABLE [pt].[Allergies] (
    [AllergyId]                UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [PatientId]                UNIQUEIDENTIFIER NOT NULL,
    [AllergenName]             NVARCHAR (200)   NOT NULL,
    [AllergenType]             NVARCHAR (50)    NOT NULL,
    [AllergenCode]             NVARCHAR (50)    NULL,
    [CodeSystem]               NVARCHAR (100)   NULL,
    [Severity]                 NVARCHAR (50)    NOT NULL,
    [ClinicalStatus]           NVARCHAR (50)    DEFAULT ('Active') NULL,
    [Reaction]                 NVARCHAR (MAX)   NULL,
    [OnsetDate]                DATETIME2 (7)    NULL,
    [ReportedBy]               NVARCHAR (200)   NULL,
    [VerificationStatus]       NVARCHAR (50)    NULL,
    [FhirAllergyIntoleranceId] NVARCHAR (100)   NULL,
    [LastFhirSync]             DATETIME2 (7)    NULL,
    [CreatedAt]                DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [UpdatedAt]                DATETIME2 (7)    NULL,
    [CreatedBy]                NVARCHAR (100)   NOT NULL,
    [IsActive]                 BIT              DEFAULT ((1)) NULL,
    [Notes]                    NVARCHAR (MAX)   NULL,
    PRIMARY KEY CLUSTERED ([AllergyId] ASC),
    FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_PatientId]
    ON [pt].[Allergies]([PatientId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Allergies_PatientId_Active]
    ON [pt].[Allergies]([PatientId] ASC)
    INCLUDE([AllergenName], [Severity], [ClinicalStatus]);

