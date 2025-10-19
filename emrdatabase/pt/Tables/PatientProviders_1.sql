CREATE TABLE [pt].[PatientProviders] (
    [PatientProviderId]   UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [PatientId]           UNIQUEIDENTIFIER NOT NULL,
    [ProviderTypeId]      INT              NOT NULL,
    [ProviderTypeName]    NVARCHAR (100)   NOT NULL,
    [ProviderName]        NVARCHAR (200)   NOT NULL,
    [Specialty]           NVARCHAR (200)   NULL,
    [NPI]                 NVARCHAR (10)    NULL,
    [Phone]               NVARCHAR (25)    NULL,
    [Fax]                 NVARCHAR (25)    NULL,
    [Email]               NVARCHAR (100)   NULL,
    [AddressLine1]        NVARCHAR (200)   NULL,
    [AddressLine2]        NVARCHAR (200)   NULL,
    [City]                NVARCHAR (100)   NULL,
    [State]               NVARCHAR (50)    NULL,
    [ZipCode]             NVARCHAR (20)    NULL,
    [Country]             NVARCHAR (100)   DEFAULT ('USA') NULL,
    [PracticeName]        NVARCHAR (200)   NULL,
    [Website]             NVARCHAR (500)   NULL,
    [Notes]               NVARCHAR (MAX)   NULL,
    [IsPrimary]           BIT              DEFAULT ((0)) NOT NULL,
    [IsAcceptingPatients] BIT              DEFAULT ((1)) NOT NULL,
    [Status]              NVARCHAR (50)    DEFAULT ('Active') NOT NULL,
    [FirstVisitDate]      DATETIME2 (7)    NULL,
    [LastVisitDate]       DATETIME2 (7)    NULL,
    [FhirPractitionerId]  NVARCHAR (100)   NULL,
    [FhirOrganizationId]  NVARCHAR (100)   NULL,
    [CreatedAt]           DATETIME2 (7)    DEFAULT (getutcdate()) NOT NULL,
    [UpdatedAt]           DATETIME2 (7)    NULL,
    [CreatedBy]           NVARCHAR (100)   DEFAULT ('system') NOT NULL,
    [UpdatedBy]           NVARCHAR (100)   NULL,
    [IsActive]            BIT              DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_PatientProviders] PRIMARY KEY CLUSTERED ([PatientProviderId] ASC),
    CONSTRAINT [FK_PatientProviders_Patient] FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE,
    CONSTRAINT [FK_PatientProviders_ProviderType] FOREIGN KEY ([ProviderTypeId]) REFERENCES [pt].[ProviderTypes] ([ProviderTypeId])
);


GO
CREATE NONCLUSTERED INDEX [IX_PatientProviders_NPI]
    ON [pt].[PatientProviders]([NPI] ASC) WHERE ([NPI] IS NOT NULL);


GO
CREATE NONCLUSTERED INDEX [IX_PatientProviders_ProviderType]
    ON [pt].[PatientProviders]([ProviderTypeId] ASC, [PatientId] ASC)
    INCLUDE([ProviderName], [IsPrimary]);


GO
CREATE NONCLUSTERED INDEX [IX_PatientProviders_PatientId]
    ON [pt].[PatientProviders]([PatientId] ASC)
    INCLUDE([ProviderTypeId], [ProviderName], [IsPrimary], [Status]);

