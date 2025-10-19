CREATE TABLE [pt].[PatientProviders] (
    [PatientProviderId]   UNIQUEIDENTIFIER NOT NULL DEFAULT (newid()),
    [PatientId]           UNIQUEIDENTIFIER NOT NULL,
    [ProviderTypeId]      INT              NOT NULL,
    [ProviderTypeName]    NVARCHAR (100)   NOT NULL,
    
    -- Provider Details
    [ProviderName]        NVARCHAR (200)   NOT NULL,
    [Specialty]           NVARCHAR (200)   NULL,
    [NPI]                 NVARCHAR (10)    NULL,
    
    -- Contact Information
    [Phone]               NVARCHAR (25)    NULL,
    [Fax]                 NVARCHAR (25)    NULL,
    [Email]               NVARCHAR (100)   NULL,
    
    -- Address
    [AddressLine1]        NVARCHAR (200)   NULL,
    [AddressLine2]        NVARCHAR (200)   NULL,
    [City]                NVARCHAR (100)   NULL,
    [State]               NVARCHAR (50)    NULL,
    [ZipCode]             NVARCHAR (20)    NULL,
    [Country]             NVARCHAR (100)   NULL DEFAULT ('USA'),
    
    -- Additional Details
    [PracticeName]        NVARCHAR (200)   NULL,
    [Website]             NVARCHAR (500)   NULL,
    [Notes]               NVARCHAR (MAX)   NULL,
    
    -- Status
    [IsPrimary]           BIT              NOT NULL DEFAULT 0,
    [IsAcceptingPatients] BIT              NOT NULL DEFAULT 1,
    [Status]              NVARCHAR (50)    NOT NULL DEFAULT ('Active'),
    
    -- Dates
    [FirstVisitDate]      DATETIME2 (7)    NULL,
    [LastVisitDate]       DATETIME2 (7)    NULL,
    
    -- FHIR Integration
    [FhirPractitionerId]  NVARCHAR (100)   NULL,
    [FhirOrganizationId]  NVARCHAR (100)   NULL,
    
    -- System Fields
    [CreatedAt]           DATETIME2 (7)    NOT NULL DEFAULT (getutcdate()),
    [UpdatedAt]           DATETIME2 (7)    NULL,
    [CreatedBy]           NVARCHAR (100)   NOT NULL DEFAULT ('system'),
    [UpdatedBy]           NVARCHAR (100)   NULL,
    [IsActive]            BIT              NOT NULL DEFAULT 1,
    
    CONSTRAINT [PK_PatientProviders] PRIMARY KEY CLUSTERED ([PatientProviderId] ASC),
    CONSTRAINT [FK_PatientProviders_Patient] FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE,
    CONSTRAINT [FK_PatientProviders_ProviderType] FOREIGN KEY ([ProviderTypeId]) REFERENCES [pt].[ProviderTypes] ([ProviderTypeId])
);
GO

CREATE NONCLUSTERED INDEX [IX_PatientProviders_PatientId]
    ON [pt].[PatientProviders]([PatientId] ASC)
    INCLUDE ([ProviderTypeId], [ProviderName], [IsPrimary], [Status]);
GO

CREATE NONCLUSTERED INDEX [IX_PatientProviders_ProviderType]
    ON [pt].[PatientProviders]([ProviderTypeId] ASC, [PatientId] ASC)
    INCLUDE ([ProviderName], [IsPrimary]);
GO

CREATE NONCLUSTERED INDEX [IX_PatientProviders_NPI]
    ON [pt].[PatientProviders]([NPI] ASC)
    WHERE ([NPI] IS NOT NULL);
GO

