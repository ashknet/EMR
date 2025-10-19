CREATE TABLE [pt].[RecordTransfers] (
    [TransferId]            UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [PatientId]             UNIQUEIDENTIFIER NOT NULL,
    [ConsentId]             UNIQUEIDENTIFIER NULL,
    [TransferType]          NVARCHAR (50)    NOT NULL,
    [TransferMethod]        NVARCHAR (50)    NOT NULL,
    [RecipientOrganization] NVARCHAR (200)   NULL,
    [RecipientName]         NVARCHAR (200)   NULL,
    [RecipientEmail]        NVARCHAR (255)   NULL,
    [RecipientFhirEndpoint] NVARCHAR (500)   NULL,
    [RecipientNpi]          NVARCHAR (20)    NULL,
    [SenderOrganization]    NVARCHAR (200)   NULL,
    [SenderName]            NVARCHAR (200)   NULL,
    [SenderEmail]           NVARCHAR (255)   NULL,
    [Purpose]               NVARCHAR (500)   NULL,
    [RequestedBy]           NVARCHAR (200)   NULL,
    [RequestedDate]         DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [Status]                NVARCHAR (50)    DEFAULT ('Requested') NULL,
    [StatusReason]          NVARCHAR (500)   NULL,
    [ApprovedDate]          DATETIME2 (7)    NULL,
    [ApprovedBy]            NVARCHAR (200)   NULL,
    [CompletedDate]         DATETIME2 (7)    NULL,
    [CancelledDate]         DATETIME2 (7)    NULL,
    [CancellationReason]    NVARCHAR (500)   NULL,
    [IncludeDemographics]   BIT              DEFAULT ((1)) NULL,
    [IncludeAllergies]      BIT              DEFAULT ((1)) NULL,
    [IncludeConditions]     BIT              DEFAULT ((1)) NULL,
    [IncludeImmunizations]  BIT              DEFAULT ((1)) NULL,
    [IncludeMedications]    BIT              DEFAULT ((1)) NULL,
    [IncludeEncounters]     BIT              DEFAULT ((1)) NULL,
    [IncludeDocuments]      BIT              DEFAULT ((1)) NULL,
    [IncludeInsurance]      BIT              DEFAULT ((0)) NULL,
    [DataStartDate]         DATETIME2 (7)    NULL,
    [DataEndDate]           DATETIME2 (7)    NULL,
    [FhirBundleUrl]         NVARCHAR (1000)  NULL,
    [FhirBundleId]          NVARCHAR (100)   NULL,
    [ResourceCount]         INT              NULL,
    [AccessToken]           NVARCHAR (500)   NULL,
    [TokenExpiryDate]       DATETIME2 (7)    NULL,
    [EncryptionKeyId]       NVARCHAR (100)   NULL,
    [TrackingNumber]        NVARCHAR (50)    NULL,
    [ExternalTransferId]    NVARCHAR (100)   NULL,
    [CreatedAt]             DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [UpdatedAt]             DATETIME2 (7)    NULL,
    [CreatedBy]             NVARCHAR (100)   NOT NULL,
    PRIMARY KEY CLUSTERED ([TransferId] ASC),
    FOREIGN KEY ([ConsentId]) REFERENCES [pt].[Consents] ([ConsentId]),
    FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE,
    UNIQUE NONCLUSTERED ([TrackingNumber] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_TrackingNumber]
    ON [pt].[RecordTransfers]([TrackingNumber] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PatientId]
    ON [pt].[RecordTransfers]([PatientId] ASC);

