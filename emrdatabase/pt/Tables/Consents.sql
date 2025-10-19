CREATE TABLE [pt].[Consents] (
    [ConsentId]             UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [PatientId]             UNIQUEIDENTIFIER NOT NULL,
    [ConsentType]           NVARCHAR (100)   NOT NULL,
    [Status]                NVARCHAR (50)    DEFAULT ('Active') NULL,
    [Scope]                 NVARCHAR (100)   NOT NULL,
    [AllowDemographics]     BIT              DEFAULT ((0)) NULL,
    [AllowAllergies]        BIT              DEFAULT ((0)) NULL,
    [AllowConditions]       BIT              DEFAULT ((0)) NULL,
    [AllowImmunizations]    BIT              DEFAULT ((0)) NULL,
    [AllowMedications]      BIT              DEFAULT ((0)) NULL,
    [AllowInsurance]        BIT              DEFAULT ((0)) NULL,
    [AllowDocuments]        BIT              DEFAULT ((0)) NULL,
    [AllowEncounters]       BIT              DEFAULT ((0)) NULL,
    [RecipientName]         NVARCHAR (200)   NULL,
    [RecipientOrganization] NVARCHAR (200)   NULL,
    [RecipientType]         NVARCHAR (100)   NULL,
    [RecipientEmail]        NVARCHAR (255)   NULL,
    [ShareToken]            NVARCHAR (200)   NULL,
    [TokenExpiryDate]       DATETIME2 (7)    NULL,
    [MaxAccessCount]        INT              NULL,
    [AccessCount]           INT              DEFAULT ((0)) NULL,
    [GrantedDate]           DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [ExpiryDate]            DATETIME2 (7)    NULL,
    [RevokedDate]           DATETIME2 (7)    NULL,
    [RevocationReason]      NVARCHAR (500)   NULL,
    [LegalBasis]            NVARCHAR (200)   NULL,
    [ConsentFormUrl]        NVARCHAR (500)   NULL,
    [SignatureObtained]     BIT              DEFAULT ((0)) NULL,
    [SignatureDate]         DATETIME2 (7)    NULL,
    [Purpose]               NVARCHAR (500)   NULL,
    [Notes]                 NVARCHAR (MAX)   NULL,
    [LastAccessedBy]        NVARCHAR (200)   NULL,
    [LastAccessedAt]        DATETIME2 (7)    NULL,
    [CreatedAt]             DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [UpdatedAt]             DATETIME2 (7)    NULL,
    [CreatedBy]             NVARCHAR (100)   NOT NULL,
    [UpdatedBy]             NVARCHAR (100)   NULL,
    PRIMARY KEY CLUSTERED ([ConsentId] ASC),
    FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE,
    UNIQUE NONCLUSTERED ([ShareToken] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_ShareToken]
    ON [pt].[Consents]([ShareToken] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PatientId]
    ON [pt].[Consents]([PatientId] ASC);

