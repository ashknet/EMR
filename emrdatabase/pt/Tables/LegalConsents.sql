CREATE TABLE [pt].[LegalConsents] (
    [PatientId]               UNIQUEIDENTIFIER NOT NULL,
    [HipaaAgreed]             BIT              DEFAULT ((0)) NOT NULL,
    [ConsentToTreat]          BIT              DEFAULT ((0)) NOT NULL,
    [AdvanceDirectives]       BIT              DEFAULT ((0)) NOT NULL,
    [AdvanceDirectivesPath]   NVARCHAR (500)   NULL,
    [AssignmentOfBenefits]    BIT              DEFAULT ((0)) NOT NULL,
    [FinancialResponsibility] BIT              DEFAULT ((0)) NOT NULL,
    [SignedOnUtc]             DATETIME2 (7)    NULL,
    [SignaturePath]           NVARCHAR (260)   NULL,
    [CreatedAt]               DATETIME2 (7)    DEFAULT (getutcdate()) NOT NULL,
    [UpdatedAt]               DATETIME2 (7)    NULL,
    PRIMARY KEY CLUSTERED ([PatientId] ASC),
    CONSTRAINT [FK_PTLegalConsents_Patient] FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_LegalConsents_PatientId]
    ON [pt].[LegalConsents]([PatientId] ASC);

