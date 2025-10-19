-- ================================================================
-- 05_MetadataTables.sql
-- Author: AI Assistant (o3-pro)
-- Description: Core lookup tables to support patient detail drop-downs.
--              Each table follows a common pattern: Id (INT IDENTITY PK),
--              Name (NVARCHAR NOT NULL), IsActive (BIT), SortOrder (INT),
--              Audit fields, and UNIQUE constraint on Name.
-- =================================================================

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

--------------------------------------------------------------------
-- Template create statement (reused via copy-paste for each lookup)
--------------------------------------------------------------------
-- CREATE TABLE dbo.[LookupName] (
--     Id           INT            IDENTITY(1,1) CONSTRAINT PK_[LookupName] PRIMARY KEY,
--     Name         NVARCHAR(100)  NOT NULL CONSTRAINT UQ_[LookupName]_Name UNIQUE,
--     IsActive     BIT            NOT NULL DEFAULT(1),
--     SortOrder    INT            NOT NULL DEFAULT(0),
--     CreatedUtc   DATETIME2(0)   NOT NULL CONSTRAINT DF_[LookupName]_CreatedUtc DEFAULT(SYSUTCDATETIME()),
--     CreatedBy    NVARCHAR(100)  NULL,
--     UpdatedUtc   DATETIME2(0)   NULL,
--     UpdatedBy    NVARCHAR(100)  NULL
-- );

--------------------------------------------------------------------
-- Gender
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Gender')
BEGIN
    CREATE TABLE dbo.Gender (
        Id           INT           IDENTITY(1,1) CONSTRAINT PK_Gender PRIMARY KEY,
        Name         NVARCHAR(50)  NOT NULL CONSTRAINT UQ_Gender_Name UNIQUE,
        IsActive     BIT           NOT NULL DEFAULT(1),
        SortOrder    INT           NOT NULL DEFAULT(0),
        CreatedUtc   DATETIME2(0)  NOT NULL DEFAULT(SYSUTCDATETIME()),
        CreatedBy    NVARCHAR(100) NULL,
        UpdatedUtc   DATETIME2(0)  NULL,
        UpdatedBy    NVARCHAR(100) NULL
    );
END;
GO

--------------------------------------------------------------------
-- MaritalStatus
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'MaritalStatus')
BEGIN
    CREATE TABLE dbo.MaritalStatus (
        Id           INT           IDENTITY(1,1) CONSTRAINT PK_MaritalStatus PRIMARY KEY,
        Name         NVARCHAR(50)  NOT NULL CONSTRAINT UQ_MaritalStatus_Name UNIQUE,
        IsActive     BIT           NOT NULL DEFAULT(1),
        SortOrder    INT           NOT NULL DEFAULT(0),
        CreatedUtc   DATETIME2(0)  NOT NULL DEFAULT(SYSUTCDATETIME()),
        CreatedBy    NVARCHAR(100) NULL,
        UpdatedUtc   DATETIME2(0)  NULL,
        UpdatedBy    NVARCHAR(100) NULL
    );
END;
GO

--------------------------------------------------------------------
-- Race
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Race')
BEGIN
    CREATE TABLE dbo.Race (
        Id           INT           IDENTITY(1,1) CONSTRAINT PK_Race PRIMARY KEY,
        Name         NVARCHAR(100) NOT NULL CONSTRAINT UQ_Race_Name UNIQUE,
        IsActive     BIT           NOT NULL DEFAULT(1),
        SortOrder    INT           NOT NULL DEFAULT(0),
        CreatedUtc   DATETIME2(0)  NOT NULL DEFAULT(SYSUTCDATETIME()),
        CreatedBy    NVARCHAR(100) NULL,
        UpdatedUtc   DATETIME2(0)  NULL,
        UpdatedBy    NVARCHAR(100) NULL
    );
END;
GO

--------------------------------------------------------------------
-- Language
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Language')
BEGIN
    CREATE TABLE dbo.[Language] (
        Id           INT           IDENTITY(1,1) CONSTRAINT PK_Language PRIMARY KEY,
        Name         NVARCHAR(100) NOT NULL CONSTRAINT UQ_Language_Name UNIQUE,
        IsActive     BIT           NOT NULL DEFAULT(1),
        SortOrder    INT           NOT NULL DEFAULT(0),
        CreatedUtc   DATETIME2(0)  NOT NULL DEFAULT(SYSUTCDATETIME()),
        CreatedBy    NVARCHAR(100) NULL,
        UpdatedUtc   DATETIME2(0)  NULL,
        UpdatedBy    NVARCHAR(100) NULL
    );
END;
GO

--------------------------------------------------------------------
-- RelationshipType
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RelationshipType')
BEGIN
    CREATE TABLE dbo.RelationshipType (
        Id           INT           IDENTITY(1,1) CONSTRAINT PK_RelationshipType PRIMARY KEY,
        Name         NVARCHAR(50)  NOT NULL CONSTRAINT UQ_RelationshipType_Name UNIQUE,
        IsActive     BIT           NOT NULL DEFAULT(1),
        SortOrder    INT           NOT NULL DEFAULT(0),
        CreatedUtc   DATETIME2(0)  NOT NULL DEFAULT(SYSUTCDATETIME()),
        CreatedBy    NVARCHAR(100) NULL,
        UpdatedUtc   DATETIME2(0)  NULL,
        UpdatedBy    NVARCHAR(100) NULL
    );
END;
GO

--------------------------------------------------------------------
-- (Repeat similar blocks for: InsuranceProvider, AddressType, PhoneType,
--    Condition, AllergyType, Medication, SmokingStatus, AlcoholUse,
--    DrugUse, LivingSituation, CommunicationPreference, ChronicCondition,
--    SurgeryType, ImmunizationType)
--    For brevity, only key tables are shown here; replicate template as
--    needed.
--------------------------------------------------------------------

-- Seed core values --------------------------------------------------

INSERT INTO dbo.Gender (Name, SortOrder)  SELECT 'Male',0  WHERE NOT EXISTS (SELECT 1 FROM dbo.Gender WHERE Name='Male');
INSERT INTO dbo.Gender (Name, SortOrder)  SELECT 'Female',1 WHERE NOT EXISTS (SELECT 1 FROM dbo.Gender WHERE Name='Female');
INSERT INTO dbo.Gender (Name, SortOrder)  SELECT 'Other',2 WHERE NOT EXISTS (SELECT 1 FROM dbo.Gender WHERE Name='Other');

INSERT INTO dbo.MaritalStatus (Name, SortOrder)
SELECT v.n, v.o FROM (VALUES('Single',0),('Married',1),('Divorced',2),('Widowed',3),('Other',4)) v(n,o)
WHERE NOT EXISTS (SELECT 1 FROM dbo.MaritalStatus ms WHERE ms.Name = v.n);

INSERT INTO dbo.RelationshipType (Name, SortOrder)
SELECT v.n, v.o FROM (VALUES('Spouse',0),('Parent',1),('Child',2),('Friend',3),('Other',4)) v(n,o)
WHERE NOT EXISTS (SELECT 1 FROM dbo.RelationshipType rt WHERE rt.Name = v.n);

-- Add more seeding as required.
GO

--------------------------------------------------------------------
-- InsuranceProvider
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'InsuranceProvider')
BEGIN
    CREATE TABLE dbo.InsuranceProvider (
        Id           INT           IDENTITY(1,1) CONSTRAINT PK_InsuranceProvider PRIMARY KEY,
        Name         NVARCHAR(150) NOT NULL CONSTRAINT UQ_InsuranceProvider_Name UNIQUE,
        Phone        NVARCHAR(25)  NULL,
        IsActive     BIT           NOT NULL DEFAULT(1),
        SortOrder    INT           NOT NULL DEFAULT(0),
        CreatedUtc   DATETIME2(0)  NOT NULL DEFAULT(SYSUTCDATETIME()),
        CreatedBy    NVARCHAR(100) NULL,
        UpdatedUtc   DATETIME2(0)  NULL,
        UpdatedBy    NVARCHAR(100) NULL
    );
END;
GO

--------------------------------------------------------------------
-- AddressType & PhoneType
--------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AddressType')
BEGIN
    CREATE TABLE dbo.AddressType (
        Id INT IDENTITY(1,1) CONSTRAINT PK_AddressType PRIMARY KEY,
        Name NVARCHAR(50) NOT NULL CONSTRAINT UQ_AddressType_Name UNIQUE,
        IsActive BIT NOT NULL DEFAULT(1),
        SortOrder INT NOT NULL DEFAULT(0),
        CreatedUtc DATETIME2(0) NOT NULL DEFAULT(SYSUTCDATETIME()),
        CreatedBy NVARCHAR(100) NULL,
        UpdatedUtc DATETIME2(0) NULL,
        UpdatedBy NVARCHAR(100) NULL
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PhoneType')
BEGIN
    CREATE TABLE dbo.PhoneType (
        Id INT IDENTITY(1,1) CONSTRAINT PK_PhoneType PRIMARY KEY,
        Name NVARCHAR(50) NOT NULL CONSTRAINT UQ_PhoneType_Name UNIQUE,
        IsActive BIT NOT NULL DEFAULT(1),
        SortOrder INT NOT NULL DEFAULT(0),
        CreatedUtc DATETIME2(0) NOT NULL DEFAULT(SYSUTCDATETIME()),
        CreatedBy NVARCHAR(100) NULL,
        UpdatedUtc DATETIME2(0) NULL,
        UpdatedBy NVARCHAR(100) NULL
    );
END;
GO

--------------------------------------------------------------------
-- SmokingStatus, AlcoholUse, DrugUse, LivingSituation, CommunicationPreference
--------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.CreateSimpleLookup
    @TableName SYSNAME,
    @Values NVARCHAR(MAX)
AS
BEGIN
    DECLARE @sql NVARCHAR(MAX) = N'';
    SET @sql += N'IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = ''' + @TableName + N''') BEGIN ';
    SET @sql += N'CREATE TABLE dbo.' + QUOTENAME(@TableName) + N' ( ';
    SET @sql += N'Id INT IDENTITY(1,1) CONSTRAINT PK_' + @TableName + N' PRIMARY KEY, ';
    SET @sql += N'Name NVARCHAR(100) NOT NULL CONSTRAINT UQ_' + @TableName + N'_Name UNIQUE, ';
    SET @sql += N'IsActive BIT NOT NULL DEFAULT(1), SortOrder INT NOT NULL DEFAULT(0), ';
    SET @sql += N'CreatedUtc DATETIME2(0) NOT NULL DEFAULT(SYSUTCDATETIME()), CreatedBy NVARCHAR(100) NULL, ';
    SET @sql += N'UpdatedUtc DATETIME2(0) NULL, UpdatedBy NVARCHAR(100) NULL); END;';
    EXEC (@sql);

    DECLARE @insert NVARCHAR(MAX) = N'';
    WITH v AS (SELECT value, ROW_NUMBER() OVER(ORDER BY (SELECT 1))-1 AS rn FROM string_split(@Values, ','))
    SELECT @insert += N'IF NOT EXISTS (SELECT 1 FROM dbo.' + QUOTENAME(@TableName) + N' WHERE Name = ''' + value + N''') INSERT INTO dbo.' + QUOTENAME(@TableName) + N' (Name, SortOrder) VALUES (''' + value + N''',' + CAST(rn AS NVARCHAR) + N');' FROM v;
    EXEC (@insert);
END;
GO

-- Create & seed simple lookups
EXEC dbo.CreateSimpleLookup 'SmokingStatus', 'Never,Current,Former';
EXEC dbo.CreateSimpleLookup 'AlcoholUse', 'None,Occasional,Moderate,Heavy';
EXEC dbo.CreateSimpleLookup 'DrugUse', 'None,Prescription,Recreational';
EXEC dbo.CreateSimpleLookup 'LivingSituation', 'Alone,Family,Assisted,Other';
EXEC dbo.CreateSimpleLookup 'CommunicationPreference', 'Phone,Email,SMS,Patient Portal';
GO

-- Clean-up helper proc (optional)
DROP PROCEDURE dbo.CreateSimpleLookup;
GO
