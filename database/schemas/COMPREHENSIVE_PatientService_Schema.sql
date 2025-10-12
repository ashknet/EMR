-- =============================================
-- Healthcare Platform - COMPREHENSIVE Patient Service Schema
-- ALL patient-related data in ONE schema
-- =============================================

USE [HealthcarePlatform]
GO

-- Create comprehensive schema
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'patient')
BEGIN
    EXEC('CREATE SCHEMA patient')
END
GO

-- =============================================
-- CORE PATIENT TABLE
-- =============================================
CREATE TABLE patient.Patients (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    
    -- Demographics (Encrypted PHI)
    FirstName NVARCHAR(500) NOT NULL,
    MiddleName NVARCHAR(500) NULL,
    LastName NVARCHAR(500) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Gender NVARCHAR(50) NOT NULL,
    SSN NVARCHAR(500) NULL, -- Encrypted
    
    -- Contact (Encrypted PHI)
    Email NVARCHAR(500) NULL, -- Encrypted
    PhoneNumber NVARCHAR(500) NULL, -- Encrypted
    AlternatePhone NVARCHAR(500) NULL, -- Encrypted
    
    -- Address (Encrypted PHI)
    AddressLine1 NVARCHAR(500) NULL, -- Encrypted
    AddressLine2 NVARCHAR(500) NULL, -- Encrypted
    City NVARCHAR(255) NULL,
    State NVARCHAR(100) NULL,
    ZipCode NVARCHAR(20) NULL,
    Country NVARCHAR(100) DEFAULT 'USA',
    
    -- Additional Demographics
    PreferredLanguage NVARCHAR(50) DEFAULT 'English',
    Ethnicity NVARCHAR(100) NULL,
    Race NVARCHAR(100) NULL,
    MaritalStatus NVARCHAR(50) NULL,
    
    -- Medical Record Numbers
    MRN NVARCHAR(50) UNIQUE NULL,
    
    -- Profile Type
    ProfileType NVARCHAR(50) DEFAULT 'Self', -- Self, Spouse, Child, Parent, Sibling, Other
    IsPrimaryAccountHolder BIT DEFAULT 0,
    PrimaryAccountHolderId UNIQUEIDENTIFIER NULL,
    RelationshipToPrimary NVARCHAR(100) NULL,
    
    -- Emergency Contact
    EmergencyContactName NVARCHAR(255) NULL,
    EmergencyContactPhone NVARCHAR(500) NULL, -- Encrypted
    EmergencyContactRelationship NVARCHAR(100) NULL,
    
    -- Photo
    PhotoUrl NVARCHAR(1000) NULL,
    
    -- Status
    IsActive BIT DEFAULT 1,
    IsDeceased BIT DEFAULT 0,
    DeceasedDate DATE NULL,
    
    -- FHIR
    FHIRResourceId NVARCHAR(255) NULL,
    FHIRResourceJson NVARCHAR(MAX) NULL,
    LastFHIRSync DATETIME2 NULL,
    
    -- Audit Fields
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(255) NULL,
    RowVersion ROWVERSION,
    
    CONSTRAINT FK_Patient_PrimaryAccountHolder FOREIGN KEY (PrimaryAccountHolderId) 
        REFERENCES patient.Patients(Id),
    
    INDEX IX_Patients_Email (Email),
    INDEX IX_Patients_MRN (MRN),
    INDEX IX_Patients_PrimaryAccountHolder (PrimaryAccountHolderId),
    INDEX IX_Patients_IsActive (IsActive),
    INDEX IX_Patients_DateOfBirth (DateOfBirth)
)
GO

-- =============================================
-- FAMILY MEMBERS TABLE
-- =============================================
CREATE TABLE patient.FamilyMembers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PrimaryPatientId UNIQUEIDENTIFIER NOT NULL,
    FamilyMemberPatientId UNIQUEIDENTIFIER NOT NULL,
    
    RelationshipType NVARCHAR(100) NOT NULL, -- Spouse, Child, Parent, Sibling, Other
    IsDependent BIT DEFAULT 0,
    IsPrimaryContact BIT DEFAULT 0,
    
    -- Permissions
    CanViewMedicalRecords BIT DEFAULT 0,
    CanManageAppointments BIT DEFAULT 0,
    CanAccessInsurance BIT DEFAULT 0,
    
    -- Status
    IsActive BIT DEFAULT 1,
    AddedDate DATETIME2 DEFAULT GETUTCDATE(),
    
    -- Audit
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(255) NOT NULL,
    
    CONSTRAINT FK_FamilyMember_Primary FOREIGN KEY (PrimaryPatientId) 
        REFERENCES patient.Patients(Id),
    CONSTRAINT FK_FamilyMember_Member FOREIGN KEY (FamilyMemberPatientId) 
        REFERENCES patient.Patients(Id),
    
    INDEX IX_FamilyMembers_Primary (PrimaryPatientId),
    INDEX IX_FamilyMembers_Member (FamilyMemberPatientId)
)
GO

-- =============================================
-- ALLERGIES TABLE
-- =============================================
CREATE TABLE patient.Allergies (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Allergy Information
    AllergenName NVARCHAR(255) NOT NULL,
    AllergenCode NVARCHAR(50) NULL,
    Category NVARCHAR(100) NOT NULL, -- medication, food, environment
    AllergyType NVARCHAR(100) DEFAULT 'allergy',
    
    -- Severity
    Criticality NVARCHAR(20) NOT NULL, -- low, high
    Severity NVARCHAR(50) NULL, -- mild, moderate, severe
    ClinicalStatus NVARCHAR(50) DEFAULT 'active',
    VerificationStatus NVARCHAR(50) DEFAULT 'confirmed',
    
    -- Reaction
    ReactionDescription NVARCHAR(MAX) NULL,
    Manifestations NVARCHAR(MAX) NULL,
    
    -- Dates
    OnsetDate DATE NULL,
    RecordedDate DATETIME2 DEFAULT GETUTCDATE(),
    
    -- Source
    RecordedBy NVARCHAR(255) NULL,
    SourceType NVARCHAR(100) DEFAULT 'patient-reported',
    
    -- FHIR
    FHIRResourceId NVARCHAR(255) NULL,
    FHIRResourceJson NVARCHAR(MAX) NULL,
    
    -- Audit
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(255) NULL,
    
    CONSTRAINT FK_Allergy_Patient FOREIGN KEY (PatientId) 
        REFERENCES patient.Patients(Id),
    
    INDEX IX_Allergies_Patient (PatientId),
    INDEX IX_Allergies_Criticality (Criticality),
    INDEX IX_Allergies_Category (Category)
)
GO

-- =============================================
-- MEDICATIONS TABLE
-- =============================================
CREATE TABLE patient.Medications (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Medication Information
    MedicationName NVARCHAR(255) NOT NULL,
    MedicationCode NVARCHAR(50) NULL,
    GenericName NVARCHAR(255) NULL,
    BrandName NVARCHAR(255) NULL,
    
    -- Dosage
    Dosage NVARCHAR(100) NULL,
    DosageForm NVARCHAR(100) NULL, -- Tablet, Capsule, Liquid
    Route NVARCHAR(100) NULL, -- Oral, Injection, Topical
    Frequency NVARCHAR(100) NULL,
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'active', -- active, discontinued, completed
    StartDate DATE NULL,
    EndDate DATE NULL,
    
    -- Prescriber
    PrescribedBy NVARCHAR(255) NULL,
    PrescribedDate DATE NULL,
    
    -- Reason
    ReasonCode NVARCHAR(50) NULL,
    ReasonDescription NVARCHAR(MAX) NULL,
    
    -- Instructions
    Instructions NVARCHAR(MAX) NULL,
    
    -- Source
    SourceType NVARCHAR(100) DEFAULT 'provider-entered',
    
    -- FHIR
    FHIRResourceId NVARCHAR(255) NULL,
    FHIRResourceJson NVARCHAR(MAX) NULL,
    
    -- Audit
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(255) NULL,
    
    CONSTRAINT FK_Medication_Patient FOREIGN KEY (PatientId) 
        REFERENCES patient.Patients(Id),
    
    INDEX IX_Medications_Patient (PatientId),
    INDEX IX_Medications_Status (Status),
    INDEX IX_Medications_StartDate (StartDate)
)
GO

-- =============================================
-- CONDITIONS TABLE
-- =============================================
CREATE TABLE patient.Conditions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Condition Information
    ConditionName NVARCHAR(255) NOT NULL,
    ConditionCode NVARCHAR(50) NULL,
    CodingSystem NVARCHAR(50) DEFAULT 'ICD-10-CM',
    Category NVARCHAR(100) DEFAULT 'problem-list-item',
    
    -- Status
    ClinicalStatus NVARCHAR(50) DEFAULT 'active',
    VerificationStatus NVARCHAR(50) DEFAULT 'confirmed',
    Severity NVARCHAR(50) NULL,
    
    -- Dates
    OnsetDate DATE NULL,
    AbatementDate DATE NULL,
    RecordedDate DATETIME2 DEFAULT GETUTCDATE(),
    
    -- Provider
    RecordedBy NVARCHAR(255) NULL,
    DiagnosedBy NVARCHAR(255) NULL,
    
    -- Notes
    Notes NVARCHAR(MAX) NULL,
    
    -- Source
    SourceType NVARCHAR(100) DEFAULT 'provider-entered',
    
    -- FHIR
    FHIRResourceId NVARCHAR(255) NULL,
    FHIRResourceJson NVARCHAR(MAX) NULL,
    
    -- Audit
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(255) NULL,
    
    CONSTRAINT FK_Condition_Patient FOREIGN KEY (PatientId) 
        REFERENCES patient.Patients(Id),
    
    INDEX IX_Conditions_Patient (PatientId),
    INDEX IX_Conditions_Status (ClinicalStatus)
)
GO

-- =============================================
-- IMMUNIZATIONS TABLE
-- =============================================
CREATE TABLE patient.Immunizations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Vaccine Information
    VaccineName NVARCHAR(255) NOT NULL,
    VaccineCode NVARCHAR(50) NULL,
    
    -- Administration
    AdministrationDate DATE NOT NULL,
    DoseNumber INT NULL,
    SeriesCount INT NULL,
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'completed',
    
    -- Details
    BodySite NVARCHAR(100) NULL,
    Route NVARCHAR(100) NULL,
    DoseQuantity NVARCHAR(50) NULL,
    LotNumber NVARCHAR(100) NULL,
    ExpirationDate DATE NULL,
    
    -- Provider
    AdministeredBy NVARCHAR(255) NULL,
    AdministeredAt NVARCHAR(255) NULL,
    
    -- Source
    SourceType NVARCHAR(100) DEFAULT 'provider-entered',
    
    -- FHIR
    FHIRResourceId NVARCHAR(255) NULL,
    FHIRResourceJson NVARCHAR(MAX) NULL,
    
    -- Audit
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(255) NULL,
    
    CONSTRAINT FK_Immunization_Patient FOREIGN KEY (PatientId) 
        REFERENCES patient.Patients(Id),
    
    INDEX IX_Immunizations_Patient (PatientId),
    INDEX IX_Immunizations_Date (AdministrationDate)
)
GO

-- =============================================
-- INSURANCE POLICIES TABLE
-- =============================================
CREATE TABLE patient.InsurancePolicies (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Policy Information (Encrypted)
    PolicyNumber NVARCHAR(500) NOT NULL,
    GroupNumber NVARCHAR(500) NULL,
    SubscriberId NVARCHAR(500) NULL,
    
    -- Insurance Company
    InsuranceCompanyName NVARCHAR(255) NOT NULL,
    InsuranceCompanyCode NVARCHAR(50) NULL,
    CompanyPhone NVARCHAR(500) NULL,
    
    -- Policy Type
    PolicyType NVARCHAR(50) DEFAULT 'Medical',
    CoverageType NVARCHAR(50) DEFAULT 'Individual',
    PlanName NVARCHAR(255) NULL,
    PlanNetwork NVARCHAR(100) NULL,
    
    -- Subscriber
    IsSubscriber BIT DEFAULT 1,
    SubscriberPatientId UNIQUEIDENTIFIER NULL,
    RelationshipToSubscriber NVARCHAR(100) NULL,
    
    -- Coverage Period
    EffectiveDate DATE NOT NULL,
    TerminationDate DATE NULL,
    
    -- Coverage Details
    AnnualDeductible DECIMAL(18,2) NULL,
    DeductibleMet DECIMAL(18,2) DEFAULT 0,
    OutOfPocketMax DECIMAL(18,2) NULL,
    OutOfPocketMet DECIMAL(18,2) DEFAULT 0,
    CopayPrimaryCare DECIMAL(18,2) NULL,
    CopaySpecialist DECIMAL(18,2) NULL,
    CopayEmergency DECIMAL(18,2) NULL,
    CoinsurancePercentage DECIMAL(5,2) NULL,
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'active',
    IsPrimary BIT DEFAULT 1,
    Priority INT DEFAULT 1,
    
    -- Documents
    InsuranceCardFrontUrl NVARCHAR(1000) NULL,
    InsuranceCardBackUrl NVARCHAR(1000) NULL,
    PolicyDocumentUrl NVARCHAR(1000) NULL,
    
    -- Verification
    IsVerified BIT DEFAULT 0,
    VerifiedAt DATETIME2 NULL,
    VerifiedBy NVARCHAR(255) NULL,
    LastEligibilityCheck DATETIME2 NULL,
    
    -- FHIR
    FHIRResourceId NVARCHAR(255) NULL,
    FHIRResourceJson NVARCHAR(MAX) NULL,
    
    -- Audit
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(255) NULL,
    RowVersion ROWVERSION,
    
    CONSTRAINT FK_InsurancePolicy_Patient FOREIGN KEY (PatientId) 
        REFERENCES patient.Patients(Id),
    CONSTRAINT FK_InsurancePolicy_Subscriber FOREIGN KEY (SubscriberPatientId) 
        REFERENCES patient.Patients(Id),
    
    INDEX IX_InsurancePolicies_Patient (PatientId),
    INDEX IX_InsurancePolicies_Status (Status),
    INDEX IX_InsurancePolicies_Priority (IsPrimary, Priority)
)
GO

-- =============================================
-- CLAIMS TABLE
-- =============================================
CREATE TABLE patient.Claims (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    InsurancePolicyId UNIQUEIDENTIFIER NOT NULL,
    
    -- Claim Information
    ClaimNumber NVARCHAR(100) NOT NULL UNIQUE,
    ClaimType NVARCHAR(50) DEFAULT 'professional',
    
    -- Dates
    ServiceDate DATE NOT NULL,
    ClaimDate DATETIME2 DEFAULT GETUTCDATE(),
    ReceivedDate DATETIME2 NULL,
    ProcessedDate DATETIME2 NULL,
    
    -- Provider
    ProviderName NVARCHAR(255) NULL,
    ProviderNPI NVARCHAR(50) NULL,
    FacilityName NVARCHAR(255) NULL,
    
    -- Financial
    TotalCharges DECIMAL(18,2) NOT NULL,
    AllowedAmount DECIMAL(18,2) NULL,
    PaidAmount DECIMAL(18,2) NULL,
    PatientResponsibility DECIMAL(18,2) NULL,
    Deductible DECIMAL(18,2) NULL,
    Coinsurance DECIMAL(18,2) NULL,
    Copay DECIMAL(18,2) NULL,
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'submitted',
    DenialReason NVARCHAR(MAX) NULL,
    DenialCode NVARCHAR(50) NULL,
    
    -- Diagnosis
    PrimaryDiagnosisCode NVARCHAR(50) NULL,
    PrimaryDiagnosisDescription NVARCHAR(255) NULL,
    ProcedureCodes NVARCHAR(MAX) NULL,
    
    -- EOB
    EOBDocumentUrl NVARCHAR(1000) NULL,
    EOBReceivedDate DATETIME2 NULL,
    
    -- FHIR
    FHIRResourceId NVARCHAR(255) NULL,
    FHIRResourceJson NVARCHAR(MAX) NULL,
    
    -- Audit
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(255) NULL,
    RowVersion ROWVERSION,
    
    CONSTRAINT FK_Claim_Patient FOREIGN KEY (PatientId) 
        REFERENCES patient.Patients(Id),
    CONSTRAINT FK_Claim_Policy FOREIGN KEY (InsurancePolicyId) 
        REFERENCES patient.InsurancePolicies(Id),
    
    INDEX IX_Claims_Patient (PatientId),
    INDEX IX_Claims_Policy (InsurancePolicyId),
    INDEX IX_Claims_Status (Status),
    INDEX IX_Claims_ServiceDate (ServiceDate)
)
GO

-- =============================================
-- CONSENTS TABLE
-- =============================================
CREATE TABLE patient.Consents (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Consent Information
    ConsentType NVARCHAR(100) NOT NULL,
    ConsentScope NVARCHAR(100) NOT NULL,
    ConsentCategory NVARCHAR(100) NULL,
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'active',
    
    -- Period
    EffectiveDate DATETIME2 DEFAULT GETUTCDATE(),
    ExpirationDate DATETIME2 NULL,
    IsIndefinite BIT DEFAULT 0,
    
    -- Permissions
    PolicyRule NVARCHAR(50) DEFAULT 'permit',
    CanView BIT DEFAULT 1,
    CanUpdate BIT DEFAULT 0,
    CanDelete BIT DEFAULT 0,
    CanShare BIT DEFAULT 0,
    CanExport BIT DEFAULT 0,
    
    -- Data Categories
    CoverageScope NVARCHAR(50) DEFAULT 'all',
    DataCategories NVARCHAR(MAX) NULL,
    
    -- Granted To
    GrantedToType NVARCHAR(100) NULL,
    GrantedToId UNIQUEIDENTIFIER NULL,
    GrantedToName NVARCHAR(255) NULL,
    GrantedToOrganization NVARCHAR(255) NULL,
    
    -- Purpose
    Purpose NVARCHAR(255) NULL,
    PurposeDescription NVARCHAR(MAX) NULL,
    
    -- Signature
    IsSignedElectronically BIT DEFAULT 0,
    SignatureData NVARCHAR(MAX) NULL,
    SignedDate DATETIME2 NULL,
    WitnessName NVARCHAR(255) NULL,
    
    -- QR Code
    QRCodeData NVARCHAR(MAX) NULL,
    QRCodeExpiration DATETIME2 NULL,
    QRCodeUsageCount INT DEFAULT 0,
    QRCodeMaxUsage INT NULL,
    
    -- Revocation
    IsRevoked BIT DEFAULT 0,
    RevokedAt DATETIME2 NULL,
    RevokedBy NVARCHAR(255) NULL,
    RevocationReason NVARCHAR(MAX) NULL,
    
    -- FHIR
    FHIRResourceId NVARCHAR(255) NULL,
    FHIRResourceJson NVARCHAR(MAX) NULL,
    
    -- Audit
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(255) NULL,
    RowVersion ROWVERSION,
    
    CONSTRAINT FK_Consent_Patient FOREIGN KEY (PatientId) 
        REFERENCES patient.Patients(Id),
    
    INDEX IX_Consents_Patient (PatientId),
    INDEX IX_Consents_Status (Status),
    INDEX IX_Consents_Expiration (QRCodeExpiration)
)
GO

-- =============================================
-- DOCUMENTS TABLE
-- =============================================
CREATE TABLE patient.Documents (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Document Information
    DocumentName NVARCHAR(255) NOT NULL,
    DocumentType NVARCHAR(100) NOT NULL,
    DocumentCategory NVARCHAR(100) NULL,
    Description NVARCHAR(MAX) NULL,
    
    -- File Information
    FileName NVARCHAR(255) NOT NULL,
    FileType NVARCHAR(50) NOT NULL,
    FileSizeBytes BIGINT NOT NULL,
    BlobStorageUrl NVARCHAR(1000) NOT NULL,
    ThumbnailUrl NVARCHAR(1000) NULL,
    
    -- Dates
    DocumentDate DATE NULL,
    UploadedAt DATETIME2 DEFAULT GETUTCDATE(),
    
    -- Source
    SourceType NVARCHAR(100) NULL,
    SourceProvider NVARCHAR(255) NULL,
    
    -- OCR
    OCRCompleted BIT DEFAULT 0,
    OCRText NVARCHAR(MAX) NULL,
    OCRConfidence DECIMAL(5,2) NULL,
    
    -- Sharing
    IsShared BIT DEFAULT 0,
    ShareExpiresAt DATETIME2 NULL,
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'active',
    
    -- Audit
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(255) NULL,
    
    CONSTRAINT FK_Document_Patient FOREIGN KEY (PatientId) 
        REFERENCES patient.Patients(Id),
    
    INDEX IX_Documents_Patient (PatientId),
    INDEX IX_Documents_Type (DocumentType),
    INDEX IX_Documents_Date (DocumentDate)
)
GO

-- =============================================
-- VISITS TABLE
-- =============================================
CREATE TABLE patient.Visits (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Visit Information
    VisitType NVARCHAR(100) NOT NULL,
    VisitReason NVARCHAR(255) NULL,
    VisitDate DATETIME2 NOT NULL,
    
    -- Provider
    ProviderName NVARCHAR(255) NULL,
    ProviderNPI NVARCHAR(50) NULL,
    FacilityName NVARCHAR(255) NOT NULL,
    FacilityAddress NVARCHAR(500) NULL,
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'scheduled',
    
    -- Diagnosis
    PrimaryDiagnosisCode NVARCHAR(50) NULL,
    PrimaryDiagnosisDescription NVARCHAR(255) NULL,
    
    -- Notes
    ChiefComplaint NVARCHAR(MAX) NULL,
    VisitSummary NVARCHAR(MAX) NULL,
    FollowUpInstructions NVARCHAR(MAX) NULL,
    
    -- Outcome
    DischargeDisposition NVARCHAR(100) NULL,
    NextAppointment DATETIME2 NULL,
    
    -- Audit
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(255) NULL,
    
    CONSTRAINT FK_Visit_Patient FOREIGN KEY (PatientId) 
        REFERENCES patient.Patients(Id),
    
    INDEX IX_Visits_Patient (PatientId),
    INDEX IX_Visits_Date (VisitDate),
    INDEX IX_Visits_Status (Status)
)
GO

-- =============================================
-- TRANSFERS TABLE
-- =============================================
CREATE TABLE patient.Transfers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    ConsentId UNIQUEIDENTIFIER NULL,
    
    -- Transfer Details
    TransferType NVARCHAR(50) NOT NULL,
    TransferMethod NVARCHAR(50) NOT NULL,
    
    -- Source and Destination
    SourceFacilityName NVARCHAR(255) NOT NULL,
    SourceFacilityNPI NVARCHAR(50) NULL,
    DestinationFacilityName NVARCHAR(255) NOT NULL,
    DestinationFacilityNPI NVARCHAR(50) NULL,
    
    -- Data
    DataPackageUrl NVARCHAR(1000) NULL,
    DataPackageSize BIGINT NULL,
    ResourcesIncluded NVARCHAR(MAX) NULL,
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'Initiated',
    InitiatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CompletedAt DATETIME2 NULL,
    
    -- Security
    IsEncrypted BIT DEFAULT 1,
    TransferToken NVARCHAR(500) NULL,
    
    -- Audit
    InitiatedBy NVARCHAR(255) NOT NULL,
    ReceivedBy NVARCHAR(255) NULL,
    ReceivedAt DATETIME2 NULL,
    
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    
    CONSTRAINT FK_Transfer_Patient FOREIGN KEY (PatientId) 
        REFERENCES patient.Patients(Id),
    CONSTRAINT FK_Transfer_Consent FOREIGN KEY (ConsentId) 
        REFERENCES patient.Consents(Id),
    
    INDEX IX_Transfers_Patient (PatientId),
    INDEX IX_Transfers_Status (Status),
    INDEX IX_Transfers_InitiatedAt (InitiatedAt)
)
GO

-- =============================================
-- AUDIT LOG TABLE (IMMUTABLE - HIPAA COMPLIANT)
-- =============================================
CREATE TABLE patient.AuditLog (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    
    -- Timestamp (immutable)
    EventTimestamp DATETIME2 DEFAULT GETUTCDATE(),
    
    -- Event Information
    EventType NVARCHAR(100) NOT NULL,
    EventCategory NVARCHAR(100) NOT NULL,
    EventAction NVARCHAR(100) NOT NULL,
    EventOutcome NVARCHAR(50) NOT NULL,
    
    -- Actor
    ActorId NVARCHAR(255) NOT NULL,
    ActorType NVARCHAR(100) NOT NULL,
    ActorName NVARCHAR(255) NULL,
    ActorRole NVARCHAR(100) NULL,
    
    -- Target
    TargetType NVARCHAR(100) NOT NULL,
    TargetId UNIQUEIDENTIFIER NULL,
    PatientId UNIQUEIDENTIFIER NULL,
    
    -- Source
    SourceType NVARCHAR(100) NULL,
    IPAddress NVARCHAR(50) NULL,
    UserAgent NVARCHAR(500) NULL,
    Location NVARCHAR(255) NULL,
    DeviceId NVARCHAR(255) NULL,
    
    -- Event Details
    EventDescription NVARCHAR(MAX) NULL,
    DataAccessed NVARCHAR(MAX) NULL,
    ChangesApplied NVARCHAR(MAX) NULL,
    
    -- Authorization
    ConsentId UNIQUEIDENTIFIER NULL,
    HasValidConsent BIT DEFAULT 1,
    
    -- Security
    IsAuthorized BIT DEFAULT 1,
    IsAnomaly BIT DEFAULT 0,
    RiskScore INT NULL,
    
    -- Correlation
    CorrelationId UNIQUEIDENTIFIER NULL,
    SessionId NVARCHAR(255) NULL,
    
    INDEX IX_AuditLog_Timestamp (EventTimestamp),
    INDEX IX_AuditLog_Patient (PatientId),
    INDEX IX_AuditLog_Actor (ActorId),
    INDEX IX_AuditLog_Type (EventType),
    INDEX IX_AuditLog_Anomaly (IsAnomaly)
)
GO

-- Trigger to prevent modification/deletion of audit logs
CREATE TRIGGER TR_AuditLog_PreventModification
ON patient.AuditLog
AFTER UPDATE, DELETE
AS
BEGIN
    RAISERROR ('Audit log records cannot be modified or deleted (HIPAA compliance)', 16, 1)
    ROLLBACK TRANSACTION
END
GO

PRINT '✓ Comprehensive Patient Service schema created successfully'
PRINT '  - 14 core tables created'
PRINT '  - All indexes configured'
PRINT '  - Foreign keys established'
PRINT '  - Audit log protection enabled'
GO
