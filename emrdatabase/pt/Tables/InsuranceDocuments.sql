CREATE TABLE [pt].[InsuranceDocuments] (
    [InsuranceDocumentId] UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [InsuranceId]         UNIQUEIDENTIFIER NOT NULL,
    [FileName]            NVARCHAR (500)   NOT NULL,
    [DocumentType]        NVARCHAR (100)   NOT NULL,
    [ContentType]         NVARCHAR (100)   NOT NULL,
    [FileSize]            BIGINT           NOT NULL,
    [StorageLocation]     NVARCHAR (1000)  NOT NULL,
    [EncryptionKeyId]     NVARCHAR (100)   NULL,
    [FileHash]            NVARCHAR (256)   NULL,
    [EffectiveDate]       DATETIME2 (7)    NULL,
    [ExpirationDate]      DATETIME2 (7)    NULL,
    [IsActive]            BIT              DEFAULT ((1)) NULL,
    [CreatedAt]           DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [CreatedBy]           NVARCHAR (100)   NOT NULL,
    PRIMARY KEY CLUSTERED ([InsuranceDocumentId] ASC),
    FOREIGN KEY ([InsuranceId]) REFERENCES [pt].[Insurances] ([InsuranceId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_InsuranceId]
    ON [pt].[InsuranceDocuments]([InsuranceId] ASC);

