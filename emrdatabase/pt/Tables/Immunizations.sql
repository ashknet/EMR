CREATE TABLE [pt].[Immunizations] (
    [ImmunizationId]     UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [PatientId]          UNIQUEIDENTIFIER NOT NULL,
    [VaccineName]        NVARCHAR (200)   NOT NULL,
    [VaccineCode]        NVARCHAR (50)    NULL,
    [CodeSystem]         NVARCHAR (100)   NULL,
    [LotNumber]          NVARCHAR (50)    NULL,
    [ExpirationDate]     DATETIME2 (7)    NULL,
    [AdministeredDate]   DATETIME2 (7)    NOT NULL,
    [AdministeredBy]     NVARCHAR (200)   NULL,
    [Site]               NVARCHAR (100)   NULL,
    [Route]              NVARCHAR (100)   NULL,
    [DoseQuantity]       NVARCHAR (50)    NULL,
    [DoseNumber]         INT              NULL,
    [SeriesDoses]        INT              NULL,
    [Status]             NVARCHAR (50)    DEFAULT ('Completed') NULL,
    [StatusReason]       NVARCHAR (200)   NULL,
    [ProviderName]       NVARCHAR (200)   NULL,
    [FacilityName]       NVARCHAR (200)   NULL,
    [FhirImmunizationId] NVARCHAR (100)   NULL,
    [LastFhirSync]       DATETIME2 (7)    NULL,
    [CreatedAt]          DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [UpdatedAt]          DATETIME2 (7)    NULL,
    [CreatedBy]          NVARCHAR (100)   NOT NULL,
    [IsActive]           BIT              DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([ImmunizationId] ASC),
    FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);


GO


GO


GO
CREATE NONCLUSTERED INDEX [IX_PatientId]
    ON [pt].[Immunizations]([PatientId] ASC);

