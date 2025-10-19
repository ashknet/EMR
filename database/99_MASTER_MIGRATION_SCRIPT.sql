-- =============================================
-- MASTER MIGRATION SCRIPT
-- Created: October 19, 2025
-- Purpose: Apply all Patient Portal enhancements
-- =============================================
-- This script consolidates all changes made during the synchronization project
-- Run this script to update your database with all new features
-- =============================================

USE EMRMaster;
GO

SET NOCOUNT ON;
GO

PRINT '';
PRINT '╔═══════════════════════════════════════════════════════════════╗';
PRINT '║   PATIENT PORTAL SYNCHRONIZATION - MASTER MIGRATION SCRIPT   ║';
PRINT '║   Date: October 19, 2025                                      ║';
PRINT '╚═══════════════════════════════════════════════════════════════╝';
PRINT '';

-- =============================================
-- STEP 1: Chronic Conditions Table
-- =============================================
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';
PRINT 'STEP 1: Creating Chronic Conditions Table...';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ChronicConditions' AND schema_id = SCHEMA_ID('pt'))
BEGIN
    CREATE TABLE pt.ChronicConditions (
        ConditionId     UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        PatientId       UNIQUEIDENTIFIER NOT NULL,
        ConditionName   NVARCHAR(200)    NOT NULL,
        DiagnosedDate   DATETIME2        NULL,
        Status          NVARCHAR(50)     NOT NULL DEFAULT 'Active',
        Notes           NVARCHAR(MAX)    NULL,
        CreatedAt       DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt       DATETIME2        NULL,
        CreatedBy       NVARCHAR(100)    NOT NULL DEFAULT 'system',
        UpdatedBy       NVARCHAR(100)    NULL,
        IsActive        BIT              NOT NULL DEFAULT 1,
        
        CONSTRAINT PK_ChronicConditions PRIMARY KEY CLUSTERED (ConditionId),
        CONSTRAINT FK_ChronicConditions_Patient FOREIGN KEY (PatientId) 
            REFERENCES pt.Patients(PatientId) ON DELETE CASCADE
    );
    
    CREATE NONCLUSTERED INDEX IX_ChronicConditions_PatientId
        ON pt.ChronicConditions(PatientId)
        INCLUDE (ConditionName, Status, IsActive);
    
    PRINT '✓ ChronicConditions table created successfully';
END
ELSE
BEGIN
    PRINT '✓ ChronicConditions table already exists';
END
GO

-- =============================================
-- STEP 2: Immunizations Table
-- =============================================
PRINT '';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';
PRINT 'STEP 2: Creating Immunizations Table...';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Immunizations' AND schema_id = SCHEMA_ID('pt'))
BEGIN
    CREATE TABLE pt.Immunizations (
        ImmunizationId      UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        PatientId           UNIQUEIDENTIFIER NOT NULL,
        
        VaccineName         NVARCHAR(200)    NOT NULL,
        AdministeredDate    DATETIME2        NOT NULL,
        DoseNumber          INT              NULL,
        Provider            NVARCHAR(200)    NULL,
        LotNumber           NVARCHAR(100)    NULL,
        ExpirationDate      DATETIME2        NULL,
        Site                NVARCHAR(100)    NULL,
        Route               NVARCHAR(50)     NULL,
        Notes               NVARCHAR(MAX)    NULL,
        
        -- FHIR Integration
        FhirImmunizationId  NVARCHAR(100)    NULL,
        VaccineCode         NVARCHAR(50)     NULL,
        Status              NVARCHAR(50)     NOT NULL DEFAULT 'Completed',
        
        -- System Fields
        CreatedAt           DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt           DATETIME2        NULL,
        CreatedBy           NVARCHAR(100)    NOT NULL DEFAULT 'system',
        UpdatedBy           NVARCHAR(100)    NULL,
        IsActive            BIT              NOT NULL DEFAULT 1,
        
        CONSTRAINT PK_Immunizations PRIMARY KEY CLUSTERED (ImmunizationId),
        CONSTRAINT FK_Immunizations_Patient FOREIGN KEY (PatientId) 
            REFERENCES pt.Patients(PatientId) ON DELETE CASCADE
    );
    
    CREATE NONCLUSTERED INDEX IX_Immunizations_PatientId
        ON pt.Immunizations(PatientId)
        INCLUDE (VaccineName, AdministeredDate, Status);
    
    CREATE NONCLUSTERED INDEX IX_Immunizations_AdministeredDate
        ON pt.Immunizations(AdministeredDate DESC)
        INCLUDE (PatientId, VaccineName);
    
    PRINT '✓ Immunizations table created successfully';
END
ELSE
BEGIN
    PRINT '✓ Immunizations table already exists';
END
GO

-- =============================================
-- STEP 3: Provider Types Lookup Table
-- =============================================
PRINT '';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';
PRINT 'STEP 3: Creating Provider Types Lookup Table...';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProviderTypes' AND schema_id = SCHEMA_ID('pt'))
BEGIN
    CREATE TABLE pt.ProviderTypes (
        ProviderTypeId  INT             IDENTITY(1,1) NOT NULL,
        Name            NVARCHAR(100)   NOT NULL,
        Description     NVARCHAR(500)   NULL,
        DisplayOrder    INT             NOT NULL DEFAULT 0,
        IsActive        BIT             NOT NULL DEFAULT 1,
        CreatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt       DATETIME2       NULL,
        
        CONSTRAINT PK_ProviderTypes PRIMARY KEY CLUSTERED (ProviderTypeId),
        CONSTRAINT UQ_ProviderTypes_Name UNIQUE (Name)
    );
    
    PRINT '✓ ProviderTypes table created successfully';
    
    -- Seed provider types
    SET IDENTITY_INSERT pt.ProviderTypes ON;
    
    INSERT INTO pt.ProviderTypes (ProviderTypeId, Name, Description, DisplayOrder, IsActive)
    VALUES
        (1, 'Primary Care', 'Primary Care Physician (PCP) - General/Family Medicine', 1, 1),
        (2, 'Dental', 'Dental Care Provider - Dentist/Orthodontist', 2, 1),
        (3, 'Eye Care', 'Optometrist or Ophthalmologist', 3, 1),
        (4, 'Specialist', 'Medical Specialist (Cardiologist, Dermatologist, etc.)', 4, 1),
        (5, 'Orthopedic', 'Orthopedic Surgeon or Specialist', 5, 1),
        (6, 'Pediatrics', 'Pediatrician - Children''s Healthcare', 6, 1),
        (7, 'OB/GYN', 'Obstetrician/Gynecologist - Women''s Health', 7, 1),
        (8, 'Mental Health', 'Psychiatrist, Psychologist, or Therapist', 8, 1),
        (9, 'Physical Therapy', 'Physical Therapist or Rehabilitation', 9, 1),
        (10, 'Urgent Care', 'Urgent Care or Walk-in Clinic', 10, 1),
        (11, 'Hospital', 'Hospital or Medical Center', 11, 1),
        (12, 'Other', 'Other Healthcare Provider', 99, 1);
    
    SET IDENTITY_INSERT pt.ProviderTypes OFF;
    
    PRINT '✓ ProviderTypes seeded with 12 types';
END
ELSE
BEGIN
    PRINT '✓ ProviderTypes table already exists';
END
GO

-- =============================================
-- STEP 4: Patient Providers Table
-- =============================================
PRINT '';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';
PRINT 'STEP 4: Creating Patient Providers Table...';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PatientProviders' AND schema_id = SCHEMA_ID('pt'))
BEGIN
    CREATE TABLE pt.PatientProviders (
        PatientProviderId   UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        PatientId           UNIQUEIDENTIFIER NOT NULL,
        ProviderTypeId      INT              NOT NULL,
        ProviderTypeName    NVARCHAR(100)    NOT NULL,
        
        -- Provider Details
        ProviderName        NVARCHAR(200)    NOT NULL,
        Specialty           NVARCHAR(200)    NULL,
        NPI                 NVARCHAR(10)     NULL,
        
        -- Contact Information
        Phone               NVARCHAR(25)     NULL,
        Fax                 NVARCHAR(25)     NULL,
        Email               NVARCHAR(100)    NULL,
        
        -- Address
        AddressLine1        NVARCHAR(200)    NULL,
        AddressLine2        NVARCHAR(200)    NULL,
        City                NVARCHAR(100)    NULL,
        State               NVARCHAR(50)     NULL,
        ZipCode             NVARCHAR(20)     NULL,
        Country             NVARCHAR(100)    NULL DEFAULT 'USA',
        
        -- Additional Details
        PracticeName        NVARCHAR(200)    NULL,
        Website             NVARCHAR(500)    NULL,
        Notes               NVARCHAR(MAX)    NULL,
        
        -- Status
        IsPrimary           BIT              NOT NULL DEFAULT 0,
        IsAcceptingPatients BIT              NOT NULL DEFAULT 1,
        Status              NVARCHAR(50)     NOT NULL DEFAULT 'Active',
        
        -- Dates
        FirstVisitDate      DATETIME2        NULL,
        LastVisitDate       DATETIME2        NULL,
        
        -- FHIR Integration
        FhirPractitionerId  NVARCHAR(100)    NULL,
        FhirOrganizationId  NVARCHAR(100)    NULL,
        
        -- System Fields
        CreatedAt           DATETIME2        NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt           DATETIME2        NULL,
        CreatedBy           NVARCHAR(100)    NOT NULL DEFAULT 'system',
        UpdatedBy           NVARCHAR(100)    NULL,
        IsActive            BIT              NOT NULL DEFAULT 1,
        
        CONSTRAINT PK_PatientProviders PRIMARY KEY CLUSTERED (PatientProviderId),
        CONSTRAINT FK_PatientProviders_Patient FOREIGN KEY (PatientId) 
            REFERENCES pt.Patients(PatientId) ON DELETE CASCADE,
        CONSTRAINT FK_PatientProviders_ProviderType FOREIGN KEY (ProviderTypeId) 
            REFERENCES pt.ProviderTypes(ProviderTypeId)
    );
    
    CREATE NONCLUSTERED INDEX IX_PatientProviders_PatientId
        ON pt.PatientProviders(PatientId)
        INCLUDE (ProviderTypeId, ProviderName, IsPrimary, Status);
    
    CREATE NONCLUSTERED INDEX IX_PatientProviders_ProviderType
        ON pt.PatientProviders(ProviderTypeId, PatientId)
        INCLUDE (ProviderName, IsPrimary);
    
    CREATE NONCLUSTERED INDEX IX_PatientProviders_NPI
        ON pt.PatientProviders(NPI)
        WHERE NPI IS NOT NULL;
    
    PRINT '✓ PatientProviders table created successfully';
END
ELSE
BEGIN
    PRINT '✓ PatientProviders table already exists';
END
GO

-- =============================================
-- STEP 5: Social History Enhancements
-- =============================================
PRINT '';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';
PRINT 'STEP 5: Enhancing Social History Table...';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';

-- Add ExerciseFrequency
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('pt.SocialHistory') AND name = 'ExerciseFrequency')
BEGIN
    ALTER TABLE pt.SocialHistory ADD ExerciseFrequency NVARCHAR(50) NULL;
    PRINT '✓ Added ExerciseFrequency column';
END
ELSE
BEGIN
    PRINT '  • ExerciseFrequency column already exists';
END

-- Add Diet
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('pt.SocialHistory') AND name = 'Diet')
BEGIN
    ALTER TABLE pt.SocialHistory ADD Diet NVARCHAR(100) NULL;
    PRINT '✓ Added Diet column';
END
ELSE
BEGIN
    PRINT '  • Diet column already exists';
END

-- Add StressLevel
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('pt.SocialHistory') AND name = 'StressLevel')
BEGIN
    ALTER TABLE pt.SocialHistory ADD StressLevel NVARCHAR(50) NULL;
    PRINT '✓ Added StressLevel column';
END
ELSE
BEGIN
    PRINT '  • StressLevel column already exists';
END

-- Add SleepHours
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('pt.SocialHistory') AND name = 'SleepHours')
BEGIN
    ALTER TABLE pt.SocialHistory ADD SleepHours INT NULL;
    PRINT '✓ Added SleepHours column';
END
ELSE
BEGIN
    PRINT '  • SleepHours column already exists';
END

PRINT '✓ Social History enhancements complete';
GO

-- =============================================
-- VERIFICATION SECTION
-- =============================================
PRINT '';
PRINT '╔═══════════════════════════════════════════════════════════════╗';
PRINT '║                    VERIFICATION REPORT                        ║';
PRINT '╚═══════════════════════════════════════════════════════════════╝';
PRINT '';

-- Check ChronicConditions
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ChronicConditions' AND schema_id = SCHEMA_ID('pt'))
    PRINT '✓ pt.ChronicConditions table exists'
ELSE
    PRINT '✗ pt.ChronicConditions table MISSING';

-- Check Immunizations
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Immunizations' AND schema_id = SCHEMA_ID('pt'))
    PRINT '✓ pt.Immunizations table exists'
ELSE
    PRINT '✗ pt.Immunizations table MISSING';

-- Check ProviderTypes
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ProviderTypes' AND schema_id = SCHEMA_ID('pt'))
BEGIN
    DECLARE @ProviderTypeCount INT;
    SELECT @ProviderTypeCount = COUNT(*) FROM pt.ProviderTypes;
    PRINT '✓ pt.ProviderTypes table exists (' + CAST(@ProviderTypeCount AS NVARCHAR(10)) + ' types)';
END
ELSE
    PRINT '✗ pt.ProviderTypes table MISSING';

-- Check PatientProviders
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'PatientProviders' AND schema_id = SCHEMA_ID('pt'))
    PRINT '✓ pt.PatientProviders table exists'
ELSE
    PRINT '✗ pt.PatientProviders table MISSING';

-- Check SocialHistory columns
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('pt.SocialHistory') AND name = 'ExerciseFrequency')
    PRINT '✓ pt.SocialHistory.ExerciseFrequency column exists'
ELSE
    PRINT '✗ pt.SocialHistory.ExerciseFrequency column MISSING';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('pt.SocialHistory') AND name = 'Diet')
    PRINT '✓ pt.SocialHistory.Diet column exists'
ELSE
    PRINT '✗ pt.SocialHistory.Diet column MISSING';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('pt.SocialHistory') AND name = 'StressLevel')
    PRINT '✓ pt.SocialHistory.StressLevel column exists'
ELSE
    PRINT '✗ pt.SocialHistory.StressLevel column MISSING';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('pt.SocialHistory') AND name = 'SleepHours')
    PRINT '✓ pt.SocialHistory.SleepHours column exists'
ELSE
    PRINT '✗ pt.SocialHistory.SleepHours column MISSING';

PRINT '';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';
PRINT 'MIGRATION SUMMARY';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';
PRINT 'Tables Created/Enhanced: 5';
PRINT '  • ChronicConditions';
PRINT '  • Immunizations';
PRINT '  • ProviderTypes (with 12 seeded types)';
PRINT '  • PatientProviders';
PRINT '  • SocialHistory (4 new columns)';
PRINT '';
PRINT 'Indexes Created: 7';
PRINT 'Foreign Keys Created: 4';
PRINT 'Lookup Data Seeded: 12 provider types';
PRINT '━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━';
PRINT '';
PRINT '╔═══════════════════════════════════════════════════════════════╗';
PRINT '║              ✓ MIGRATION COMPLETED SUCCESSFULLY              ║';
PRINT '╚═══════════════════════════════════════════════════════════════╝';
PRINT '';
PRINT 'Next Steps:';
PRINT '  1. Restart your API service to apply Entity Framework changes';
PRINT '  2. Test the new features in the Patient Portal';
PRINT '  3. Verify data can be saved and retrieved correctly';
PRINT '';

GO

