-- ================================================================
-- 10_PatientIntakeExtensions.sql
-- Description: Comprehensive patient intake extensions for pt schema
-- Adds all missing fields and tables for complete patient intake
-- ================================================================

USE HealthcarePlatform;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

--------------------------------------------------------------------
-- 1. Extend pt.Patients table with missing fields
--------------------------------------------------------------------
IF COL_LENGTH('pt.Patients', 'MiddleName') IS NULL 
    ALTER TABLE pt.Patients ADD MiddleName NVARCHAR(100) NULL;

IF COL_LENGTH('pt.Patients', 'Suffix') IS NULL 
    ALTER TABLE pt.Patients ADD Suffix NVARCHAR(20) NULL;

IF COL_LENGTH('pt.Patients', 'GenderId') IS NULL 
    ALTER TABLE pt.Patients ADD GenderId INT NULL;

IF COL_LENGTH('pt.Patients', 'MaritalStatusId') IS NULL 
    ALTER TABLE pt.Patients ADD MaritalStatusId INT NULL;

IF COL_LENGTH('pt.Patients', 'RaceId') IS NULL 
    ALTER TABLE pt.Patients ADD RaceId INT NULL;

IF COL_LENGTH('pt.Patients', 'PrimaryLanguageId') IS NULL 
    ALTER TABLE pt.Patients ADD PrimaryLanguageId INT NULL;

IF COL_LENGTH('pt.Patients', 'PreferredLanguageId') IS NULL 
    ALTER TABLE pt.Patients ADD PreferredLanguageId INT NULL;

IF COL_LENGTH('pt.Patients', 'PhotoIdPath') IS NULL 
    ALTER TABLE pt.Patients ADD PhotoIdPath NVARCHAR(500) NULL;

IF COL_LENGTH('pt.Patients', 'InterpreterRequired') IS NULL 
    ALTER TABLE pt.Patients ADD InterpreterRequired BIT NOT NULL DEFAULT(0);

IF COL_LENGTH('pt.Patients', 'MobilityAssistance') IS NULL 
    ALTER TABLE pt.Patients ADD MobilityAssistance BIT NOT NULL DEFAULT(0);

IF COL_LENGTH('pt.Patients', 'CommunicationPrefId') IS NULL 
    ALTER TABLE pt.Patients ADD CommunicationPrefId INT NULL;

IF COL_LENGTH('pt.Patients', 'ReligiousConsiderations') IS NULL 
    ALTER TABLE pt.Patients ADD ReligiousConsiderations NVARCHAR(MAX) NULL;

IF COL_LENGTH('pt.Patients', 'PrimaryCarePhysician') IS NULL 
    ALTER TABLE pt.Patients ADD PrimaryCarePhysician NVARCHAR(200) NULL;

IF COL_LENGTH('pt.Patients', 'PCPPhoneNumber') IS NULL 
    ALTER TABLE pt.Patients ADD PCPPhoneNumber NVARCHAR(20) NULL;

IF COL_LENGTH('pt.Patients', 'ReferringPhysician') IS NULL 
    ALTER TABLE pt.Patients ADD ReferringPhysician NVARCHAR(200) NULL;

IF COL_LENGTH('pt.Patients', 'PreferredPharmacyName') IS NULL 
    ALTER TABLE pt.Patients ADD PreferredPharmacyName NVARCHAR(200) NULL;

IF COL_LENGTH('pt.Patients', 'PreferredPharmacyLocation') IS NULL 
    ALTER TABLE pt.Patients ADD PreferredPharmacyLocation NVARCHAR(500) NULL;

IF COL_LENGTH('pt.Patients', 'ElectronicSignature') IS NULL 
    ALTER TABLE pt.Patients ADD ElectronicSignature NVARCHAR(200) NULL;

IF COL_LENGTH('pt.Patients', 'SignedDate') IS NULL 
    ALTER TABLE pt.Patients ADD SignedDate DATETIME2 NULL;
GO

PRINT 'Extended pt.Patients table with additional fields';
GO

--------------------------------------------------------------------
-- 2. Create pt.PatientAddresses if not exists
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables t 
               JOIN sys.schemas s ON t.schema_id = s.schema_id 
               WHERE s.name = 'pt' AND t.name = 'PatientAddresses')
BEGIN
    CREATE TABLE pt.PatientAddresses (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PatientId UNIQUEIDENTIFIER NOT NULL,
        AddressTypeId INT NOT NULL,
        Line1 NVARCHAR(200) NOT NULL,
        Line2 NVARCHAR(200) NULL,
        City NVARCHAR(100) NULL,
        State NVARCHAR(100) NULL,
        PostalCode NVARCHAR(20) NULL,
        Country NVARCHAR(100) NULL DEFAULT('USA'),
        CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_PTPatientAddresses_Patient FOREIGN KEY (PatientId) 
            REFERENCES pt.Patients(PatientId) ON DELETE CASCADE
    );
    PRINT 'Created pt.PatientAddresses table';
END
GO

--------------------------------------------------------------------
-- 3. Create pt.PatientPhones if not exists
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables t 
               JOIN sys.schemas s ON t.schema_id = s.schema_id 
               WHERE s.name = 'pt' AND t.name = 'PatientPhones')
BEGIN
    CREATE TABLE pt.PatientPhones (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PatientId UNIQUEIDENTIFIER NOT NULL,
        PhoneTypeId INT NOT NULL,
        PhoneNumber NVARCHAR(25) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_PTPatientPhones_Patient FOREIGN KEY (PatientId) 
            REFERENCES pt.Patients(PatientId) ON DELETE CASCADE
    );
    PRINT 'Created pt.PatientPhones table';
END
GO

--------------------------------------------------------------------
-- 4. Create pt.EmergencyContacts if not exists
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables t 
               JOIN sys.schemas s ON t.schema_id = s.schema_id 
               WHERE s.name = 'pt' AND t.name = 'EmergencyContacts')
BEGIN
    CREATE TABLE pt.EmergencyContacts (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PatientId UNIQUEIDENTIFIER NOT NULL,
        Name NVARCHAR(150) NOT NULL,
        RelationshipTypeId INT NOT NULL,
        Phone NVARCHAR(25) NOT NULL,
        AltPhone NVARCHAR(25) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_PTEmergencyContacts_Patient FOREIGN KEY (PatientId) 
            REFERENCES pt.Patients(PatientId) ON DELETE CASCADE
    );
    PRINT 'Created pt.EmergencyContacts table';
END
GO

--------------------------------------------------------------------
-- 5. Create pt.InsurancePolicies if not exists
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables t 
               JOIN sys.schemas s ON t.schema_id = s.schema_id 
               WHERE s.name = 'pt' AND t.name = 'InsurancePolicies')
BEGIN
    CREATE TABLE pt.InsurancePolicies (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PatientId UNIQUEIDENTIFIER NOT NULL,
        ProviderId INT NOT NULL,
        GroupNumber NVARCHAR(50) NULL,
        PolicyNumber NVARCHAR(50) NULL,
        PolicyHolderName NVARCHAR(150) NULL,
        ProviderPhone NVARCHAR(25) NULL,
        MedicareMedicaidId NVARCHAR(50) NULL,
        CardImagePath NVARCHAR(260) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_PTInsurancePolicies_Patient FOREIGN KEY (PatientId) 
            REFERENCES pt.Patients(PatientId) ON DELETE CASCADE
    );
    PRINT 'Created pt.InsurancePolicies table';
END
GO

--------------------------------------------------------------------
-- 6. Create pt.SocialHistory if not exists (1-to-1)
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables t 
               JOIN sys.schemas s ON t.schema_id = s.schema_id 
               WHERE s.name = 'pt' AND t.name = 'SocialHistory')
BEGIN
    CREATE TABLE pt.SocialHistory (
        PatientId UNIQUEIDENTIFIER PRIMARY KEY,
        SmokingStatusId INT NULL,
        AlcoholUseId INT NULL,
        DrugUseId INT NULL,
        Occupation NVARCHAR(150) NULL,
        LivingSituationId INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_PTSocialHistory_Patient FOREIGN KEY (PatientId) 
            REFERENCES pt.Patients(PatientId) ON DELETE CASCADE
    );
    PRINT 'Created pt.SocialHistory table';
END
GO

--------------------------------------------------------------------
-- 7. Create pt.LegalConsents if not exists (1-to-1)
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables t 
               JOIN sys.schemas s ON t.schema_id = s.schema_id 
               WHERE s.name = 'pt' AND t.name = 'LegalConsents')
BEGIN
    CREATE TABLE pt.LegalConsents (
        PatientId UNIQUEIDENTIFIER PRIMARY KEY,
        HipaaAgreed BIT NOT NULL DEFAULT(0),
        ConsentToTreat BIT NOT NULL DEFAULT(0),
        AdvanceDirectives BIT NOT NULL DEFAULT(0),
        AdvanceDirectivesPath NVARCHAR(500) NULL,
        AssignmentOfBenefits BIT NOT NULL DEFAULT(0),
        FinancialResponsibility BIT NOT NULL DEFAULT(0),
        SignedOnUtc DATETIME2 NULL,
        SignaturePath NVARCHAR(260) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_PTLegalConsents_Patient FOREIGN KEY (PatientId) 
            REFERENCES pt.Patients(PatientId) ON DELETE CASCADE
    );
    PRINT 'Created pt.LegalConsents table';
END
GO

--------------------------------------------------------------------
-- 8. Extend pt.Allergies if needed
--------------------------------------------------------------------
IF COL_LENGTH('pt.Allergies', 'Notes') IS NULL 
    ALTER TABLE pt.Allergies ADD Notes NVARCHAR(MAX) NULL;
GO

--------------------------------------------------------------------
-- 9. Create pt.Medications table
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables t 
               JOIN sys.schemas s ON t.schema_id = s.schema_id 
               WHERE s.name = 'pt' AND t.name = 'Medications')
BEGIN
    CREATE TABLE pt.Medications (
        MedicationId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        PatientId UNIQUEIDENTIFIER NOT NULL,
        MedicationName NVARCHAR(200) NOT NULL,
        Dosage NVARCHAR(100) NULL,
        Frequency NVARCHAR(100) NULL,
        Prescriber NVARCHAR(200) NULL,
        StartDate DATETIME2 NULL,
        EndDate DATETIME2 NULL,
        IsActive BIT DEFAULT 1,
        Notes NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2,
        CreatedBy NVARCHAR(100) NOT NULL,
        FOREIGN KEY (PatientId) REFERENCES pt.Patients(PatientId) ON DELETE CASCADE,
        INDEX IX_Medications_PatientId (PatientId)
    );
    PRINT 'Created pt.Medications table';
END
GO

--------------------------------------------------------------------
-- 10. Create pt.PatientSurgeries table
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables t 
               JOIN sys.schemas s ON t.schema_id = s.schema_id 
               WHERE s.name = 'pt' AND t.name = 'PatientSurgeries')
BEGIN
    CREATE TABLE pt.PatientSurgeries (
        SurgeryId INT IDENTITY(1,1) PRIMARY KEY,
        PatientId UNIQUEIDENTIFIER NOT NULL,
        SurgeryType NVARCHAR(200) NOT NULL,
        SurgeryDate DATE NULL,
        Notes NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
        CONSTRAINT FK_PTPatientSurgeries_Patient FOREIGN KEY (PatientId) 
            REFERENCES pt.Patients(PatientId) ON DELETE CASCADE
    );
    PRINT 'Created pt.PatientSurgeries table';
END
GO

--------------------------------------------------------------------
-- 11. Create pt.PatientHospitalizations table
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables t 
               JOIN sys.schemas s ON t.schema_id = s.schema_id 
               WHERE s.name = 'pt' AND t.name = 'PatientHospitalizations')
BEGIN
    CREATE TABLE pt.PatientHospitalizations (
        HospitalizationId INT IDENTITY(1,1) PRIMARY KEY,
        PatientId UNIQUEIDENTIFIER NOT NULL,
        HospitalName NVARCHAR(200) NULL,
        Reason NVARCHAR(500) NULL,
        AdmissionDate DATE NULL,
        DischargeDate DATE NULL,
        Notes NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
        CONSTRAINT FK_PTPatientHospitalizations_Patient FOREIGN KEY (PatientId) 
            REFERENCES pt.Patients(PatientId) ON DELETE CASCADE
    );
    PRINT 'Created pt.PatientHospitalizations table';
END
GO

--------------------------------------------------------------------
-- 12. Create pt.PatientChronicConditions junction table
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables t 
               JOIN sys.schemas s ON t.schema_id = s.schema_id 
               WHERE s.name = 'pt' AND t.name = 'PatientChronicConditions')
BEGIN
    CREATE TABLE pt.PatientChronicConditions (
        PatientId UNIQUEIDENTIFIER NOT NULL,
        ConditionId UNIQUEIDENTIFIER NOT NULL,
        DiagnosedDate DATE NULL,
        Notes NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
        PRIMARY KEY (PatientId, ConditionId),
        CONSTRAINT FK_PTPatientChronicConditions_Patient FOREIGN KEY (PatientId) 
            REFERENCES pt.Patients(PatientId) ON DELETE CASCADE,
        CONSTRAINT FK_PTPatientChronicConditions_Condition FOREIGN KEY (ConditionId) 
            REFERENCES pt.Conditions(ConditionId) ON DELETE NO ACTION
    );
    PRINT 'Created pt.PatientChronicConditions table';
END
GO

--------------------------------------------------------------------
-- 13. Create pt.FamilyMedicalHistory table
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables t 
               JOIN sys.schemas s ON t.schema_id = s.schema_id 
               WHERE s.name = 'pt' AND t.name = 'FamilyMedicalHistory')
BEGIN
    CREATE TABLE pt.FamilyMedicalHistory (
        FamilyHistoryId INT IDENTITY(1,1) PRIMARY KEY,
        PatientId UNIQUEIDENTIFIER NOT NULL,
        Relative NVARCHAR(100) NOT NULL,
        Condition NVARCHAR(200) NOT NULL,
        Notes NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT(GETUTCDATE()),
        CONSTRAINT FK_PTFamilyMedicalHistory_Patient FOREIGN KEY (PatientId) 
            REFERENCES pt.Patients(PatientId) ON DELETE CASCADE
    );
    PRINT 'Created pt.FamilyMedicalHistory table';
END
GO

PRINT 'Patient intake extensions completed successfully';
GO

