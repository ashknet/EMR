-- ================================================================
-- 12_OptimizedStoredProcedures.sql
-- Description: High-performance stored procedures for critical operations
-- Replaces EF Core queries with optimized T-SQL for better performance
-- ================================================================

USE HealthcarePlatform;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

--------------------------------------------------------------------
-- 1. Profile Completeness Check (High Frequency Operation)
--------------------------------------------------------------------
IF OBJECT_ID('pt.usp_GetProfileCompleteness', 'P') IS NOT NULL
    DROP PROCEDURE pt.usp_GetProfileCompleteness;
GO

CREATE PROCEDURE pt.usp_GetProfileCompleteness
    @PatientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @HasBasicInfo BIT = 0;
    DECLARE @HasContactInfo BIT = 0;
    DECLARE @HasEmergencyContact BIT = 0;
    DECLARE @HasInsurance BIT = 0;
    DECLARE @HasAllergies BIT = 0;
    DECLARE @IsComplete BIT = 0;
    
    -- Check if patient exists
    IF NOT EXISTS (SELECT 1 FROM pt.Patients WHERE PatientId = @PatientId AND IsDeleted = 0)
    BEGIN
        -- Return error result
        SELECT NULL AS PatientId, 0 AS IsComplete, 'Patient not found' AS ErrorMessage;
        RETURN;
    END
    
    -- Check basic info
    SELECT @HasBasicInfo = CASE 
        WHEN FirstName IS NOT NULL AND FirstName <> ''
         AND LastName IS NOT NULL AND LastName <> ''
         AND Email IS NOT NULL AND Email <> ''
        THEN 1 ELSE 0 END
    FROM pt.Patients
    WHERE PatientId = @PatientId;
    
    -- Check contact info
    SELECT @HasContactInfo = CASE 
        WHEN PhoneNumber IS NOT NULL AND PhoneNumber <> ''
         AND AddressLine1 IS NOT NULL AND AddressLine1 <> ''
        THEN 1 ELSE 0 END
    FROM pt.Patients
    WHERE PatientId = @PatientId;
    
    -- Check emergency contact
    SELECT @HasEmergencyContact = CASE 
        WHEN EmergencyContactName IS NOT NULL AND EmergencyContactName <> ''
         AND EmergencyContactPhone IS NOT NULL AND EmergencyContactPhone <> ''
        THEN 1 ELSE 0 END
    FROM pt.Patients
    WHERE PatientId = @PatientId;
    
    -- Check insurance (optimized with index)
    IF EXISTS (SELECT 1 FROM pt.Insurances WHERE PatientId = @PatientId AND IsActive = 1)
        SET @HasInsurance = 1;
    
    -- Check allergies (optimized with index)
    IF EXISTS (SELECT 1 FROM pt.Allergies WHERE PatientId = @PatientId)
        SET @HasAllergies = 1;
    
    -- Calculate overall completeness
    SET @IsComplete = CASE 
        WHEN @HasBasicInfo = 1 AND @HasContactInfo = 1 
         AND @HasEmergencyContact = 1 AND @HasInsurance = 1 
         AND @HasAllergies = 1
        THEN 1 ELSE 0 END;
    
    -- Return result
    SELECT 
        @PatientId AS PatientId,
        @IsComplete AS IsComplete,
        @HasBasicInfo AS HasBasicInfo,
        @HasContactInfo AS HasContactInfo,
        @HasEmergencyContact AS HasEmergencyContact,
        @HasInsurance AS HasInsurance,
        @HasAllergies AS HasAllergies;
END
GO

PRINT 'Created procedure: pt.usp_GetProfileCompleteness';
GO

--------------------------------------------------------------------
-- 2. Get Patient Details (Optimized Single Query)
--------------------------------------------------------------------
IF OBJECT_ID('pt.usp_GetPatientDetails', 'P') IS NOT NULL
    DROP PROCEDURE pt.usp_GetPatientDetails;
GO

CREATE PROCEDURE pt.usp_GetPatientDetails
    @PatientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.PatientId,
        p.FirstName,
        p.MiddleName,
        p.LastName,
        p.Suffix,
        p.DateOfBirth,
        p.Gender,
        p.Email,
        p.PhoneNumber,
        p.AlternatePhone,
        p.AddressLine1,
        p.AddressLine2,
        p.City,
        p.State,
        p.ZipCode,
        p.Country,
        p.Race,
        p.Ethnicity,
        p.PreferredLanguage,
        p.MaritalStatus,
        p.EmergencyContactName,
        p.EmergencyContactPhone,
        p.EmergencyContactRelationship,
        p.InterpreterRequired,
        p.MobilityAssistance,
        p.ReligiousConsiderations,
        p.PrimaryCarePhysician,
        p.PCPPhoneNumber,
        p.ReferringPhysician,
        p.PreferredPharmacyName,
        p.PreferredPharmacyLocation,
        CASE WHEN p.SsnEncrypted IS NOT NULL AND p.SsnEncrypted <> '' THEN 1 ELSE 0 END AS HasSsn,
        p.CreatedAt,
        p.UpdatedAt
    FROM pt.Patients p
    WHERE p.PatientId = @PatientId 
      AND p.IsDeleted = 0;
END
GO

PRINT 'Created procedure: pt.usp_GetPatientDetails';
GO

--------------------------------------------------------------------
-- 3. Update Personal Information (Optimized Single Statement)
--------------------------------------------------------------------
IF OBJECT_ID('pt.usp_UpdatePersonalInfo', 'P') IS NOT NULL
    DROP PROCEDURE pt.usp_UpdatePersonalInfo;
GO

CREATE PROCEDURE pt.usp_UpdatePersonalInfo
    @PatientId UNIQUEIDENTIFIER,
    @FirstName NVARCHAR(100) = NULL,
    @MiddleName NVARCHAR(100) = NULL,
    @LastName NVARCHAR(100) = NULL,
    @Suffix NVARCHAR(20) = NULL,
    @DateOfBirth DATETIME2 = NULL,
    @Gender NVARCHAR(50) = NULL,
    @MaritalStatus NVARCHAR(50) = NULL,
    @Race NVARCHAR(100) = NULL,
    @Ethnicity NVARCHAR(100) = NULL,
    @PreferredLanguage NVARCHAR(50) = NULL,
    @UpdatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if patient exists
    IF NOT EXISTS (SELECT 1 FROM pt.Patients WHERE PatientId = @PatientId AND IsDeleted = 0)
    BEGIN
        RAISERROR('Patient not found', 16, 1);
        RETURN -1;
    END
    
    -- Update only provided fields
    UPDATE pt.Patients
    SET 
        FirstName = ISNULL(@FirstName, FirstName),
        MiddleName = CASE WHEN @MiddleName IS NOT NULL THEN @MiddleName ELSE MiddleName END,
        LastName = ISNULL(@LastName, LastName),
        Suffix = CASE WHEN @Suffix IS NOT NULL THEN @Suffix ELSE Suffix END,
        DateOfBirth = ISNULL(@DateOfBirth, DateOfBirth),
        Gender = ISNULL(@Gender, Gender),
        MaritalStatus = CASE WHEN @MaritalStatus IS NOT NULL THEN @MaritalStatus ELSE MaritalStatus END,
        Race = CASE WHEN @Race IS NOT NULL THEN @Race ELSE Race END,
        Ethnicity = CASE WHEN @Ethnicity IS NOT NULL THEN @Ethnicity ELSE Ethnicity END,
        PreferredLanguage = CASE WHEN @PreferredLanguage IS NOT NULL THEN @PreferredLanguage ELSE PreferredLanguage END,
        UpdatedAt = GETUTCDATE(),
        UpdatedBy = @UpdatedBy
    WHERE PatientId = @PatientId;
    
    -- Log audit
    INSERT INTO pt.AuditLogs (
        AuditLogId, PatientId, EventType, EntityType, EntityId, 
        EventTimestamp, ActorId, ActorName, Action, Success, PhiAccessed
    )
    VALUES (
        NEWID(), @PatientId, 'Update', 'Patient', @PatientId,
        GETUTCDATE(), @UpdatedBy, @UpdatedBy, 'Personal information updated', 1, 1
    );
    
    RETURN 0;
END
GO

PRINT 'Created procedure: pt.usp_UpdatePersonalInfo';
GO

--------------------------------------------------------------------
-- 4. Get Dashboard Summary (Complex Aggregation)
--------------------------------------------------------------------
IF OBJECT_ID('pt.usp_GetDashboardSummary', 'P') IS NOT NULL
    DROP PROCEDURE pt.usp_GetDashboardSummary;
GO

CREATE PROCEDURE pt.usp_GetDashboardSummary
    @PatientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Patient basic info
    SELECT 
        p.PatientId,
        p.FirstName + ' ' + p.LastName AS PatientName,
        p.Email,
        p.PhoneNumber
    FROM pt.Patients p
    WHERE p.PatientId = @PatientId AND p.IsDeleted = 0;
    
    -- Primary insurance
    SELECT TOP 1
        i.InsuranceId,
        i.PayerName,
        i.PlanName,
        i.MemberId,
        i.EffectiveDate,
        i.ExpirationDate
    FROM pt.Insurances i
    WHERE i.PatientId = @PatientId 
      AND i.IsActive = 1
      AND i.Priority = 1
    ORDER BY i.EffectiveDate DESC;
    
    -- Counts
    SELECT 
        (SELECT COUNT(*) FROM pt.FamilyRelations WHERE PatientId = @PatientId) AS FamilyMemberCount,
        (SELECT COUNT(*) FROM pt.Allergies WHERE PatientId = @PatientId) AS AllergyCount,
        (SELECT COUNT(*) FROM pt.Medications WHERE PatientId = @PatientId AND IsActive = 1) AS MedicationCount,
        (SELECT COUNT(*) FROM pt.Documents WHERE PatientId = @PatientId) AS DocumentCount;
END
GO

PRINT 'Created procedure: pt.usp_GetDashboardSummary';
GO

--------------------------------------------------------------------
-- 5. Get Complete Medical History (Multiple Joins)
--------------------------------------------------------------------
IF OBJECT_ID('pt.usp_GetMedicalHistory', 'P') IS NOT NULL
    DROP PROCEDURE pt.usp_GetMedicalHistory;
GO

CREATE PROCEDURE pt.usp_GetMedicalHistory
    @PatientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Allergies
    SELECT 
        a.AllergyId,
        a.AllergenName,
        a.AllergenType,
        a.Reaction,
        a.Severity,
        a.OnsetDate,
        a.ClinicalStatus
    FROM pt.Allergies a
    WHERE a.PatientId = @PatientId
    ORDER BY a.Severity DESC, a.OnsetDate DESC;
    
    -- Medications
    SELECT 
        m.MedicationId,
        m.MedicationName,
        m.Dosage,
        m.Frequency,
        m.Prescriber,
        m.StartDate,
        m.IsActive
    FROM pt.Medications m
    WHERE m.PatientId = @PatientId
    ORDER BY m.IsActive DESC, m.StartDate DESC;
    
    -- Chronic Conditions
    SELECT 
        c.ConditionId,
        c.ConditionName,
        c.ConditionCode,
        pcc.DiagnosedDate,
        pcc.Notes
    FROM pt.PatientChronicConditions pcc
    INNER JOIN pt.Conditions c ON pcc.ConditionId = c.ConditionId
    WHERE pcc.PatientId = @PatientId
    ORDER BY pcc.DiagnosedDate DESC;
    
    -- Surgeries
    SELECT 
        s.SurgeryId,
        s.SurgeryType,
        s.SurgeryDate,
        s.Notes,
        s.CreatedAt
    FROM pt.PatientSurgeries s
    WHERE s.PatientId = @PatientId
    ORDER BY s.SurgeryDate DESC;
    
    -- Hospitalizations
    SELECT 
        h.HospitalizationId,
        h.HospitalName,
        h.Reason,
        h.AdmissionDate,
        h.DischargeDate,
        h.Notes
    FROM pt.PatientHospitalizations h
    WHERE h.PatientId = @PatientId
    ORDER BY h.AdmissionDate DESC;
    
    -- Family Medical History
    SELECT 
        f.FamilyHistoryId,
        f.Relative,
        f.Condition,
        f.Notes,
        f.CreatedAt
    FROM pt.FamilyMedicalHistory f
    WHERE f.PatientId = @PatientId
    ORDER BY f.CreatedAt DESC;
    
    -- Immunizations
    SELECT 
        i.ImmunizationId,
        i.VaccineName,
        i.VaccineCode,
        i.DateAdministered,
        i.DoseNumber,
        i.AdministeredBy
    FROM pt.Immunizations i
    WHERE i.PatientId = @PatientId
    ORDER BY i.DateAdministered DESC;
END
GO

PRINT 'Created procedure: pt.usp_GetMedicalHistory';
GO

--------------------------------------------------------------------
-- 6. Search Patients (Optimized with Filters)
--------------------------------------------------------------------
IF OBJECT_ID('pt.usp_SearchPatients', 'P') IS NOT NULL
    DROP PROCEDURE pt.usp_SearchPatients;
GO

CREATE PROCEDURE pt.usp_SearchPatients
    @SearchTerm NVARCHAR(100) = NULL,
    @Email NVARCHAR(100) = NULL,
    @PhoneNumber NVARCHAR(25) = NULL,
    @DateOfBirth DATE = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    -- Get total count
    DECLARE @TotalCount INT;
    
    SELECT @TotalCount = COUNT(*)
    FROM pt.Patients p
    WHERE p.IsDeleted = 0
      AND (@SearchTerm IS NULL OR 
           p.FirstName LIKE '%' + @SearchTerm + '%' OR 
           p.LastName LIKE '%' + @SearchTerm + '%')
      AND (@Email IS NULL OR p.Email = @Email)
      AND (@PhoneNumber IS NULL OR p.PhoneNumber = @PhoneNumber)
      AND (@DateOfBirth IS NULL OR CAST(p.DateOfBirth AS DATE) = @DateOfBirth);
    
    -- Get paginated results
    SELECT 
        p.PatientId,
        p.FirstName,
        p.MiddleName,
        p.LastName,
        p.DateOfBirth,
        p.Gender,
        p.Email,
        p.PhoneNumber,
        p.City,
        p.State,
        @TotalCount AS TotalCount,
        @PageNumber AS CurrentPage,
        @PageSize AS PageSize
    FROM pt.Patients p
    WHERE p.IsDeleted = 0
      AND (@SearchTerm IS NULL OR 
           p.FirstName LIKE '%' + @SearchTerm + '%' OR 
           p.LastName LIKE '%' + @SearchTerm + '%')
      AND (@Email IS NULL OR p.Email = @Email)
      AND (@PhoneNumber IS NULL OR p.PhoneNumber = @PhoneNumber)
      AND (@DateOfBirth IS NULL OR CAST(p.DateOfBirth AS DATE) = @DateOfBirth)
    ORDER BY p.LastName, p.FirstName
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO

PRINT 'Created procedure: pt.usp_SearchPatients';
GO

--------------------------------------------------------------------
-- 7. Comprehensive Patient Intake (Complex Transaction)
--------------------------------------------------------------------
IF OBJECT_ID('pt.usp_SavePatientIntake', 'P') IS NOT NULL
    DROP PROCEDURE pt.usp_SavePatientIntake;
GO

CREATE PROCEDURE pt.usp_SavePatientIntake
    @PatientData NVARCHAR(MAX), -- JSON format
    @CreatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Parse JSON
        DECLARE @PatientId UNIQUEIDENTIFIER;
        DECLARE @FirstName NVARCHAR(100);
        DECLARE @LastName NVARCHAR(100);
        DECLARE @Email NVARCHAR(100);
        
        SELECT 
            @PatientId = JSON_VALUE(@PatientData, '$.patient.id'),
            @FirstName = JSON_VALUE(@PatientData, '$.patient.firstName'),
            @LastName = JSON_VALUE(@PatientData, '$.patient.lastName'),
            @Email = JSON_VALUE(@PatientData, '$.patient.email');
        
        -- Check if new or update
        IF @PatientId IS NULL OR @PatientId = '00000000-0000-0000-0000-000000000000'
        BEGIN
            SET @PatientId = NEWID();
            
            -- Insert new patient (simplified - full implementation would parse all JSON fields)
            INSERT INTO pt.Patients (
                PatientId, FirstName, LastName, Email, 
                CreatedAt, CreatedBy, IsActive, IsDeleted
            )
            VALUES (
                @PatientId, @FirstName, @LastName, @Email,
                GETUTCDATE(), @CreatedBy, 1, 0
            );
        END
        ELSE
        BEGIN
            -- Update existing patient
            UPDATE pt.Patients
            SET 
                FirstName = @FirstName,
                LastName = @LastName,
                Email = @Email,
                UpdatedAt = GETUTCDATE(),
                UpdatedBy = @CreatedBy
            WHERE PatientId = @PatientId;
        END
        
        -- Log audit
        INSERT INTO pt.AuditLogs (
            AuditLogId, PatientId, EventType, EntityType, EntityId,
            EventTimestamp, ActorId, ActorName, Action, Success, PhiAccessed
        )
        VALUES (
            NEWID(), @PatientId, 'Update', 'Patient', @PatientId,
            GETUTCDATE(), @CreatedBy, @CreatedBy, 'Comprehensive intake completed', 1, 1
        );
        
        COMMIT TRANSACTION;
        
        -- Return patient ID
        SELECT @PatientId AS PatientId, 'Success' AS Status;
        RETURN 0;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- Return error
        SELECT 
            NULL AS PatientId, 
            'Error' AS Status,
            ERROR_MESSAGE() AS ErrorMessage;
        RETURN -1;
    END CATCH
END
GO

PRINT 'Created procedure: pt.usp_SavePatientIntake';
GO

--------------------------------------------------------------------
-- 8. Bulk Get Allergies for Multiple Patients (Optimization)
--------------------------------------------------------------------
IF OBJECT_ID('pt.usp_GetAllergiesBulk', 'P') IS NOT NULL
    DROP PROCEDURE pt.usp_GetAllergiesBulk;
GO

CREATE PROCEDURE pt.usp_GetAllergiesBulk
    @PatientIds NVARCHAR(MAX) -- Comma-separated GUIDs
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Parse patient IDs into table
    DECLARE @PatientIdTable TABLE (PatientId UNIQUEIDENTIFIER);
    
    INSERT INTO @PatientIdTable (PatientId)
    SELECT CAST(value AS UNIQUEIDENTIFIER)
    FROM STRING_SPLIT(@PatientIds, ',')
    WHERE value <> '';
    
    -- Get all allergies for these patients
    SELECT 
        a.PatientId,
        a.AllergyId,
        a.AllergenName,
        a.AllergenType,
        a.Reaction,
        a.Severity,
        a.OnsetDate
    FROM pt.Allergies a
    INNER JOIN @PatientIdTable p ON a.PatientId = p.PatientId
    ORDER BY a.PatientId, a.Severity DESC;
END
GO

PRINT 'Created procedure: pt.usp_GetAllergiesBulk';
GO

--------------------------------------------------------------------
-- Grant execute permissions
--------------------------------------------------------------------
GRANT EXECUTE ON pt.usp_GetProfileCompleteness TO PUBLIC;
GRANT EXECUTE ON pt.usp_GetPatientDetails TO PUBLIC;
GRANT EXECUTE ON pt.usp_UpdatePersonalInfo TO PUBLIC;
GRANT EXECUTE ON pt.usp_GetDashboardSummary TO PUBLIC;
GRANT EXECUTE ON pt.usp_GetMedicalHistory TO PUBLIC;
GRANT EXECUTE ON pt.usp_SearchPatients TO PUBLIC;
GRANT EXECUTE ON pt.usp_SavePatientIntake TO PUBLIC;
GRANT EXECUTE ON pt.usp_GetAllergiesBulk TO PUBLIC;

PRINT 'Granted execute permissions on all stored procedures';
GO

PRINT '========================================';
PRINT 'All optimized stored procedures created successfully!';
PRINT '========================================';
GO

