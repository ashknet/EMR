CREATE TABLE [pt].[Patients] (
    [PatientId]                    UNIQUEIDENTIFIER DEFAULT (newid()) NOT NULL,
    [FirstName]                    NVARCHAR (100)   NOT NULL,
    [MiddleName]                   NVARCHAR (100)   NULL,
    [LastName]                     NVARCHAR (100)   NOT NULL,
    [DateOfBirth]                  DATE             NOT NULL,
    [Gender]                       NVARCHAR (20)    NOT NULL,
    [SsnEncrypted]                 NVARCHAR (500)   NULL,
    [SsnHash]                      NVARCHAR (128)   NULL,
    [Email]                        NVARCHAR (255)   NOT NULL,
    [PhoneNumber]                  NVARCHAR (20)    NOT NULL,
    [AlternatePhone]               NVARCHAR (20)    NULL,
    [AddressLine1]                 NVARCHAR (200)   NOT NULL,
    [AddressLine2]                 NVARCHAR (200)   NULL,
    [City]                         NVARCHAR (100)   NOT NULL,
    [State]                        NVARCHAR (2)     NOT NULL,
    [ZipCode]                      NVARCHAR (10)    NOT NULL,
    [Country]                      NVARCHAR (50)    DEFAULT ('USA') NULL,
    [Race]                         NVARCHAR (100)   NULL,
    [Ethnicity]                    NVARCHAR (100)   NULL,
    [PreferredLanguage]            NVARCHAR (50)    NULL,
    [MaritalStatus]                NVARCHAR (50)    NULL,
    [EmergencyContactName]         NVARCHAR (200)   NULL,
    [EmergencyContactPhone]        NVARCHAR (20)    NULL,
    [EmergencyContactRelationship] NVARCHAR (50)    NULL,
    [FhirPatientId]                NVARCHAR (100)   NULL,
    [LastFhirSync]                 DATETIME2 (7)    NULL,
    [CreatedAt]                    DATETIME2 (7)    DEFAULT (getutcdate()) NULL,
    [UpdatedAt]                    DATETIME2 (7)    NULL,
    [CreatedBy]                    NVARCHAR (100)   NOT NULL,
    [UpdatedBy]                    NVARCHAR (100)   NULL,
    [IsActive]                     BIT              DEFAULT ((1)) NULL,
    [IsDeleted]                    BIT              DEFAULT ((0)) NULL,
    [Suffix]                       NVARCHAR (20)    NULL,
    [GenderId]                     INT              NULL,
    [MaritalStatusId]              INT              NULL,
    [RaceId]                       INT              NULL,
    [PrimaryLanguageId]            INT              NULL,
    [PreferredLanguageId]          INT              NULL,
    [PhotoIdPath]                  NVARCHAR (500)   NULL,
    [InterpreterRequired]          BIT              DEFAULT ((0)) NOT NULL,
    [MobilityAssistance]           BIT              DEFAULT ((0)) NOT NULL,
    [CommunicationPrefId]          INT              NULL,
    [ReligiousConsiderations]      NVARCHAR (MAX)   NULL,
    [PrimaryCarePhysician]         NVARCHAR (200)   NULL,
    [PCPPhoneNumber]               NVARCHAR (20)    NULL,
    [ReferringPhysician]           NVARCHAR (200)   NULL,
    [PreferredPharmacyName]        NVARCHAR (200)   NULL,
    [PreferredPharmacyLocation]    NVARCHAR (500)   NULL,
    [ElectronicSignature]          NVARCHAR (200)   NULL,
    [SignedDate]                   DATETIME2 (7)    NULL,
    PRIMARY KEY CLUSTERED ([PatientId] ASC),
    UNIQUE NONCLUSTERED ([Email] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Name_DOB]
    ON [pt].[Patients]([LastName] ASC, [FirstName] ASC, [DateOfBirth] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SsnHash]
    ON [pt].[Patients]([SsnHash] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Email]
    ON [pt].[Patients]([Email] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Patients_Email]
    ON [pt].[Patients]([Email] ASC) WHERE ([IsDeleted]=(0));


GO
CREATE NONCLUSTERED INDEX [IX_Patients_PhoneNumber]
    ON [pt].[Patients]([PhoneNumber] ASC) WHERE ([IsDeleted]=(0));


GO
CREATE NONCLUSTERED INDEX [IX_Patients_Name]
    ON [pt].[Patients]([LastName] ASC, [FirstName] ASC)
    INCLUDE([MiddleName], [DateOfBirth]) WHERE ([IsDeleted]=(0));


GO
CREATE NONCLUSTERED INDEX [IX_Patients_Active]
    ON [pt].[Patients]([IsActive] ASC, [IsDeleted] ASC)
    INCLUDE([PatientId], [FirstName], [LastName], [Email]);

