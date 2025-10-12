-- =============================================
-- Healthcare Platform - Consent & Audit Service Database Schema
-- HIPAA-compliant consent management and comprehensive audit logging
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'consent')
BEGIN
    EXEC('CREATE SCHEMA consent')
END
GO

-- =============================================
-- Table: Consents
-- Purpose: Patient consent records for data sharing and access
-- Maps to FHIR Consent resource
-- =============================================
CREATE TABLE consent.Consents (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Consent Information
    ConsentType NVARCHAR(100) NOT NULL, -- DataSharing, Treatment, Research, Marketing, GeneralConsent
    ConsentScope NVARCHAR(100) NOT NULL, -- PatientPrivacy, Research, AdrDecision, Treatment
    ConsentCategory NVARCHAR(100) NULL, -- HIPAA, Research, AdvanceDirective
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'active', -- draft, proposed, active, rejected, inactive, entered-in-error
    
    -- Period
    EffectiveDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ExpirationDate DATETIME2 NULL,
    IsIndefinite BIT DEFAULT 0,
    
    -- Granular Permissions
    PolicyRule NVARCHAR(50) NOT NULL, -- permit, deny
    CanView BIT DEFAULT 0,
    CanUpdate BIT DEFAULT 0,
    CanDelete BIT DEFAULT 0,
    CanShare BIT DEFAULT 0,
    CanExport BIT DEFAULT 0,
    
    -- Data Categories Covered
    CoverageScope NVARCHAR(50) DEFAULT 'all', -- all, allergies, conditions, medications, labs, imaging, etc.
    DataCategories NVARCHAR(MAX) NULL, -- JSON array of specific data categories
    
    -- Actors (Who can access)
    GrantedToType NVARCHAR(50) NULL, -- Organization, Person, RelatedPerson, CareTeam
    GrantedToId UNIQUEIDENTIFIER NULL,
    GrantedToName NVARCHAR(255) NULL,
    GrantedToOrganization NVARCHAR(255) NULL,
    
    -- Purpose
    Purpose NVARCHAR(MAX) NULL, -- JSON array of purposes (treatment, payment, operations, research)
    PurposeDescription NVARCHAR(500) NULL,
    
    -- Signature and Verification
    IsSignedElectronically BIT DEFAULT 0,
    SignatureData NVARCHAR(MAX) NULL, -- Base64 encoded signature
    SignedDate DATETIME2 NULL,
    WitnessName NVARCHAR(255) NULL,
    WitnessSignature NVARCHAR(MAX) NULL,
    
    -- Documentation
    ConsentFormUrl NVARCHAR(1000) NULL,
    ConsentFormVersion NVARCHAR(50) NULL,
    
    -- Source
    SourceType NVARCHAR(50) DEFAULT 'patient', -- patient, guardian, power-of-attorney
    SourceReferenceId UNIQUEIDENTIFIER NULL,
    
    -- QR Code for Sharing
    QRCodeData NVARCHAR(MAX) NULL,
    QRCodeExpiration DATETIME2 NULL,
    QRCodeUsageCount INT DEFAULT 0,
    QRCodeMaxUsage INT NULL,
    
    -- Revocation
    IsRevoked BIT DEFAULT 0,
    RevokedAt DATETIME2 NULL,
    RevokedBy NVARCHAR(255) NULL,
    RevocationReason NVARCHAR(500) NULL,
    
    -- FHIR Compliance
    FHIRResourceId NVARCHAR(100) NULL,
    FHIRResourceJson NVARCHAR(MAX) NULL,
    
    -- Base Entity Fields
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(255) NULL,
    RowVersion ROWVERSION,
    
    INDEX IX_Consents_PatientId (PatientId),
    INDEX IX_Consents_Status (Status),
    INDEX IX_Consents_Type (ConsentType),
    INDEX IX_Consents_EffectiveDate (EffectiveDate),
    INDEX IX_Consents_IsRevoked (IsRevoked),
    INDEX IX_Consents_IsDeleted (IsDeleted)
)
GO

-- =============================================
-- Table: AuditLog
-- Purpose: Comprehensive HIPAA-compliant audit trail
-- Immutable: No updates or deletes allowed
-- =============================================
CREATE TABLE consent.AuditLog (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    
    -- Timestamp (cannot be modified)
    EventTimestamp DATETIME2 DEFAULT GETUTCDATE() NOT NULL,
    EventDate AS CAST(EventTimestamp AS DATE) PERSISTED, -- For efficient date queries
    
    -- Event Information
    EventType NVARCHAR(100) NOT NULL, -- Access, Create, Update, Delete, Export, Share, Login, Logout
    EventCategory NVARCHAR(100) NOT NULL, -- Security, Privacy, DataAccess, Administrative
    EventAction NVARCHAR(100) NOT NULL, -- View, Create, Update, Delete, Execute, etc.
    EventOutcome NVARCHAR(50) NOT NULL, -- Success, Failure, PartialSuccess
    
    -- Actor (Who performed the action)
    ActorId NVARCHAR(255) NOT NULL,
    ActorType NVARCHAR(50) NOT NULL, -- Patient, Provider, System, Administrator, Agent
    ActorName NVARCHAR(255) NULL,
    ActorRole NVARCHAR(100) NULL,
    
    -- Target (What was accessed/modified)
    TargetType NVARCHAR(100) NOT NULL, -- Patient, Document, Consent, Insurance, etc.
    TargetId UNIQUEIDENTIFIER NULL,
    PatientId UNIQUEIDENTIFIER NULL, -- For PHI-related events
    
    -- Source (Where did the action originate)
    SourceType NVARCHAR(50) NULL, -- Portal, API, MobileApp, DesktopAgent, System
    SourceIdentifier NVARCHAR(255) NULL,
    IPAddress NVARCHAR(50) NULL,
    UserAgent NVARCHAR(500) NULL,
    Location NVARCHAR(200) NULL,
    DeviceId NVARCHAR(255) NULL,
    
    -- Event Details
    EventDescription NVARCHAR(MAX) NULL,
    DataAccessed NVARCHAR(MAX) NULL, -- JSON array of fields/resources accessed
    ChangesApplied NVARCHAR(MAX) NULL, -- JSON with before/after values (if applicable)
    
    -- Authorization
    ConsentId UNIQUEIDENTIFIER NULL,
    HasValidConsent BIT DEFAULT 1,
    AuthorizationMethod NVARCHAR(100) NULL, -- OAuth, MFA, Biometric, QRCode
    
    -- Security
    IsAuthorized BIT DEFAULT 1,
    IsAnomaly BIT DEFAULT 0, -- Flagged by security monitoring
    RiskScore INT NULL, -- 0-100, calculated risk score
    
    -- Compliance
    ComplianceFlags NVARCHAR(MAX) NULL, -- JSON with compliance-related flags
    RequiresReview BIT DEFAULT 0,
    ReviewedAt DATETIME2 NULL,
    ReviewedBy NVARCHAR(255) NULL,
    
    -- FHIR Compliance (maps to AuditEvent)
    FHIRResourceId NVARCHAR(100) NULL,
    FHIRResourceJson NVARCHAR(MAX) NULL,
    
    -- Correlation
    CorrelationId UNIQUEIDENTIFIER NULL, -- For tracking related events
    SessionId NVARCHAR(255) NULL,
    
    INDEX IX_AuditLog_EventTimestamp (EventTimestamp),
    INDEX IX_AuditLog_EventDate (EventDate),
    INDEX IX_AuditLog_PatientId (PatientId),
    INDEX IX_AuditLog_ActorId (ActorId),
    INDEX IX_AuditLog_EventType (EventType),
    INDEX IX_AuditLog_EventOutcome (EventOutcome),
    INDEX IX_AuditLog_IsAnomaly (IsAnomaly),
    INDEX IX_AuditLog_CorrelationId (CorrelationId)
)
GO

-- Prevent updates and deletes on AuditLog
CREATE TRIGGER consent.TR_AuditLog_PreventModification
ON consent.AuditLog
INSTEAD OF UPDATE, DELETE
AS
BEGIN
    RAISERROR('Audit log records cannot be modified or deleted. This table is immutable for compliance.', 16, 1)
    ROLLBACK TRANSACTION
END
GO

-- =============================================
-- Table: DataSharingEvents
-- Purpose: Track all data sharing via QR codes or direct transfers
-- =============================================
CREATE TABLE consent.DataSharingEvents (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    ConsentId UNIQUEIDENTIFIER NULL,
    
    -- Sharing Details
    ShareMethod NVARCHAR(50) NOT NULL, -- QRCode, DirectTransfer, Email, Download
    SharedWith NVARCHAR(255) NOT NULL, -- Organization, facility, or individual
    SharedWithType NVARCHAR(50) NULL, -- Hospital, Clinic, Provider, Family
    
    -- What was shared
    DataShared NVARCHAR(MAX) NULL, -- JSON array of resources/documents shared
    ShareScope NVARCHAR(50) NULL, -- Full, Partial, Specific
    
    -- Time window
    ShareInitiatedAt DATETIME2 DEFAULT GETUTCDATE(),
    ShareAccessedAt DATETIME2 NULL,
    ShareExpiresAt DATETIME2 NULL,
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'active', -- active, accessed, expired, revoked
    AccessCount INT DEFAULT 0,
    MaxAccessCount INT NULL,
    
    -- QR Code (if applicable)
    QRCodeToken NVARCHAR(500) NULL,
    QRCodeScannedBy NVARCHAR(255) NULL,
    QRCodeScannedAt DATETIME2 NULL,
    
    -- Location
    SharedAtLocation NVARCHAR(255) NULL,
    IPAddress NVARCHAR(50) NULL,
    
    -- Purpose
    SharePurpose NVARCHAR(500) NULL,
    
    INDEX IX_DataSharingEvents_PatientId (PatientId),
    INDEX IX_DataSharingEvents_ConsentId (ConsentId),
    INDEX IX_DataSharingEvents_Status (Status),
    INDEX IX_DataSharingEvents_InitiatedAt (ShareInitiatedAt)
)
GO

PRINT 'Consent & Audit Service schema created successfully'
GO
