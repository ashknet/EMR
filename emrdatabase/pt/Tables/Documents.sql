CREATE TABLE [pt].[Documents] (
    [DocumentId]              UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [PatientId]               UNIQUEIDENTIFIER NOT NULL,
    [EncounterId]             UNIQUEIDENTIFIER NULL,
    [FileName]                NVARCHAR (500)   NOT NULL,
    [DocumentType]            NVARCHAR (100)   NOT NULL,
    [DocumentTypeCode]        NVARCHAR (50)    NULL,
    [ContentType]             NVARCHAR (100)   NOT NULL,
    [FileSize]                BIGINT           NOT NULL,
    [StorageLocation]         NVARCHAR (1000)  NOT NULL,
    [EncryptionKeyId]         NVARCHAR (100)   NULL,
    [FileHash]                NVARCHAR (256)   NULL,
    [Category]                NVARCHAR (100)   NULL,
    [Specialty]               NVARCHAR (100)   NULL,
    [FacilityType]            NVARCHAR (100)   NULL,
    [ProviderId]              NVARCHAR (100)   NULL,
    [ProviderName]            NVARCHAR (200)   NULL,
    [ServiceDate]             DATETIME2 (7)    NULL,
    [OcrProcessed]            BIT              DEFAULT ((0)) NULL,
    [OcrProcessedDate]        DATETIME2 (7)    NULL,
    [OcrText]                 NVARCHAR (MAX)   NULL,
    [OcrConfidence]           NVARCHAR (50)    NULL,
    [FhirMapped]              BIT              DEFAULT ((0)) NULL,
    [FhirMappingResult]       NVARCHAR (MAX)   NULL,
    [Status]                  NVARCHAR (50)    DEFAULT ('Current') NULL,
    [Description]             NVARCHAR (500)   NULL,
    [SecurityLevel]           NVARCHAR (50)    DEFAULT ('Standard') NULL,
    [RequiresConsent]         BIT              DEFAULT ((1)) NULL,
    [FhirDocumentReferenceId] NVARCHAR (100)   NULL,
    [LastFhirSync]            DATETIME2 (7)    NULL,
    [CreatedAt]               DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [UpdatedAt]               DATETIME2 (7)    NULL,
    [CreatedBy]               NVARCHAR (100)   NOT NULL,
    [IsActive]                BIT              DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([DocumentId] ASC),
    FOREIGN KEY ([EncounterId]) REFERENCES [pt].[Encounters] ([EncounterId]),
    FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_EncounterId]
    ON [pt].[Documents]([EncounterId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PatientId]
    ON [pt].[Documents]([PatientId] ASC);

