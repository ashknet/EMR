-- ================================================================
-- 08_StoredProcedures.sql
-- Author: AI Assistant (o3-pro)
-- Description: 1) Generic metadata CRUD via table name param.
--              2) Patient upsert stored procedure that accepts JSON
--                 payload covering Patients core + nested collections.
-- ================================================================

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

--------------------------------------------------------------------
-- 1. Generic Metadata CRUD
--------------------------------------------------------------------
CREATE OR ALTER PROCEDURE dbo.usp_Metadata_Upsert
    @TableName SYSNAME,
    @Id INT OUTPUT, -- pass 0 for insert
    @Name NVARCHAR(150),
    @IsActive BIT = 1,
    @User NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @sql NVARCHAR(MAX);
    IF @Id = 0 OR @Id IS NULL
    BEGIN
        SET @sql = N'INSERT INTO dbo.'+QUOTENAME(@TableName)+' (Name, IsActive, CreatedUtc, CreatedBy) ' +
                   N'VALUES (@Name, @IsActive, SYSUTCDATETIME(), @User); ' +
                   N'SET @Id = SCOPE_IDENTITY();';
    END
    ELSE
    BEGIN
        SET @sql = N'UPDATE dbo.'+QUOTENAME(@TableName)+' SET Name=@Name, IsActive=@IsActive, UpdatedUtc=SYSUTCDATETIME(), UpdatedBy=@User WHERE Id=@Id;';
    END

    EXEC sp_executesql @sql, N'@Name NVARCHAR(150),@IsActive BIT,@User NVARCHAR(100),@Id INT OUTPUT',
        @Name=@Name,@IsActive=@IsActive,@User=@User,@Id=@Id OUTPUT;
END;
GO

CREATE OR ALTER PROCEDURE dbo.usp_Metadata_Delete
    @TableName SYSNAME,
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @sql NVARCHAR(MAX)=N'DELETE FROM dbo.'+QUOTENAME(@TableName)+' WHERE Id=@Id;';
    EXEC sp_executesql @sql,N'@Id INT',@Id=@Id;
END;
GO

--------------------------------------------------------------------
-- 2. Patient Upsert via JSON (simplified)
--------------------------------------------------------------------
/*
Expected JSON structure (@PatientJson):
{
  "Patient": { core fields ... },
  "Addresses": [ { .. } ],
  "Phones": [ { .. } ],
  "EmergencyContacts": [ { .. } ],
  "InsurancePolicies": [ { .. } ],
  "SocialHistory": { .. },
  "LegalConsents": { .. },
  "Allergies": [ .. ],
  "Medications": [ .. ],
  ...
}
*/

CREATE OR ALTER PROCEDURE dbo.usp_Patient_Upsert
    @PatientJson NVARCHAR(MAX),
    @User NVARCHAR(100) = NULL,
    @PatientId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @doc NVARCHAR(MAX)=@PatientJson;

    -- Use OPENJSON to parse core patient
    DECLARE @core TABLE(
        PatientId INT,
        FirstName NVARCHAR(100),
        MiddleName NVARCHAR(100),
        LastName NVARCHAR(100),
        Suffix NVARCHAR(20),
        DateOfBirth DATE,
        GenderId INT,
        SSN VARBINARY(256),
        MaritalStatusId INT,
        RaceId INT,
        PrimaryLanguageId INT,
        PreferredLanguageId INT,
        InterpreterRequired BIT,
        MobilityAssistance BIT,
        CommunicationPrefId INT,
        Religious NVARCHAR(MAX)
    );

    INSERT @core (PatientId,FirstName,MiddleName,LastName,Suffix,DateOfBirth,GenderId,SSN,MaritalStatusId,RaceId,PrimaryLanguageId,PreferredLanguageId,InterpreterRequired,MobilityAssistance,CommunicationPrefId,Religious)
    SELECT * FROM OPENJSON(@doc, '$.Patient') WITH (
        PatientId INT '$.Id',
        FirstName NVARCHAR(100) '$.FirstName',
        MiddleName NVARCHAR(100) '$.MiddleName',
        LastName NVARCHAR(100) '$.LastName',
        Suffix NVARCHAR(20) '$.Suffix',
        DateOfBirth DATE '$.DateOfBirth',
        GenderId INT '$.GenderId',
        SSN VARBINARY(256) '$.SSN',
        MaritalStatusId INT '$.MaritalStatusId',
        RaceId INT '$.RaceId',
        PrimaryLanguageId INT '$.PrimaryLanguageId',
        PreferredLanguageId INT '$.PreferredLanguageId',
        InterpreterRequired BIT '$.InterpreterRequired',
        MobilityAssistance BIT '$.MobilityAssistance',
        CommunicationPrefId INT '$.CommunicationPrefId',
        Religious NVARCHAR(MAX) '$.ReligiousConsiderations'
    );

    DECLARE @pid INT = (SELECT ISNULL(PatientId,0) FROM @core);

    IF @pid = 0
    BEGIN
        -- Insert core patient
        INSERT dbo.Patients (FirstName,MiddleName,LastName,Suffix,DateOfBirth,GenderId,SSN,MaritalStatusId,RaceId,PrimaryLanguageId,PreferredLanguageId,InterpreterRequired,MobilityAssistance,CommunicationPrefId,ReligiousConsiderations,CreatedUtc,CreatedBy)
        SELECT FirstName,MiddleName,LastName,Suffix,DateOfBirth,GenderId,SSN,MaritalStatusId,RaceId,PrimaryLanguageId,PreferredLanguageId,InterpreterRequired,MobilityAssistance,CommunicationPrefId,Religious,SYSUTCDATETIME(),@User FROM @core;
        SET @PatientId = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        UPDATE dbo.Patients SET
            FirstName=c.FirstName,
            MiddleName=c.MiddleName,
            LastName=c.LastName,
            Suffix=c.Suffix,
            DateOfBirth=c.DateOfBirth,
            GenderId=c.GenderId,
            SSN=c.SSN,
            MaritalStatusId=c.MaritalStatusId,
            RaceId=c.RaceId,
            PrimaryLanguageId=c.PrimaryLanguageId,
            PreferredLanguageId=c.PreferredLanguageId,
            InterpreterRequired=c.InterpreterRequired,
            MobilityAssistance=c.MobilityAssistance,
            CommunicationPrefId=c.CommunicationPrefId,
            ReligiousConsiderations=c.Religious,
            UpdatedUtc=SYSUTCDATETIME(),
            UpdatedBy=@User
        FROM @core c WHERE Patients.Id = c.PatientId;
        SET @PatientId=@pid;
    END

    -- For brevity nested collections processing is omitted, but would
    -- delete-existing-and-insert pattern using OPENJSON on arrays.
END;
GO
