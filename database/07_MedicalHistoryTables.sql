-- ================================================================
-- 07_MedicalHistoryTables.sql
-- Author: AI Assistant (o3-pro)
-- Description: Lookup + detail tables for allergies, medications,
--              chronic conditions, surgeries, hospitalizations,
--              immunizations, test results, family medical history
-- ================================================================

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

--------------------------------------------------------------------
-- 1. Create additional lookup tables if not exist
--------------------------------------------------------------------
DECLARE @simpleLookups TABLE(Name SYSNAME, Values NVARCHAR(MAX));
INSERT INTO @simpleLookups(Name,Values)
VALUES
 ('AllergyType','Drug,Food,Environmental,Other'),
 ('Medication','Custom'),
 ('ChronicCondition','Asthma,Diabetes,Hypertension,Other'),
 ('SurgeryType','Appendectomy,Cesarean,Heart Bypass,Other'),
 ('ImmunizationType','Tetanus,Flu,COVID-19,HepB,Other'),
 ('Condition','Cancer,Stroke,Heart Disease,Diabetes,Other');

-- Helper proc
CREATE OR ALTER PROCEDURE dbo._CreateSimpleLookup
    @Table SYSNAME,
    @Csv NVARCHAR(MAX)
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = @Table)
    BEGIN
        DECLARE @sql NVARCHAR(MAX)= N'CREATE TABLE dbo.'+QUOTENAME(@Table)+N' (Id INT IDENTITY(1,1) CONSTRAINT PK_'+@Table+' PRIMARY KEY, Name NVARCHAR(150) NOT NULL CONSTRAINT UQ_'+@Table+'_Name UNIQUE, IsActive BIT NOT NULL DEFAULT(1), SortOrder INT NOT NULL DEFAULT(0));';
        EXEC (@sql);
    END;

    DECLARE @iSql NVARCHAR(MAX)='';
    WITH t AS (SELECT value, ROW_NUMBER() OVER(ORDER BY (SELECT 1))-1 rn FROM string_split(@Csv,','))
    SELECT @iSql+=N'IF NOT EXISTS (SELECT 1 FROM dbo.'+QUOTENAME(@Table)+' WHERE Name='''+LTRIM(value)+''') INSERT dbo.'+QUOTENAME(@Table)+'(Name,SortOrder) VALUES('''+LTRIM(value)+''','+CAST(rn AS NVARCHAR)+');' FROM t;
    EXEC(@iSql);
END;
GO

DECLARE @n SYSNAME, @v NVARCHAR(MAX);
DECLARE cur CURSOR FOR SELECT Name, Values FROM @simpleLookups;
OPEN cur;
FETCH NEXT FROM cur INTO @n,@v;
WHILE @@FETCH_STATUS=0
BEGIN
    EXEC dbo._CreateSimpleLookup @n,@v;
    FETCH NEXT FROM cur INTO @n,@v;
END
CLOSE cur; DEALLOCATE cur;
GO
DROP PROCEDURE dbo._CreateSimpleLookup;
GO

--------------------------------------------------------------------
-- 2. PatientAllergies (many-to-many style, allow custom text)
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PatientAllergies')
BEGIN
    CREATE TABLE dbo.PatientAllergies (
        Id INT IDENTITY(1,1) CONSTRAINT PK_PatientAllergies PRIMARY KEY,
        PatientId INT NOT NULL,
        AllergyTypeId INT NULL, -- may be null when CustomName set
        CustomName NVARCHAR(150) NULL,
        Notes NVARCHAR(500) NULL,
        CreatedUtc DATETIME2(0) NOT NULL DEFAULT(SYSUTCDATETIME()),
        CONSTRAINT FK_PA_Patient FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id) ON DELETE CASCADE,
        CONSTRAINT FK_PA_Allergy FOREIGN KEY (AllergyTypeId) REFERENCES dbo.AllergyType(Id)
    );
END;
GO

--------------------------------------------------------------------
-- 3. PatientMedications
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PatientMedications')
BEGIN
    CREATE TABLE dbo.PatientMedications (
        Id INT IDENTITY(1,1) CONSTRAINT PK_PatientMedications PRIMARY KEY,
        PatientId INT NOT NULL,
        MedicationId INT NULL,
        MedicationName NVARCHAR(150) NULL, -- free-text if not in lookup
        Dosage NVARCHAR(50) NULL,
        Frequency NVARCHAR(50) NULL,
        Prescriber NVARCHAR(150) NULL,
        CreatedUtc DATETIME2(0) NOT NULL DEFAULT(SYSUTCDATETIME()),
        CONSTRAINT FK_PM_Patient FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id) ON DELETE CASCADE,
        CONSTRAINT FK_PM_Med FOREIGN KEY (MedicationId) REFERENCES dbo.Medication(Id)
    );
END;
GO

--------------------------------------------------------------------
-- 4. PatientChronicConditions
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PatientChronicConditions')
BEGIN
    CREATE TABLE dbo.PatientChronicConditions (
        PatientId INT NOT NULL,
        ChronicConditionId INT NOT NULL,
        CONSTRAINT PK_PatientChronic PRIMARY KEY (PatientId,ChronicConditionId),
        CONSTRAINT FK_PC_Patient FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id) ON DELETE CASCADE,
        CONSTRAINT FK_PC_Condition FOREIGN KEY (ChronicConditionId) REFERENCES dbo.ChronicCondition(Id)
    );
END;
GO

--------------------------------------------------------------------
-- 5. PatientSurgeries
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PatientSurgeries')
BEGIN
    CREATE TABLE dbo.PatientSurgeries (
        Id INT IDENTITY(1,1) CONSTRAINT PK_PatientSurgeries PRIMARY KEY,
        PatientId INT NOT NULL,
        SurgeryTypeId INT NULL,
        CustomName NVARCHAR(150) NULL,
        SurgeryDate DATE NULL,
        Notes NVARCHAR(500) NULL,
        FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id) ON DELETE CASCADE,
        FOREIGN KEY (SurgeryTypeId) REFERENCES dbo.SurgeryType(Id)
    );
END;
GO

--------------------------------------------------------------------
-- 6. PatientHospitalizations
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PatientHospitalizations')
BEGIN
    CREATE TABLE dbo.PatientHospitalizations (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PatientId INT NOT NULL,
        HospitalName NVARCHAR(150) NULL,
        Reason NVARCHAR(300) NULL,
        AdmissionDate DATE NULL,
        DischargeDate DATE NULL,
        Notes NVARCHAR(500) NULL,
        FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id) ON DELETE CASCADE
    );
END;
GO

--------------------------------------------------------------------
-- 7. PatientImmunizations
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PatientImmunizations')
BEGIN
    CREATE TABLE dbo.PatientImmunizations (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PatientId INT NOT NULL,
        ImmunizationTypeId INT NULL,
        CustomName NVARCHAR(150) NULL,
        ImmunizationDate DATE NULL,
        FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id) ON DELETE CASCADE,
        FOREIGN KEY (ImmunizationTypeId) REFERENCES dbo.ImmunizationType(Id)
    );
END;
GO

--------------------------------------------------------------------
-- 8. PatientTestResults (attachments or manual entry)
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PatientTestResults')
BEGIN
    CREATE TABLE dbo.PatientTestResults (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PatientId INT NOT NULL,
        TestName NVARCHAR(150) NULL,
        ResultText NVARCHAR(MAX) NULL,
        ResultDate DATE NULL,
        AttachmentPath NVARCHAR(260) NULL,
        FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id) ON DELETE CASCADE
    );
END;
GO

--------------------------------------------------------------------
-- 9. FamilyMedicalHistory
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'FamilyMedicalHistory')
BEGIN
    CREATE TABLE dbo.FamilyMedicalHistory (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        PatientId INT NOT NULL,
        Relative NVARCHAR(100) NOT NULL,
        ConditionId INT NULL,
        CustomCondition NVARCHAR(150) NULL,
        FOREIGN KEY (PatientId) REFERENCES dbo.Patients(Id) ON DELETE CASCADE,
        FOREIGN KEY (ConditionId) REFERENCES dbo.Condition(Id)
    );
END;
GO
