-- =============================================
-- Patient Providers and Provider Types Tables
-- Created: October 19, 2025
-- Purpose: Support multiple healthcare providers per patient
-- =============================================

USE EMRMaster;
GO

-- =============================================
-- Provider Types Lookup Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProviderTypes' AND schema_id = SCHEMA_ID('pt'))
BEGIN
    CREATE TABLE pt.ProviderTypes (
        ProviderTypeId      INT             IDENTITY(1,1) NOT NULL,
        Name                NVARCHAR(100)   NOT NULL,
        Description         NVARCHAR(500)   NULL,
        DisplayOrder        INT             NOT NULL DEFAULT 0,
        IsActive            BIT             NOT NULL DEFAULT 1,
        CreatedAt           DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt           DATETIME2       NULL,
        
        CONSTRAINT PK_ProviderTypes PRIMARY KEY CLUSTERED (ProviderTypeId),
        CONSTRAINT UQ_ProviderTypes_Name UNIQUE (Name)
    );
    
    PRINT 'Table pt.ProviderTypes created successfully.';
END
ELSE
BEGIN
    PRINT 'Table pt.ProviderTypes already exists.';
END
GO

-- =============================================
-- Patient Providers Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PatientProviders' AND schema_id = SCHEMA_ID('pt'))
BEGIN
    CREATE TABLE pt.PatientProviders (
        PatientProviderId   UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        PatientId           UNIQUEIDENTIFIER NOT NULL,
        ProviderTypeId      INT             NOT NULL,
        ProviderTypeName    NVARCHAR(100)   NOT NULL,
        
        -- Provider Details
        ProviderName        NVARCHAR(200)   NOT NULL,
        Specialty           NVARCHAR(200)   NULL,
        NPI                 NVARCHAR(10)    NULL, -- National Provider Identifier
        
        -- Contact Information
        Phone               NVARCHAR(25)    NULL,
        Fax                 NVARCHAR(25)    NULL,
        Email               NVARCHAR(100)   NULL,
        
        -- Address
        AddressLine1        NVARCHAR(200)   NULL,
        AddressLine2        NVARCHAR(200)   NULL,
        City                NVARCHAR(100)   NULL,
        State               NVARCHAR(50)    NULL,
        ZipCode             NVARCHAR(20)    NULL,
        Country             NVARCHAR(100)   NULL DEFAULT 'USA',
        
        -- Additional Details
        PracticeName        NVARCHAR(200)   NULL,
        Website             NVARCHAR(500)   NULL,
        Notes               NVARCHAR(MAX)   NULL,
        
        -- Status
        IsPrimary           BIT             NOT NULL DEFAULT 0,
        IsAcceptingPatients BIT             NOT NULL DEFAULT 1,
        Status              NVARCHAR(50)    NOT NULL DEFAULT 'Active', -- Active, Inactive, Retired
        
        -- Dates
        FirstVisitDate      DATETIME2       NULL,
        LastVisitDate       DATETIME2       NULL,
        
        -- FHIR Integration
        FhirPractitionerId  NVARCHAR(100)   NULL,
        FhirOrganizationId  NVARCHAR(100)   NULL,
        
        -- System Fields
        CreatedAt           DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt           DATETIME2       NULL,
        CreatedBy           NVARCHAR(100)   NOT NULL DEFAULT 'system',
        UpdatedBy           NVARCHAR(100)   NULL,
        IsActive            BIT             NOT NULL DEFAULT 1,
        
        CONSTRAINT PK_PatientProviders PRIMARY KEY CLUSTERED (PatientProviderId),
        CONSTRAINT FK_PatientProviders_Patient FOREIGN KEY (PatientId) 
            REFERENCES pt.Patients(PatientId) ON DELETE CASCADE,
        CONSTRAINT FK_PatientProviders_ProviderType FOREIGN KEY (ProviderTypeId) 
            REFERENCES pt.ProviderTypes(ProviderTypeId)
    );
    
    PRINT 'Table pt.PatientProviders created successfully.';
END
ELSE
BEGIN
    PRINT 'Table pt.PatientProviders already exists.';
END
GO

-- =============================================
-- Indexes for Performance
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PatientProviders_PatientId' AND object_id = OBJECT_ID('pt.PatientProviders'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_PatientProviders_PatientId
        ON pt.PatientProviders(PatientId)
        INCLUDE (ProviderTypeId, ProviderName, IsPrimary, Status);
    
    PRINT 'Index IX_PatientProviders_PatientId created successfully.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PatientProviders_ProviderType' AND object_id = OBJECT_ID('pt.PatientProviders'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_PatientProviders_ProviderType
        ON pt.PatientProviders(ProviderTypeId, PatientId)
        INCLUDE (ProviderName, IsPrimary);
    
    PRINT 'Index IX_PatientProviders_ProviderType created successfully.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PatientProviders_NPI' AND object_id = OBJECT_ID('pt.PatientProviders'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_PatientProviders_NPI
        ON pt.PatientProviders(NPI)
        WHERE NPI IS NOT NULL;
    
    PRINT 'Index IX_PatientProviders_NPI created successfully.';
END
GO

-- =============================================
-- Seed Provider Types Data
-- =============================================
IF NOT EXISTS (SELECT * FROM pt.ProviderTypes)
BEGIN
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
    
    PRINT 'Provider Types seeded successfully.';
END
ELSE
BEGIN
    PRINT 'Provider Types already have data.';
END
GO

-- =============================================
-- Verification Queries
-- =============================================
PRINT '';
PRINT '=== Verification ===';
PRINT 'Provider Types Count: ' + CAST((SELECT COUNT(*) FROM pt.ProviderTypes) AS NVARCHAR(10));
PRINT '';
PRINT 'Available Provider Types:';
SELECT 
    ProviderTypeId,
    Name,
    Description,
    DisplayOrder
FROM pt.ProviderTypes
ORDER BY DisplayOrder;
GO

PRINT '';
PRINT 'Patient Providers table structure:';
EXEC sp_help 'pt.PatientProviders';
GO

