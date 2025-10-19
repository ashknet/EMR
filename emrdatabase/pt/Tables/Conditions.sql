CREATE TABLE [pt].[Conditions] (
    [ConditionId]        UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [PatientId]          UNIQUEIDENTIFIER NOT NULL,
    [ConditionName]      NVARCHAR (300)   NOT NULL,
    [ConditionCode]      NVARCHAR (50)    NULL,
    [CodeSystem]         NVARCHAR (100)   NULL,
    [ClinicalStatus]     NVARCHAR (50)    DEFAULT ('Active') NULL,
    [VerificationStatus] NVARCHAR (50)    NULL,
    [Severity]           NVARCHAR (50)    NULL,
    [Category]           NVARCHAR (100)   NULL,
    [OnsetDate]          DATETIME2 (7)    NULL,
    [AbatementDate]      DATETIME2 (7)    NULL,
    [RecordedDate]       DATETIME2 (7)    NULL,
    [Notes]              NVARCHAR (MAX)   NULL,
    [FhirConditionId]    NVARCHAR (100)   NULL,
    [LastFhirSync]       DATETIME2 (7)    NULL,
    [CreatedAt]          DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [UpdatedAt]          DATETIME2 (7)    NULL,
    [CreatedBy]          NVARCHAR (100)   NOT NULL,
    [IsActive]           BIT              DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([ConditionId] ASC),
    FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_PatientId]
    ON [pt].[Conditions]([PatientId] ASC);

