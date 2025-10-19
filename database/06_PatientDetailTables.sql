-- ================================================================
-- 06_PatientDetailTables.sql
-- Author: AI Assistant (o3-pro)
-- Description: Alter existing Patients table and create all related
--              detail tables (1-many) required by new patient intake
--              specification. Foreign keys enforce referential integrity.
-- ================================================================

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

--------------------------------------------------------------------
-- 1. Extend Patients core table
--------------------------------------------------------------------
IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Patients')
BEGIN
    -- Add columns if they do not exist
    IF COL_LENGTH('dbo.Patients','MiddleName') IS NULL ALTER TABLE dbo.Patients ADD MiddleName NVARCHAR(100) NULL;
    IF COL_LENGTH('dbo.Patients','Suffix') IS NULL ALTER TABLE dbo.Patients ADD Suffix NVARCHAR(20) NULL;
    IF COL_LENGTH('dbo.Patients','DateOfBirth') IS NULL ALTER TABLE dbo.Patients ADD DateOfBirth DATE NULL;
    IF COL_LENGTH('dbo.Patients','GenderId') IS NULL ALTER TABLE dbo.Patients ADD GenderId INT NULL CONSTRAINT FK_Patients_Gender FOREIGN KEY REFERENCES dbo.Gender(Id);
    IF COL_LENGTH('dbo.Patients','SSN') IS NULL ALTER TABLE dbo.Patients ADD SSN VARBINARY(256) NULL; -- store encrypted bytes
    IF COL_LENGTH('dbo.Patients','MaritalStatusId') IS NULL ALTER TABLE dbo.Patients ADD MaritalStatusId INT NULL CONSTRAINT FK_Patients_MaritalStatus FOREIGN KEY REFERENCES dbo.MaritalStatus(Id);
    IF COL_LENGTH('dbo.Patients','RaceId') IS NULL ALTER TABLE dbo.Patients ADD RaceId INT NULL CONSTRAINT FK_Patients_Race FOREIGN KEY REFERENCES dbo.Race(Id);
    IF COL_LENGTH('dbo.Patients','PrimaryLanguageId') IS NULL ALTER TABLE dbo.Patients ADD PrimaryLanguageId INT NULL CONSTRAINT FK_Patients_Language1 FOREIGN KEY REFERENCES dbo.[Language](Id);
    IF COL_LENGTH('dbo.Patients','PreferredLanguageId') IS NULL ALTER TABLE dbo.Patients ADD PreferredLanguageId INT NULL CONSTRAINT FK_Patients_Language2 FOREIGN KEY REFERENCES dbo.[Language](Id);
    IF COL_LENGTH('dbo.Patients','PhotoIdPath') IS NULL ALTER TABLE dbo.Patients ADD PhotoIdPath NVARCHAR(260) NULL;
    IF COL_LENGTH('dbo.Patients','InterpreterRequired') IS NULL ALTER TABLE dbo.Patients ADD InterpreterRequired BIT NOT NULL DEFAULT(0);
    IF COL_LENGTH('dbo.Patients','MobilityAssistance') IS NULL ALTER TABLE dbo.Patients ADD MobilityAssistance BIT NOT NULL DEFAULT(0);
    IF COL_LENGTH('dbo.Patients','CommunicationPrefId') IS NULL ALTER TABLE dbo.Patients ADD CommunicationPrefId INT NULL CONSTRAINT FK_Patients_CommPref FOREIGN KEY REFERENCES dbo.CommunicationPreference(Id);
    IF COL_LENGTH('dbo.Patients','ReligiousConsiderations') IS NULL ALTER TABLE dbo.Patients ADD ReligiousConsiderations NVARCHAR(MAX) NULL;
END;
GO

--------------------------------------------------------------------
-- 2. AddressType seeded values
--------------------------------------------------------------------
INSERT INTO dbo.AddressType (Name, SortOrder)
SELECT v.n, v.o FROM (VALUES('Home',0),('Mailing',1)) v(n,o)
WHERE NOT EXISTS (SELECT 1 FROM dbo.AddressType t WHERE t.Name = v.n);
GO

--------------------------------------------------------------------
-- 3. PatientAddresses
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PatientAddresses')
BEGIN
    CREATE TABLE dbo.PatientAddresses (
        Id INT IDENTITY(1,1) CONSTRAINT PK_PatientAddresses PRIMARY KEY,
        PatientId INT NOT NULL,
        AddressTypeId INT NOT NULL,
        Line1 NVARCHAR(200) NOT NULL,
        Line2 NVARCHAR(200) NULL,
        City NVARCHAR(100) NULL,
        State NVARCHAR(100) NULL,
        PostalCode NVARCHAR(20) NULL,
        Country NVARCHAR(100) NULL,
        CreatedUtc DATETIME2(0) NOT NULL DEFAULT(SYSUTCDATETIME()),
        UpdatedUtc DATETIME2(0) NULL,
        CONSTRAINT FK_PatientAddresses_Patient FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id) ON DELETE CASCADE,
        CONSTRAINT FK_PatientAddresses_AddressType FOREIGN KEY (AddressTypeId) REFERENCES dbo.AddressType(Id)
    );
END;
GO

--------------------------------------------------------------------
-- 4. PhoneType seed
--------------------------------------------------------------------
INSERT INTO dbo.PhoneType (Name, SortOrder)
SELECT v.n, v.o FROM (VALUES('Primary',0),('Secondary',1)) v(n,o)
WHERE NOT EXISTS (SELECT 1 FROM dbo.PhoneType t WHERE t.Name = v.n);
GO

--------------------------------------------------------------------
-- 5. PatientPhones
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PatientPhones')
BEGIN
    CREATE TABLE dbo.PatientPhones (
        Id INT IDENTITY(1,1) CONSTRAINT PK_PatientPhones PRIMARY KEY,
        PatientId INT NOT NULL,
        PhoneTypeId INT NOT NULL,
        PhoneNumber NVARCHAR(25) NOT NULL,
        CreatedUtc DATETIME2(0) NOT NULL DEFAULT(SYSUTCDATETIME()),
        UpdatedUtc DATETIME2(0) NULL,
        CONSTRAINT FK_PatientPhones_Patient FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id) ON DELETE CASCADE,
        CONSTRAINT FK_PatientPhones_PhoneType FOREIGN KEY (PhoneTypeId) REFERENCES dbo.PhoneType(Id)
    );
END;
GO

--------------------------------------------------------------------
-- 6. EmergencyContacts
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'EmergencyContacts')
BEGIN
    CREATE TABLE dbo.EmergencyContacts (
        Id INT IDENTITY(1,1) CONSTRAINT PK_EmergencyContacts PRIMARY KEY,
        PatientId INT NOT NULL,
        Name NVARCHAR(150) NOT NULL,
        RelationshipTypeId INT NOT NULL,
        Phone NVARCHAR(25) NOT NULL,
        AltPhone NVARCHAR(25) NULL,
        CreatedUtc DATETIME2(0) NOT NULL DEFAULT(SYSUTCDATETIME()),
        UpdatedUtc DATETIME2(0) NULL,
        CONSTRAINT FK_EC_Patient FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id) ON DELETE CASCADE,
        CONSTRAINT FK_EC_Relationship FOREIGN KEY (RelationshipTypeId) REFERENCES dbo.RelationshipType(Id)
    );
END;
GO

--------------------------------------------------------------------
-- 7. InsurancePolicies
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'InsurancePolicies')
BEGIN
    CREATE TABLE dbo.InsurancePolicies (
        Id INT IDENTITY(1,1) CONSTRAINT PK_InsurancePolicies PRIMARY KEY,
        PatientId INT NOT NULL,
        ProviderId INT NOT NULL,
        GroupNumber NVARCHAR(50) NULL,
        PolicyNumber NVARCHAR(50) NULL,
        PolicyHolderName NVARCHAR(150) NULL,
        ProviderPhone NVARCHAR(25) NULL,
        MedicareMedicaidId NVARCHAR(50) NULL,
        CardImagePath NVARCHAR(260) NULL,
        CreatedUtc DATETIME2(0) NOT NULL DEFAULT(SYSUTCDATETIME()),
        UpdatedUtc DATETIME2(0) NULL,
        CONSTRAINT FK_IP_Patient FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id) ON DELETE CASCADE,
        CONSTRAINT FK_IP_Provider FOREIGN KEY (ProviderId) REFERENCES dbo.InsuranceProvider(Id)
    );
END;
GO

--------------------------------------------------------------------
-- 8. SocialHistory (1-1)
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SocialHistory')
BEGIN
    CREATE TABLE dbo.SocialHistory (
        PatientId INT CONSTRAINT PK_SocialHistory PRIMARY KEY,
        SmokingStatusId INT NULL,
        AlcoholUseId INT NULL,
        DrugUseId INT NULL,
        Occupation NVARCHAR(150) NULL,
        LivingSituationId INT NULL,
        CreatedUtc DATETIME2(0) NOT NULL DEFAULT(SYSUTCDATETIME()),
        UpdatedUtc DATETIME2(0) NULL,
        CONSTRAINT FK_SH_Patient FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id) ON DELETE CASCADE,
        CONSTRAINT FK_SH_Smoking FOREIGN KEY (SmokingStatusId) REFERENCES dbo.SmokingStatus(Id),
        CONSTRAINT FK_SH_Alcohol FOREIGN KEY (AlcoholUseId) REFERENCES dbo.AlcoholUse(Id),
        CONSTRAINT FK_SH_Drug FOREIGN KEY (DrugUseId) REFERENCES dbo.DrugUse(Id),
        CONSTRAINT FK_SH_Living FOREIGN KEY (LivingSituationId) REFERENCES dbo.LivingSituation(Id)
    );
END;
GO

--------------------------------------------------------------------
-- 9. LegalConsents (1-1)
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LegalConsents')
BEGIN
    CREATE TABLE dbo.LegalConsents (
        PatientId INT CONSTRAINT PK_LegalConsents PRIMARY KEY,
        HipaaAgreed BIT NOT NULL DEFAULT(0),
        ConsentToTreat BIT NOT NULL DEFAULT(0),
        AdvanceDirectives BIT NOT NULL DEFAULT(0),
        AssignmentOfBenefits BIT NOT NULL DEFAULT(0),
        FinancialResponsibility BIT NOT NULL DEFAULT(0),
        SignedOnUtc DATETIME2(0) NULL,
        SignaturePath NVARCHAR(260) NULL,
        CreatedUtc DATETIME2(0) NOT NULL DEFAULT(SYSUTCDATETIME()),
        UpdatedUtc DATETIME2(0) NULL,
        CONSTRAINT FK_LC_Patient FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id) ON DELETE CASCADE
    );
END;
GO

--------------------------------------------------------------------
-- 10. Constraints and indexes as needed (omitted for brevity)
--------------------------------------------------------------------
