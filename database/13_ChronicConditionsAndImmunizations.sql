-- =============================================
-- Chronic Conditions and Immunizations Tables
-- Author: System
-- Create date: 2025-10-19
-- Description: Tables for chronic conditions and immunizations
-- =============================================

USE EmrMaster;
GO

-- =============================================
-- Chronic Conditions Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[pt].[ChronicConditions]') AND type in (N'U'))
BEGIN
    CREATE TABLE [pt].[ChronicConditions] (
        [ConditionId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [PatientId] UNIQUEIDENTIFIER NOT NULL,
        [ConditionName] NVARCHAR(200) NOT NULL,
        [DiagnosedDate] DATE NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'Active', -- Active, Inactive, Resolved
        [Notes] NVARCHAR(MAX) NULL,
        
        -- System Fields
        [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2(7) NULL,
        [CreatedBy] NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,
        [UpdatedBy] NVARCHAR(100) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        
        CONSTRAINT [PK_ChronicConditions] PRIMARY KEY CLUSTERED ([ConditionId] ASC),
        CONSTRAINT [FK_ChronicConditions_Patient] FOREIGN KEY ([PatientId]) 
            REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
    );
    
    CREATE NONCLUSTERED INDEX [IX_ChronicConditions_PatientId] 
        ON [pt].[ChronicConditions] ([PatientId]) 
        INCLUDE ([ConditionName], [Status], [IsActive]);
        
    PRINT 'Table [pt].[ChronicConditions] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [pt].[ChronicConditions] already exists.';
END
GO

-- =============================================
-- Immunizations Table
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[pt].[Immunizations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [pt].[Immunizations] (
        [ImmunizationId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [PatientId] UNIQUEIDENTIFIER NOT NULL,
        [VaccineName] NVARCHAR(200) NOT NULL,
        [AdministeredDate] DATE NOT NULL,
        [DoseNumber] INT NULL,
        [Provider] NVARCHAR(200) NULL,
        [LotNumber] NVARCHAR(100) NULL,
        [ExpirationDate] DATE NULL,
        [Site] NVARCHAR(100) NULL, -- Injection site (e.g., Left arm, Right deltoid)
        [Route] NVARCHAR(50) NULL, -- Route of administration (e.g., Intramuscular, Oral)
        [Notes] NVARCHAR(MAX) NULL,
        
        -- System Fields
        [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2(7) NULL,
        [CreatedBy] NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,
        [UpdatedBy] NVARCHAR(100) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        
        CONSTRAINT [PK_Immunizations] PRIMARY KEY CLUSTERED ([ImmunizationId] ASC),
        CONSTRAINT [FK_Immunizations_Patient] FOREIGN KEY ([PatientId]) 
            REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
    );
    
    CREATE NONCLUSTERED INDEX [IX_Immunizations_PatientId] 
        ON [pt].[Immunizations] ([PatientId]) 
        INCLUDE ([VaccineName], [AdministeredDate], [IsActive]);
        
    CREATE NONCLUSTERED INDEX [IX_Immunizations_AdministeredDate] 
        ON [pt].[Immunizations] ([AdministeredDate] DESC);
        
    PRINT 'Table [pt].[Immunizations] created successfully.';
END
ELSE
BEGIN
    PRINT 'Table [pt].[Immunizations] already exists.';
END
GO

-- =============================================
-- Sample Data for Testing
-- =============================================
PRINT 'Inserting sample data for chronic conditions and immunizations...';

-- Get a sample patient ID
DECLARE @SamplePatientId UNIQUEIDENTIFIER;
SELECT TOP 1 @SamplePatientId = PatientId FROM [pt].[Patients] WHERE IsActive = 1;

IF @SamplePatientId IS NOT NULL
BEGIN
    -- Sample Chronic Conditions
    IF NOT EXISTS (SELECT 1 FROM [pt].[ChronicConditions] WHERE PatientId = @SamplePatientId)
    BEGIN
        INSERT INTO [pt].[ChronicConditions] (PatientId, ConditionName, DiagnosedDate, Status, Notes)
        VALUES 
            (@SamplePatientId, 'Hypertension', '2020-03-15', 'Active', 'Well controlled with medication'),
            (@SamplePatientId, 'Type 2 Diabetes', '2019-08-22', 'Active', 'Managed with diet and medication');
        
        PRINT 'Sample chronic conditions inserted.';
    END
    
    -- Sample Immunizations
    IF NOT EXISTS (SELECT 1 FROM [pt].[Immunizations] WHERE PatientId = @SamplePatientId)
    BEGIN
        INSERT INTO [pt].[Immunizations] (PatientId, VaccineName, AdministeredDate, DoseNumber, Provider, Site, Route)
        VALUES 
            (@SamplePatientId, 'COVID-19 (Pfizer-BioNTech)', '2021-04-10', 1, 'CVS Pharmacy', 'Left deltoid', 'Intramuscular'),
            (@SamplePatientId, 'COVID-19 (Pfizer-BioNTech)', '2021-05-01', 2, 'CVS Pharmacy', 'Left deltoid', 'Intramuscular'),
            (@SamplePatientId, 'Influenza', '2024-09-15', 1, 'Primary Care Clinic', 'Right deltoid', 'Intramuscular'),
            (@SamplePatientId, 'Tdap (Tetanus, Diphtheria, Pertussis)', '2022-06-20', 1, 'Primary Care Clinic', 'Left arm', 'Intramuscular');
        
        PRINT 'Sample immunizations inserted.';
    END
END
ELSE
BEGIN
    PRINT 'No sample patient found. Skipping sample data insertion.';
END
GO

PRINT 'Chronic Conditions and Immunizations tables setup completed.';
GO

