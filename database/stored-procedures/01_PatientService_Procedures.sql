-- =============================================
-- Healthcare Platform - Patient Service Stored Procedures
-- HIPAA-compliant data access procedures with audit logging
-- =============================================

USE [HealthcarePlatform_Patient]
GO

-- =============================================
-- Stored Procedure: usp_Patient_GetById
-- Purpose: Retrieve patient by ID with audit logging
-- =============================================
CREATE OR ALTER PROCEDURE patient.usp_Patient_GetById
    @PatientId UNIQUEIDENTIFIER,
    @AccessedBy NVARCHAR(255),
    @IPAddress NVARCHAR(50) = NULL,
    @Reason NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if patient exists
    IF NOT EXISTS (SELECT 1 FROM patient.Patients WHERE Id = @PatientId AND IsDeleted = 0)
    BEGIN
        RAISERROR('Patient not found', 16, 1)
        RETURN
    END
    
    -- Log access
    INSERT INTO patient.PatientAccessLog (PatientId, AccessType, AccessedBy, IPAddress, Reason, AccessedAt)
    VALUES (@PatientId, 'View', @AccessedBy, @IPAddress, @Reason, GETUTCDATE())
    
    -- Return patient data
    SELECT 
        Id, FirstName, MiddleName, LastName, PreferredName, DateOfBirth, Gender, BiologicalSex,
        Email, PhoneNumber, AlternatePhoneNumber,
        AddressLine1, AddressLine2, City, State, ZipCode, Country,
        SSN, SSNHash, MedicalRecordNumber, DriverLicenseNumber, PassportNumber,
        Race, Ethnicity, PreferredLanguage, MaritalStatus,
        EmergencyContactName, EmergencyContactRelationship, EmergencyContactPhone,
        PrimaryAccountHolderId, IsPrimaryAccountHolder, ProfileType, RelationshipToPrimary,
        FHIRPatientId, FHIRResourceJson,
        IsActive, IsVerified, VerifiedAt, IsMinor, DeceasedDate,
        HasConsentedToDataSharing, ConsentDate, ConsentVersion,
        LastAccessedBy, LastAccessedAt, ProfilePhotoUrl,
        CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, RowVersion
    FROM patient.Patients
    WHERE Id = @PatientId AND IsDeleted = 0
    
    -- Update last accessed
    UPDATE patient.Patients
    SET LastAccessedBy = @AccessedBy, LastAccessedAt = GETUTCDATE()
    WHERE Id = @PatientId
END
GO

-- =============================================
-- Stored Procedure: usp_Patient_Search
-- Purpose: Search patients by various criteria
-- =============================================
CREATE OR ALTER PROCEDURE patient.usp_Patient_Search
    @SearchTerm NVARCHAR(255),
    @AccessedBy NVARCHAR(255),
    @MaxResults INT = 50
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@MaxResults)
        Id, FirstName, MiddleName, LastName, PreferredName, DateOfBirth, Gender,
        Email, PhoneNumber, City, State, MedicalRecordNumber,
        ProfileType, IsActive, IsVerified, CreatedAt
    FROM patient.Patients
    WHERE IsDeleted = 0
    AND (
        FirstName LIKE '%' + @SearchTerm + '%' OR
        LastName LIKE '%' + @SearchTerm + '%' OR
        MedicalRecordNumber LIKE '%' + @SearchTerm + '%' OR
        Email LIKE '%' + @SearchTerm + '%'
    )
    ORDER BY LastName, FirstName
END
GO

-- =============================================
-- Stored Procedure: usp_Patient_GetFamilyMembers
-- Purpose: Get all family members for a primary account holder
-- =============================================
CREATE OR ALTER PROCEDURE patient.usp_Patient_GetFamilyMembers
    @PrimaryAccountHolderId UNIQUEIDENTIFIER,
    @AccessedBy NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get primary account holder
    SELECT 
        Id, FirstName, MiddleName, LastName, DateOfBirth, Gender,
        Email, PhoneNumber, ProfileType, RelationshipToPrimary,
        IsActive, IsVerified, IsMinor, CreatedAt
    FROM patient.Patients
    WHERE Id = @PrimaryAccountHolderId AND IsDeleted = 0
    
    UNION ALL
    
    -- Get all family members
    SELECT 
        Id, FirstName, MiddleName, LastName, DateOfBirth, Gender,
        Email, PhoneNumber, ProfileType, RelationshipToPrimary,
        IsActive, IsVerified, IsMinor, CreatedAt
    FROM patient.Patients
    WHERE PrimaryAccountHolderId = @PrimaryAccountHolderId AND IsDeleted = 0
    ORDER BY ProfileType, DateOfBirth
END
GO

-- =============================================
-- Stored Procedure: usp_Patient_Create
-- Purpose: Create new patient with audit logging
-- =============================================
CREATE OR ALTER PROCEDURE patient.usp_Patient_Create
    @FirstName NVARCHAR(100),
    @MiddleName NVARCHAR(100) = NULL,
    @LastName NVARCHAR(100),
    @DateOfBirth DATE,
    @Gender NVARCHAR(20),
    @Email NVARCHAR(500) = NULL,
    @PhoneNumber NVARCHAR(500) = NULL,
    @ProfileType NVARCHAR(50) = 'Self',
    @PrimaryAccountHolderId UNIQUEIDENTIFIER = NULL,
    @CreatedBy NVARCHAR(255),
    @NewPatientId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    
    BEGIN TRY
        SET @NewPatientId = NEWID()
        
        -- Determine if primary account holder
        DECLARE @IsPrimary BIT = CASE WHEN @ProfileType = 'Self' AND @PrimaryAccountHolderId IS NULL THEN 1 ELSE 0 END
        
        -- Determine if minor
        DECLARE @IsMinor BIT = CASE WHEN DATEDIFF(YEAR, @DateOfBirth, GETDATE()) < 18 THEN 1 ELSE 0 END
        
        -- Insert patient
        INSERT INTO patient.Patients (
            Id, FirstName, MiddleName, LastName, DateOfBirth, Gender,
            Email, PhoneNumber, ProfileType, PrimaryAccountHolderId,
            IsPrimaryAccountHolder, IsMinor, IsActive, CreatedBy, CreatedAt
        )
        VALUES (
            @NewPatientId, @FirstName, @MiddleName, @LastName, @DateOfBirth, @Gender,
            @Email, @PhoneNumber, @ProfileType, @PrimaryAccountHolderId,
            @IsPrimary, @IsMinor, 1, @CreatedBy, GETUTCDATE()
        )
        
        -- Log creation
        INSERT INTO patient.PatientAccessLog (PatientId, AccessType, AccessedBy, AccessedAt)
        VALUES (@NewPatientId, 'Create', @CreatedBy, GETUTCDATE())
        
        COMMIT TRANSACTION
        
        -- Return created patient
        EXEC patient.usp_Patient_GetById @NewPatientId, @CreatedBy, NULL, 'Patient created'
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        THROW
    END CATCH
END
GO

-- =============================================
-- Stored Procedure: usp_Patient_Update
-- Purpose: Update patient with audit logging
-- =============================================
CREATE OR ALTER PROCEDURE patient.usp_Patient_Update
    @PatientId UNIQUEIDENTIFIER,
    @FirstName NVARCHAR(100) = NULL,
    @MiddleName NVARCHAR(100) = NULL,
    @LastName NVARCHAR(100) = NULL,
    @Email NVARCHAR(500) = NULL,
    @PhoneNumber NVARCHAR(500) = NULL,
    @UpdatedBy NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    
    BEGIN TRY
        -- Check if patient exists
        IF NOT EXISTS (SELECT 1 FROM patient.Patients WHERE Id = @PatientId AND IsDeleted = 0)
        BEGIN
            RAISERROR('Patient not found', 16, 1)
            RETURN
        END
        
        -- Update patient (only non-null parameters)
        UPDATE patient.Patients
        SET 
            FirstName = ISNULL(@FirstName, FirstName),
            MiddleName = ISNULL(@MiddleName, MiddleName),
            LastName = ISNULL(@LastName, LastName),
            Email = ISNULL(@Email, Email),
            PhoneNumber = ISNULL(@PhoneNumber, PhoneNumber),
            UpdatedBy = @UpdatedBy,
            UpdatedAt = GETUTCDATE()
        WHERE Id = @PatientId
        
        -- Log update
        INSERT INTO patient.PatientAccessLog (PatientId, AccessType, AccessedBy, AccessedAt)
        VALUES (@PatientId, 'Update', @UpdatedBy, GETUTCDATE())
        
        COMMIT TRANSACTION
        
        -- Return updated patient
        EXEC patient.usp_Patient_GetById @PatientId, @UpdatedBy, NULL, 'Patient updated'
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        THROW
    END CATCH
END
GO

-- =============================================
-- Stored Procedure: usp_Patient_SoftDelete
-- Purpose: Soft delete patient with audit logging
-- =============================================
CREATE OR ALTER PROCEDURE patient.usp_Patient_SoftDelete
    @PatientId UNIQUEIDENTIFIER,
    @DeletedBy NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    
    BEGIN TRY
        -- Check if patient exists
        IF NOT EXISTS (SELECT 1 FROM patient.Patients WHERE Id = @PatientId AND IsDeleted = 0)
        BEGIN
            RAISERROR('Patient not found', 16, 1)
            RETURN
        END
        
        -- Soft delete patient
        UPDATE patient.Patients
        SET 
            IsDeleted = 1,
            DeletedAt = GETUTCDATE(),
            DeletedBy = @DeletedBy
        WHERE Id = @PatientId
        
        -- Log deletion
        INSERT INTO patient.PatientAccessLog (PatientId, AccessType, AccessedBy, AccessedAt)
        VALUES (@PatientId, 'Delete', @DeletedBy, GETUTCDATE())
        
        COMMIT TRANSACTION
        
        SELECT 1 AS Success
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        THROW
    END CATCH
END
GO

-- =============================================
-- Stored Procedure: usp_Patient_GetAccessLog
-- Purpose: Get audit log for a patient
-- =============================================
CREATE OR ALTER PROCEDURE patient.usp_Patient_GetAccessLog
    @PatientId UNIQUEIDENTIFIER,
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL,
    @MaxResults INT = 100
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@MaxResults)
        Id, PatientId, AccessType, AccessedBy, AccessedByRole, AccessedAt,
        IPAddress, UserAgent, Location, DataFields, Reason,
        ConsentId, HasValidConsent, IsAuthorized, AuthorizationMethod
    FROM patient.PatientAccessLog
    WHERE PatientId = @PatientId
    AND (@StartDate IS NULL OR AccessedAt >= @StartDate)
    AND (@EndDate IS NULL OR AccessedAt <= @EndDate)
    ORDER BY AccessedAt DESC
END
GO

-- =============================================
-- Stored Procedure: usp_PatientDocument_Create
-- Purpose: Create new patient document record
-- =============================================
CREATE OR ALTER PROCEDURE patient.usp_PatientDocument_Create
    @PatientId UNIQUEIDENTIFIER,
    @DocumentType NVARCHAR(100),
    @DocumentName NVARCHAR(255),
    @BlobStorageUrl NVARCHAR(1000),
    @FileType NVARCHAR(50),
    @FileSizeBytes BIGINT,
    @UploadedBy NVARCHAR(255),
    @NewDocumentId UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    SET @NewDocumentId = NEWID()
    
    INSERT INTO patient.PatientDocuments (
        Id, PatientId, DocumentType, DocumentName, BlobStorageUrl,
        FileType, FileSizeBytes, UploadedBy, UploadedAt, CreatedBy, CreatedAt
    )
    VALUES (
        @NewDocumentId, @PatientId, @DocumentType, @DocumentName, @BlobStorageUrl,
        @FileType, @FileSizeBytes, @UploadedBy, GETUTCDATE(), @UploadedBy, GETUTCDATE()
    )
    
    -- Log document upload
    INSERT INTO patient.PatientAccessLog (PatientId, AccessType, AccessedBy, Reason, AccessedAt)
    VALUES (@PatientId, 'Create', @UploadedBy, 'Document uploaded: ' + @DocumentName, GETUTCDATE())
    
    SELECT @NewDocumentId AS DocumentId
END
GO

-- =============================================
-- Stored Procedure: usp_Dashboard_GetStats
-- Purpose: Get dashboard statistics for a patient/family
-- =============================================
CREATE OR ALTER PROCEDURE patient.usp_Dashboard_GetStats
    @PrimaryAccountHolderId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Family member count
    SELECT 
        COUNT(*) AS TotalFamilyMembers,
        SUM(CASE WHEN IsMinor = 1 THEN 1 ELSE 0 END) AS MinorCount,
        SUM(CASE WHEN IsVerified = 1 THEN 1 ELSE 0 END) AS VerifiedCount
    FROM patient.Patients
    WHERE (Id = @PrimaryAccountHolderId OR PrimaryAccountHolderId = @PrimaryAccountHolderId)
    AND IsDeleted = 0
    
    -- Document count
    SELECT 
        COUNT(*) AS TotalDocuments,
        SUM(FileSizeBytes) AS TotalSizeBytes,
        MAX(UploadedAt) AS LastUploadDate
    FROM patient.PatientDocuments d
    INNER JOIN patient.Patients p ON d.PatientId = p.Id
    WHERE (p.Id = @PrimaryAccountHolderId OR p.PrimaryAccountHolderId = @PrimaryAccountHolderId)
    AND d.IsDeleted = 0
    
    -- Recent activity count
    SELECT 
        COUNT(*) AS RecentActivityCount
    FROM patient.PatientAccessLog
    WHERE PatientId IN (
        SELECT Id FROM patient.Patients 
        WHERE (Id = @PrimaryAccountHolderId OR PrimaryAccountHolderId = @PrimaryAccountHolderId)
        AND IsDeleted = 0
    )
    AND AccessedAt >= DATEADD(DAY, -30, GETUTCDATE())
END
GO

PRINT 'Patient Service stored procedures created successfully'
GO
