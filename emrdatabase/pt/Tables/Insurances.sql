CREATE TABLE [pt].[Insurances] (
    [InsuranceId]            UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [PatientId]              UNIQUEIDENTIFIER NOT NULL,
    [PayerName]              NVARCHAR (200)   NOT NULL,
    [PlanName]               NVARCHAR (200)   NOT NULL,
    [PlanType]               NVARCHAR (50)    NOT NULL,
    [MemberId]               NVARCHAR (100)   NOT NULL,
    [GroupNumber]            NVARCHAR (100)   NULL,
    [GroupName]              NVARCHAR (200)   NULL,
    [EffectiveDate]          DATETIME2 (7)    NOT NULL,
    [ExpirationDate]         DATETIME2 (7)    NULL,
    [IsPrimary]              BIT              DEFAULT ((1)) NULL,
    [Priority]               INT              DEFAULT ((1)) NULL,
    [SubscriberRelationship] NVARCHAR (50)    DEFAULT ('Self') NULL,
    [SubscriberName]         NVARCHAR (200)   NULL,
    [SubscriberDateOfBirth]  DATETIME2 (7)    NULL,
    [SubscriberSsnEncrypted] NVARCHAR (500)   NULL,
    [PayerPhone]             NVARCHAR (20)    NULL,
    [PayerWebsite]           NVARCHAR (500)   NULL,
    [CustomerServicePhone]   NVARCHAR (20)    NULL,
    [Deductible]             DECIMAL (18, 2)  NULL,
    [DeductibleMet]          DECIMAL (18, 2)  NULL,
    [OutOfPocketMax]         DECIMAL (18, 2)  NULL,
    [OutOfPocketMet]         DECIMAL (18, 2)  NULL,
    [Copay]                  DECIMAL (18, 2)  NULL,
    [CoinsurancePercentage]  DECIMAL (5, 2)   NULL,
    [Status]                 NVARCHAR (50)    DEFAULT ('Active') NULL,
    [IsVerified]             BIT              DEFAULT ((0)) NULL,
    [LastVerifiedDate]       DATETIME2 (7)    NULL,
    [FhirCoverageId]         NVARCHAR (100)   NULL,
    [LastFhirSync]           DATETIME2 (7)    NULL,
    [CreatedAt]              DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [UpdatedAt]              DATETIME2 (7)    NULL,
    [CreatedBy]              NVARCHAR (100)   NOT NULL,
    [IsActive]               BIT              DEFAULT ((1)) NULL,
    PRIMARY KEY CLUSTERED ([InsuranceId] ASC),
    FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_MemberId]
    ON [pt].[Insurances]([MemberId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_PatientId]
    ON [pt].[Insurances]([PatientId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Insurances_PatientId_Active]
    ON [pt].[Insurances]([PatientId] ASC, [IsActive] ASC)
    INCLUDE([PayerName], [MemberId], [Priority]);

