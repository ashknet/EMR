CREATE TABLE [pt].[FamilyRelations] (
    [RelationId]              UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [PatientId]               UNIQUEIDENTIFIER NOT NULL,
    [RelatedPatientId]        UNIQUEIDENTIFIER NOT NULL,
    [RelationType]            NVARCHAR (50)    NOT NULL,
    [IsGuardian]              BIT              DEFAULT ((0)) NULL,
    [IsProxy]                 BIT              DEFAULT ((0)) NULL,
    [CanViewRecords]          BIT              DEFAULT ((0)) NULL,
    [CanManageRecords]        BIT              DEFAULT ((0)) NULL,
    [CanGrantConsent]         BIT              DEFAULT ((0)) NULL,
    [LegalDocumentType]       NVARCHAR (100)   NULL,
    [LegalDocumentNumber]     NVARCHAR (100)   NULL,
    [LegalDocumentExpiryDate] DATETIME2 (7)    NULL,
    [CreatedAt]               DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [UpdatedAt]               DATETIME2 (7)    NULL,
    [CreatedBy]               NVARCHAR (100)   NOT NULL,
    [IsActive]                BIT              DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([RelationId] ASC),
    FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]),
    FOREIGN KEY ([RelatedPatientId]) REFERENCES [pt].[Patients] ([PatientId])
);


GO
CREATE NONCLUSTERED INDEX [IX_RelatedPatientId]
    ON [pt].[FamilyRelations]([RelatedPatientId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PatientId]
    ON [pt].[FamilyRelations]([PatientId] ASC);

