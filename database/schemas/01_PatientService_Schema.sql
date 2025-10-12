-- =============================================
-- Healthcare Platform - Patient Service Database Schema
-- HIPAA-compliant patient data management
-- All PHI fields use encryption at rest (AES-256)
-- =============================================

-- Create schema
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'patient')
BEGIN
    EXEC('CREATE SCHEMA patient')
END
GO

-- =============================================
-- Table: Patients
-- Purpose: Store patient demographic and profile information
-- Compliance: All sensitive fields encrypted, audit fields tracked
-- =============================================
CREATE TABLE patient.Patients (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    
    -- Identity Information
    FirstName NVARCHAR(100) NOT NULL,
    MiddleName NVARCHAR(100) NULL,
    LastName NVARCHAR(100) NOT NULL,
    PreferredName NVARCHAR(100) NULL,
    DateOfBirth DATE NOT NULL,
    Gender NVARCHAR(20) NOT NULL, -- Male, Female, Other, Unknown
    BiologicalSex NVARCHAR(20) NULL,
    
    -- Contact Information (Encrypted)
    Email NVARCHAR(500) NULL, -- Encrypted
    PhoneNumber NVARCHAR(500) NULL, -- Encrypted
    AlternatePhoneNumber NVARCHAR(500) NULL, -- Encrypted
    
    -- Address Information (Encrypted)
    AddressLine1 NVARCHAR(500) NULL, -- Encrypted
    AddressLine2 NVARCHAR(500) NULL, -- Encrypted
    City NVARCHAR(200) NULL,
    State NVARCHAR(50) NULL,
    ZipCode NVARCHAR(20) NULL,
    Country NVARCHAR(50) DEFAULT 'USA',
    
    -- Identification Numbers (Encrypted)
    SSN NVARCHAR(500) NULL, -- Encrypted SSN
    SSNHash NVARCHAR(100) NULL, -- SHA-256 hash for searching
    MedicalRecordNumber NVARCHAR(100) NULL,
    DriverLicenseNumber NVARCHAR(500) NULL, -- Encrypted
    PassportNumber NVARCHAR(500) NULL, -- Encrypted
    
    -- Demographics
    Race NVARCHAR(100) NULL,
    Ethnicity NVARCHAR(100) NULL,
    PreferredLanguage NVARCHAR(50) DEFAULT 'English',
    MaritalStatus NVARCHAR(50) NULL,
    
    -- Emergency Contact (Encrypted)
    EmergencyContactName NVARCHAR(500) NULL, -- Encrypted
    EmergencyContactRelationship NVARCHAR(100) NULL,
    EmergencyContactPhone NVARCHAR(500) NULL, -- Encrypted
    
    -- Account Information
    PrimaryAccountHolderId UNIQUEIDENTIFIER NULL,
    IsPrimaryAccountHolder BIT DEFAULT 0,
    ProfileType NVARCHAR(50) DEFAULT 'Self', -- Self, Spouse, Child, Parent, Sibling, Other
    RelationshipToPrimary NVARCHAR(100) NULL,
    
    -- FHIR Compliance
    FHIRPatientId NVARCHAR(100) NULL,
    FHIRResourceJson NVARCHAR(MAX) NULL, -- Cached FHIR Patient resource
    
    -- Status and Flags
    IsActive BIT DEFAULT 1,
    IsVerified BIT DEFAULT 0,
    VerifiedAt DATETIME2 NULL,
    IsMinor BIT DEFAULT 0,
    DeceasedDate DATETIME2 NULL,
    
    -- Privacy and Consent
    HasConsentedToDataSharing BIT DEFAULT 0,
    ConsentDate DATETIME2 NULL,
    ConsentVersion NVARCHAR(20) NULL,
    
    -- Audit Trail
    LastAccessedBy NVARCHAR(255) NULL,
    LastAccessedAt DATETIME2 NULL,
    
    -- Photo/Avatar
    ProfilePhotoUrl NVARCHAR(1000) NULL,
    
    -- Base Entity Fields
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(255) NULL,
    RowVersion ROWVERSION,
    
    -- Indexes
    INDEX IX_Patient_Email (Email),
    INDEX IX_Patient_SSNHash (SSNHash),
    INDEX IX_Patient_MRN (MedicalRecordNumber),
    INDEX IX_Patient_PrimaryAccountHolder (PrimaryAccountHolderId),
    INDEX IX_Patient_FHIRId (FHIRPatientId),
    INDEX IX_Patient_Name (LastName, FirstName),
    INDEX IX_Patient_IsActive (IsActive),
    INDEX IX_Patient_IsDeleted (IsDeleted)
)
GO

-- =============================================
-- Table: PatientDocuments
-- Purpose: Store patient document metadata and storage references
-- Documents stored in encrypted Azure Blob Storage
-- =============================================
CREATE TABLE patient.PatientDocuments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Document Information
    DocumentType NVARCHAR(100) NOT NULL, -- ID, Insurance, Medical, Lab, Imaging, Prescription, etc.
    DocumentName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(1000) NULL,
    DocumentDate DATE NULL,
    
    -- Storage Information
    BlobStorageUrl NVARCHAR(1000) NOT NULL, -- Encrypted storage URL
    ThumbnailUrl NVARCHAR(1000) NULL,
    FileType NVARCHAR(50) NOT NULL, -- PDF, JPG, PNG, TIFF, DICOM, etc.
    FileSizeBytes BIGINT NOT NULL,
    
    -- Encryption Information
    IsEncrypted BIT DEFAULT 1,
    EncryptionKeyId NVARCHAR(255) NULL,
    
    -- FHIR Compliance
    FHIRDocumentReferenceId NVARCHAR(100) NULL,
    
    -- OCR Processing
    OCRText NVARCHAR(MAX) NULL,
    OCRProcessed BIT DEFAULT 0,
    OCRProcessedAt DATETIME2 NULL,
    OCRConfidenceScore DECIMAL(5,2) NULL,
    
    -- Upload Information
    UploadedBy NVARCHAR(255) NOT NULL,
    UploadedAt DATETIME2 DEFAULT GETUTCDATE(),
    
    -- Sharing Information
    IsShared BIT DEFAULT 0,
    SharedWith NVARCHAR(MAX) NULL, -- JSON array of recipients
    SharedUntil DATETIME2 NULL,
    
    -- Base Entity Fields
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(255) NULL,
    RowVersion ROWVERSION,
    
    -- Foreign Keys
    CONSTRAINT FK_PatientDocuments_Patient FOREIGN KEY (PatientId) 
        REFERENCES patient.Patients(Id) ON DELETE CASCADE,
    
    -- Indexes
    INDEX IX_PatientDocuments_PatientId (PatientId),
    INDEX IX_PatientDocuments_Type (DocumentType),
    INDEX IX_PatientDocuments_UploadedAt (UploadedAt),
    INDEX IX_PatientDocuments_IsDeleted (IsDeleted)
)
GO

-- =============================================
-- Table: PatientNotes
-- Purpose: Store patient notes, reminders, and annotations
-- =============================================
CREATE TABLE patient.PatientNotes (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Note Information
    NoteType NVARCHAR(50) NOT NULL, -- Personal, Medical, Administrative
    Title NVARCHAR(255) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    IsImportant BIT DEFAULT 0,
    IsPrivate BIT DEFAULT 1,
    
    -- Tags and Organization
    Tags NVARCHAR(500) NULL, -- Comma-separated tags
    
    -- Reminder Information
    ReminderDate DATETIME2 NULL,
    IsReminderSent BIT DEFAULT 0,
    ReminderSentAt DATETIME2 NULL,
    
    -- Base Entity Fields
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(255) NULL,
    RowVersion ROWVERSION,
    
    -- Foreign Keys
    CONSTRAINT FK_PatientNotes_Patient FOREIGN KEY (PatientId) 
        REFERENCES patient.Patients(Id) ON DELETE CASCADE,
    
    -- Indexes
    INDEX IX_PatientNotes_PatientId (PatientId),
    INDEX IX_PatientNotes_Type (NoteType),
    INDEX IX_PatientNotes_ReminderDate (ReminderDate),
    INDEX IX_PatientNotes_IsDeleted (IsDeleted)
)
GO

-- =============================================
-- Table: PatientAccessLog
-- Purpose: HIPAA-compliant audit log for all patient data access
-- Immutable: No updates or deletes allowed
-- =============================================
CREATE TABLE patient.PatientAccessLog (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Access Information
    AccessType NVARCHAR(50) NOT NULL, -- View, Create, Update, Delete, Export, Share
    AccessedBy NVARCHAR(255) NOT NULL,
    AccessedByRole NVARCHAR(100) NULL,
    AccessedAt DATETIME2 DEFAULT GETUTCDATE(),
    
    -- Access Details
    IPAddress NVARCHAR(50) NULL,
    UserAgent NVARCHAR(500) NULL,
    Location NVARCHAR(200) NULL,
    
    -- Data Accessed
    DataFields NVARCHAR(MAX) NULL, -- JSON array of fields accessed
    Reason NVARCHAR(500) NULL,
    
    -- Consent Information
    ConsentId UNIQUEIDENTIFIER NULL,
    HasValidConsent BIT DEFAULT 1,
    
    -- Security
    IsAuthorized BIT DEFAULT 1,
    AuthorizationMethod NVARCHAR(100) NULL, -- OAuth, MFA, etc.
    
    INDEX IX_PatientAccessLog_PatientId (PatientId),
    INDEX IX_PatientAccessLog_AccessedAt (AccessedAt),
    INDEX IX_PatientAccessLog_AccessedBy (AccessedBy)
)
GO

-- Add comments to tables
EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'HIPAA-compliant patient demographic and profile information with encrypted PHI fields',
    @level0type = N'SCHEMA', @level0name = 'patient',
    @level1type = N'TABLE', @level1name = 'Patients'
GO

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Patient document metadata with references to encrypted blob storage',
    @level0type = N'SCHEMA', @level0name = 'patient',
    @level1type = N'TABLE', @level1name = 'PatientDocuments'
GO

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Patient notes, reminders, and annotations',
    @level0type = N'SCHEMA', @level0name = 'patient',
    @level1type = N'TABLE', @level1name = 'PatientNotes'
GO

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'HIPAA-compliant immutable audit log for all patient data access',
    @level0type = N'SCHEMA', @level0name = 'patient',
    @level1type = N'TABLE', @level1name = 'PatientAccessLog'
GO
