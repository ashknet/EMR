-- =============================================
-- Comprehensive Patient Service - Stored Procedures
-- ALL patient operations in one place
-- =============================================

USE [HealthcarePlatform]
GO

-- =============================================
-- Dashboard: Get Complete Patient Overview
-- =============================================
CREATE OR ALTER PROCEDURE patient.usp_Dashboard_GetOverview
    @PatientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Patient Demographics
    SELECT TOP 1
        Id, FirstName, LastName, DateOfBirth, Gender,
        Email, PhoneNumber, PhotoUrl, IsPrimaryAccountHolder
    FROM patient.Patients
    WHERE Id = @PatientId AND IsDeleted = 0;
    
    -- Family Members Count
    SELECT COUNT(*) AS FamilyMemberCount
    FROM patient.FamilyMembers
    WHERE PrimaryPatientId = @PatientId AND IsActive = 1;
    
    -- Total Insurance Coverage
    SELECT 
        SUM(ISNULL(AnnualDeductible, 0) - ISNULL(DeductibleMet, 0)) AS DeductibleRemaining,
        SUM(ISNULL(OutOfPocketMax, 0) - ISNULL(OutOfPocketMet, 0)) AS OutOfPocketRemaining,
        COUNT(*) AS ActivePolicies
    FROM patient.InsurancePolicies
    WHERE PatientId = @PatientId AND Status = 'active' AND IsDeleted = 0;
    
    -- Visits This Year
    SELECT 
        COUNT(*) AS TotalVisitsThisYear,
        MAX(CASE WHEN VisitDate > GETUTCDATE() THEN VisitDate END) AS NextAppointment,
        MAX(CASE WHEN VisitDate <= GETUTCDATE() THEN VisitDate END) AS LastVisit
    FROM patient.Visits
    WHERE PatientId = @PatientId 
      AND YEAR(VisitDate) = YEAR(GETUTCDATE())
      AND IsDeleted = 0;
    
    -- Critical Allergies
    SELECT Id, AllergenName, Category, Criticality, Severity
    FROM patient.Allergies
    WHERE PatientId = @PatientId 
      AND Criticality = 'high'
      AND ClinicalStatus = 'active'
      AND IsDeleted = 0;
    
    -- Active Medications Count
    SELECT COUNT(*) AS ActiveMedicationsCount
    FROM patient.Medications
    WHERE PatientId = @PatientId 
      AND Status = 'active'
      AND IsDeleted = 0;
    
    -- Recent Activity (Last 5)
    SELECT TOP 5
        EventType, EventAction, EventTimestamp, EventDescription
    FROM patient.AuditLog
    WHERE PatientId = @PatientId
    ORDER BY EventTimestamp DESC;
    
    -- Active Consents
    SELECT COUNT(*) AS ActiveConsentsCount
    FROM patient.Consents
    WHERE PatientId = @PatientId 
      AND Status = 'active'
      AND IsRevoked = 0
      AND (ExpirationDate IS NULL OR ExpirationDate > GETUTCDATE())
      AND IsDeleted = 0;
END
GO

-- =============================================
-- Patient: Get Full Profile
-- =============================================
CREATE OR ALTER PROCEDURE patient.usp_Patient_GetFullProfile
    @PatientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Core Demographics
    SELECT * FROM patient.Patients WHERE Id = @PatientId AND IsDeleted = 0;
    
    -- Family Members
    SELECT fm.*, p.FirstName, p.LastName, p.DateOfBirth, p.Gender, p.PhotoUrl
    FROM patient.FamilyMembers fm
    INNER JOIN patient.Patients p ON fm.FamilyMemberPatientId = p.Id
    WHERE fm.PrimaryPatientId = @PatientId AND fm.IsActive = 1;
    
    -- Allergies
    SELECT * FROM patient.Allergies 
    WHERE PatientId = @PatientId AND IsDeleted = 0
    ORDER BY Criticality DESC, AllergenName;
    
    -- Medications
    SELECT * FROM patient.Medications 
    WHERE PatientId = @PatientId AND IsDeleted = 0
    ORDER BY Status, MedicationName;
    
    -- Conditions
    SELECT * FROM patient.Conditions 
    WHERE PatientId = @PatientId AND IsDeleted = 0
    ORDER BY ClinicalStatus, ConditionName;
    
    -- Immunizations
    SELECT * FROM patient.Immunizations 
    WHERE PatientId = @PatientId AND IsDeleted = 0
    ORDER BY AdministrationDate DESC;
    
    -- Insurance Policies
    SELECT * FROM patient.InsurancePolicies 
    WHERE PatientId = @PatientId AND IsDeleted = 0
    ORDER BY IsPrimary DESC, Priority;
    
    -- Documents
    SELECT * FROM patient.Documents 
    WHERE PatientId = @PatientId AND IsDeleted = 0
    ORDER BY UploadedAt DESC;
    
    -- Visits
    SELECT * FROM patient.Visits 
    WHERE PatientId = @PatientId AND IsDeleted = 0
    ORDER BY VisitDate DESC;
    
    -- Consents
    SELECT * FROM patient.Consents 
    WHERE PatientId = @PatientId AND IsDeleted = 0
    ORDER BY CreatedAt DESC;
END
GO

-- =============================================
-- Patient: Create New Patient
-- =============================================
CREATE OR ALTER PROCEDURE patient.usp_Patient_Create
    @FirstName NVARCHAR(500),
    @LastName NVARCHAR(500),
    @DateOfBirth DATE,
    @Gender NVARCHAR(50),
    @Email NVARCHAR(500) = NULL,
    @PhoneNumber NVARCHAR(500) = NULL,
    @ProfileType NVARCHAR(50) = 'Self',
    @IsPrimaryAccountHolder BIT = 0,
    @PrimaryAccountHolderId UNIQUEIDENTIFIER = NULL,
    @CreatedBy NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @NewPatientId UNIQUEIDENTIFIER = NEWID();
    DECLARE @MRN NVARCHAR(50) = 'MRN' + FORMAT(NEWID(), 'N');
    
    INSERT INTO patient.Patients (
        Id, FirstName, LastName, DateOfBirth, Gender,
        Email, PhoneNumber, MRN, ProfileType,
        IsPrimaryAccountHolder, PrimaryAccountHolderId,
        CreatedBy, CreatedAt
    )
    VALUES (
        @NewPatientId, @FirstName, @LastName, @DateOfBirth, @Gender,
        @Email, @PhoneNumber, @MRN, @ProfileType,
        @IsPrimaryAccountHolder, @PrimaryAccountHolderId,
        @CreatedBy, GETUTCDATE()
    );
    
    -- Log audit event
    INSERT INTO patient.AuditLog (
        EventType, EventCategory, EventAction, EventOutcome,
        ActorId, ActorType, TargetType, TargetId, PatientId,
        EventDescription
    )
    VALUES (
        'PatientCreate', 'Patient', 'Create', 'Success',
        @CreatedBy, 'User', 'Patient', @NewPatientId, @NewPatientId,
        'New patient created: ' + @FirstName + ' ' + @LastName
    );
    
    -- Return new patient
    SELECT * FROM patient.Patients WHERE Id = @NewPatientId;
END
GO

-- =============================================
-- Allergy: Add Critical Allergy
-- =============================================
CREATE OR ALTER PROCEDURE patient.usp_Allergy_Create
    @PatientId UNIQUEIDENTIFIER,
    @AllergenName NVARCHAR(255),
    @Category NVARCHAR(100),
    @Criticality NVARCHAR(20),
    @Severity NVARCHAR(50) = NULL,
    @ReactionDescription NVARCHAR(MAX) = NULL,
    @CreatedBy NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @NewAllergyId UNIQUEIDENTIFIER = NEWID();
    
    INSERT INTO patient.Allergies (
        Id, PatientId, AllergenName, Category, Criticality,
        Severity, ReactionDescription, ClinicalStatus,
        RecordedDate, CreatedBy
    )
    VALUES (
        @NewAllergyId, @PatientId, @AllergenName, @Category, @Criticality,
        @Severity, @ReactionDescription, 'active',
        GETUTCDATE(), @CreatedBy
    );
    
    -- Log audit event
    INSERT INTO patient.AuditLog (
        EventType, EventCategory, EventAction, EventOutcome,
        ActorId, ActorType, TargetType, TargetId, PatientId,
        EventDescription
    )
    VALUES (
        'AllergyCreate', 'ClinicalData', 'Create', 'Success',
        @CreatedBy, 'User', 'Allergy', @NewAllergyId, @PatientId,
        'Critical allergy added: ' + @AllergenName + ' (' + @Criticality + ')'
    );
    
    SELECT * FROM patient.Allergies WHERE Id = @NewAllergyId;
END
GO

-- =============================================
-- Insurance: Calculate Total Coverage Summary
-- =============================================
CREATE OR ALTER PROCEDURE patient.usp_Insurance_GetCoverageSummary
    @PatientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get family members for coverage calculation
    DECLARE @FamilyPatientIds TABLE (PatientId UNIQUEIDENTIFIER);
    
    INSERT INTO @FamilyPatientIds
    SELECT @PatientId
    UNION
    SELECT FamilyMemberPatientId 
    FROM patient.FamilyMembers 
    WHERE PrimaryPatientId = @PatientId AND IsActive = 1;
    
    -- Calculate total family coverage
    SELECT 
        COUNT(DISTINCT ip.Id) AS TotalPolicies,
        SUM(ip.AnnualDeductible) AS TotalAnnualDeductible,
        SUM(ip.DeductibleMet) AS TotalDeductibleMet,
        SUM(ip.AnnualDeductible) - SUM(ip.DeductibleMet) AS DeductibleRemaining,
        SUM(ip.OutOfPocketMax) AS TotalOutOfPocketMax,
        SUM(ip.OutOfPocketMet) AS TotalOutOfPocketMet,
        SUM(ip.OutOfPocketMax) - SUM(ip.OutOfPocketMet) AS OutOfPocketRemaining,
        COUNT(DISTINCT c.Id) AS TotalClaims,
        SUM(c.TotalCharges) AS TotalClaimsCharges,
        SUM(c.PaidAmount) AS TotalClaimsPaid,
        SUM(c.PatientResponsibility) AS TotalPatientResponsibility
    FROM patient.InsurancePolicies ip
    LEFT JOIN patient.Claims c ON ip.Id = c.InsurancePolicyId AND c.IsDeleted = 0
    WHERE ip.PatientId IN (SELECT PatientId FROM @FamilyPatientIds)
      AND ip.Status = 'active'
      AND ip.IsDeleted = 0;
    
    -- Get individual policies
    SELECT 
        ip.*,
        p.FirstName + ' ' + p.LastName AS PatientName
    FROM patient.InsurancePolicies ip
    INNER JOIN patient.Patients p ON ip.PatientId = p.Id
    WHERE ip.PatientId IN (SELECT PatientId FROM @FamilyPatientIds)
      AND ip.Status = 'active'
      AND ip.IsDeleted = 0
    ORDER BY ip.IsPrimary DESC, ip.Priority;
END
GO

-- =============================================
-- Consent: Generate QR Code
-- =============================================
CREATE OR ALTER PROCEDURE patient.usp_Consent_GenerateQRCode
    @PatientId UNIQUEIDENTIFIER,
    @ConsentScope NVARCHAR(100) = 'EmergencyIntake',
    @ExpirationHours INT = 24,
    @MaxUsage INT = 5,
    @CreatedBy NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @NewConsentId UNIQUEIDENTIFIER = NEWID();
    DECLARE @QRCode NVARCHAR(MAX) = 'QR-' + CONVERT(NVARCHAR(50), @PatientId) + '-' + CONVERT(NVARCHAR(50), @NewConsentId);
    DECLARE @Expiration DATETIME2 = DATEADD(HOUR, @ExpirationHours, GETUTCDATE());
    
    INSERT INTO patient.Consents (
        Id, PatientId, ConsentType, ConsentScope,
        Status, EffectiveDate, ExpirationDate,
        CanView, CanShare, CanExport,
        QRCodeData, QRCodeExpiration, QRCodeMaxUsage, QRCodeUsageCount,
        IsSignedElectronically, SignedDate,
        CreatedBy
    )
    VALUES (
        @NewConsentId, @PatientId, 'QRCodeSharing', @ConsentScope,
        'active', GETUTCDATE(), @Expiration,
        1, 0, 0,
        @QRCode, @Expiration, @MaxUsage, 0,
        1, GETUTCDATE(),
        @CreatedBy
    );
    
    -- Log audit event
    INSERT INTO patient.AuditLog (
        EventType, EventCategory, EventAction, EventOutcome,
        ActorId, ActorType, TargetType, TargetId, PatientId,
        EventDescription
    )
    VALUES (
        'QRCodeGenerate', 'Consent', 'Create', 'Success',
        @CreatedBy, 'User', 'Consent', @NewConsentId, @PatientId,
        'QR code generated (expires: ' + CONVERT(NVARCHAR, @Expiration, 120) + ')'
    );
    
    SELECT * FROM patient.Consents WHERE Id = @NewConsentId;
END
GO

-- =============================================
-- Audit: Get Patient Access History
-- =============================================
CREATE OR ALTER PROCEDURE patient.usp_Audit_GetPatientAccessHistory
    @PatientId UNIQUEIDENTIFIER,
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL,
    @Limit INT = 100
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @StartDate IS NULL SET @StartDate = DATEADD(MONTH, -3, GETUTCDATE());
    IF @EndDate IS NULL SET @EndDate = GETUTCDATE();
    
    SELECT TOP (@Limit)
        EventTimestamp,
        EventType,
        EventAction,
        EventOutcome,
        ActorName,
        ActorRole,
        TargetType,
        IPAddress,
        EventDescription,
        IsAuthorized,
        IsAnomaly
    FROM patient.AuditLog
    WHERE PatientId = @PatientId
      AND EventTimestamp BETWEEN @StartDate AND @EndDate
    ORDER BY EventTimestamp DESC;
END
GO

-- =============================================
-- Visit: Get Upcoming Appointments
-- =============================================
CREATE OR ALTER PROCEDURE patient.usp_Visit_GetUpcomingAppointments
    @PatientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get family member IDs
    DECLARE @FamilyPatientIds TABLE (PatientId UNIQUEIDENTIFIER);
    
    INSERT INTO @FamilyPatientIds
    SELECT @PatientId
    UNION
    SELECT FamilyMemberPatientId 
    FROM patient.FamilyMembers 
    WHERE PrimaryPatientId = @PatientId AND IsActive = 1;
    
    -- Get upcoming visits for entire family
    SELECT 
        v.*,
        p.FirstName + ' ' + p.LastName AS PatientName,
        p.PhotoUrl
    FROM patient.Visits v
    INNER JOIN patient.Patients p ON v.PatientId = p.Id
    WHERE v.PatientId IN (SELECT PatientId FROM @FamilyPatientIds)
      AND v.VisitDate > GETUTCDATE()
      AND v.Status IN ('scheduled', 'confirmed')
      AND v.IsDeleted = 0
    ORDER BY v.VisitDate;
END
GO

PRINT '✓ Comprehensive stored procedures created successfully'
PRINT '  - Dashboard overview'
PRINT '  - Full patient profile'
PRINT '  - Patient create'
PRINT '  - Critical allergy tracking'
PRINT '  - Insurance coverage summary'
PRINT '  - QR code generation'
PRINT '  - Audit access history'
PRINT '  - Upcoming appointments'
GO
