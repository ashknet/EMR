-- =============================================
-- Healthcare Platform - Hospital Agent Service Schema
-- Hospital-side operations, QR scanning, OCR processing
-- =============================================

USE [HealthcarePlatform]
GO

-- Create agent schema
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'agent')
BEGIN
    EXEC('CREATE SCHEMA agent')
END
GO

-- =============================================
-- HOSPITAL AGENTS TABLE
-- =============================================
CREATE TABLE agent.HospitalAgents (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    
    -- Hospital Information
    HospitalName NVARCHAR(255) NOT NULL,
    HospitalNPI NVARCHAR(50) NULL,
    HospitalAddress NVARCHAR(500) NULL,
    ContactEmail NVARCHAR(255) NULL,
    ContactPhone NVARCHAR(50) NULL,
    
    -- Agent Information
    AgentVersion NVARCHAR(50) NOT NULL,
    InstallationDate DATETIME2 DEFAULT GETUTCDATE(),
    LastUpdateDate DATETIME2 NULL,
    
    -- Device Information
    DeviceId NVARCHAR(255) NOT NULL UNIQUE,
    MachineName NVARCHAR(255) NULL,
    IPAddress NVARCHAR(50) NULL,
    WindowsVersion NVARCHAR(100) NULL,
    AgentExecutablePath NVARCHAR(1000) NULL,
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'Active',
    IsOnline BIT DEFAULT 0,
    LastHeartbeat DATETIME2 NULL,
    LastActivityAt DATETIME2 NULL,
    
    -- Security
    ApiKey NVARCHAR(500) NULL,
    CertificateThumbprint NVARCHAR(100) NULL,
    EncryptionEnabled BIT DEFAULT 1,
    
    -- Statistics
    TotalScansProcessed INT DEFAULT 0,
    TotalDocumentsIngested INT DEFAULT 0,
    TotalTransfersCompleted INT DEFAULT 0,
    TotalOCRProcessed INT DEFAULT 0,
    
    -- Configuration
    AutoUpdateEnabled BIT DEFAULT 1,
    OCREnabled BIT DEFAULT 1,
    EmailIngestionEnabled BIT DEFAULT 0,
    FaxIngestionEnabled BIT DEFAULT 0,
    QRScanningEnabled BIT DEFAULT 1,
    
    -- Audit
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    
    INDEX IX_HospitalAgents_DeviceId (DeviceId),
    INDEX IX_HospitalAgents_Status (Status),
    INDEX IX_HospitalAgents_IsOnline (IsOnline),
    INDEX IX_HospitalAgents_LastHeartbeat (LastHeartbeat)
)
GO

-- =============================================
-- AGENT SESSIONS TABLE
-- =============================================
CREATE TABLE agent.AgentSessions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AgentId UNIQUEIDENTIFIER NOT NULL,
    
    -- Session Information
    SessionId NVARCHAR(255) NOT NULL UNIQUE,
    SessionStarted DATETIME2 DEFAULT GETUTCDATE(),
    SessionEnded DATETIME2 NULL,
    SessionDuration INT NULL, -- seconds
    
    -- User Information
    OperatorName NVARCHAR(255) NULL,
    OperatorRole NVARCHAR(100) NULL,
    
    -- Activity Summary
    QRScansCount INT DEFAULT 0,
    DocumentsProcessedCount INT DEFAULT 0,
    TransfersInitiatedCount INT DEFAULT 0,
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'Active',
    
    CONSTRAINT FK_AgentSession_Agent FOREIGN KEY (AgentId) 
        REFERENCES agent.HospitalAgents(Id),
    
    INDEX IX_AgentSessions_Agent (AgentId),
    INDEX IX_AgentSessions_Started (SessionStarted),
    INDEX IX_AgentSessions_Status (Status)
)
GO

-- =============================================
-- QR SCANS TABLE
-- =============================================
CREATE TABLE agent.QRScans (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AgentId UNIQUEIDENTIFIER NOT NULL,
    SessionId UNIQUEIDENTIFIER NULL,
    
    -- QR Code Information
    QRCodeData NVARCHAR(MAX) NOT NULL,
    ConsentId UNIQUEIDENTIFIER NULL,
    PatientId UNIQUEIDENTIFIER NULL,
    
    -- Scan Details
    ScannedAt DATETIME2 DEFAULT GETUTCDATE(),
    ScanMethod NVARCHAR(100) DEFAULT 'Camera', -- Camera, Manual Entry
    
    -- Validation
    IsValid BIT DEFAULT 0,
    ValidationMessage NVARCHAR(MAX) NULL,
    IsExpired BIT DEFAULT 0,
    MaxUsageReached BIT DEFAULT 0,
    
    -- Data Retrieved
    DataRetrievedSuccessfully BIT DEFAULT 0,
    DataPackageSize BIGINT NULL,
    ResourcesRetrieved NVARCHAR(MAX) NULL, -- JSON array
    
    -- Usage
    WasUsedForIntake BIT DEFAULT 0,
    IntakeFormFilled BIT DEFAULT 0,
    IntakeFormType NVARCHAR(100) NULL,
    
    -- Operator
    ScannedBy NVARCHAR(255) NULL,
    OperatorNotes NVARCHAR(MAX) NULL,
    
    -- Audit
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_QRScan_Agent FOREIGN KEY (AgentId) 
        REFERENCES agent.HospitalAgents(Id),
    CONSTRAINT FK_QRScan_Session FOREIGN KEY (SessionId) 
        REFERENCES agent.AgentSessions(Id),
    
    INDEX IX_QRScans_Agent (AgentId),
    INDEX IX_QRScans_ScannedAt (ScannedAt),
    INDEX IX_QRScans_Patient (PatientId),
    INDEX IX_QRScans_Valid (IsValid)
)
GO

-- =============================================
-- DOCUMENT PROCESSING TABLE
-- =============================================
CREATE TABLE agent.DocumentProcessing (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AgentId UNIQUEIDENTIFIER NOT NULL,
    SessionId UNIQUEIDENTIFIER NULL,
    
    -- Source Information
    SourceType NVARCHAR(50) NOT NULL, -- Email, Fax, Upload, Scan
    SourceIdentifier NVARCHAR(255) NULL,
    ReceivedAt DATETIME2 DEFAULT GETUTCDATE(),
    
    -- Document Information
    DocumentName NVARCHAR(255) NOT NULL,
    FileType NVARCHAR(50) NOT NULL,
    FileSizeBytes BIGINT NOT NULL,
    BlobStorageUrl NVARCHAR(1000) NULL,
    LocalFilePath NVARCHAR(1000) NULL,
    
    -- Patient Matching
    PatientId UNIQUEIDENTIFIER NULL,
    MatchMethod NVARCHAR(100) NULL, -- QR, Name+DOB, MRN, Manual
    MatchConfidence DECIMAL(5,2) NULL,
    IsMatched BIT DEFAULT 0,
    
    -- OCR Processing
    OCRStatus NVARCHAR(50) DEFAULT 'Pending', -- Pending, Processing, Completed, Failed
    OCRStartedAt DATETIME2 NULL,
    OCRCompletedAt DATETIME2 NULL,
    OCRText NVARCHAR(MAX) NULL,
    OCRConfidence DECIMAL(5,2) NULL,
    OCRServiceUsed NVARCHAR(100) NULL, -- Azure Form Recognizer, Custom
    
    -- Data Extraction
    ExtractedData NVARCHAR(MAX) NULL, -- JSON
    DocumentType NVARCHAR(100) NULL, -- Lab Report, Prescription, Referral, etc.
    DocumentCategory NVARCHAR(100) NULL,
    
    -- FHIR Conversion
    FHIRConversionStatus NVARCHAR(50) DEFAULT 'Pending',
    FHIRResourcesCreated NVARCHAR(MAX) NULL, -- JSON array
    FHIRBundleUrl NVARCHAR(1000) NULL,
    
    -- Upload Status
    UploadedToPortal BIT DEFAULT 0,
    UploadedAt DATETIME2 NULL,
    PortalDocumentId UNIQUEIDENTIFIER NULL,
    
    -- Processing Status
    ProcessingStatus NVARCHAR(50) DEFAULT 'Pending',
    ProcessedAt DATETIME2 NULL,
    ProcessingDuration INT NULL, -- seconds
    
    -- Error Handling
    ErrorMessage NVARCHAR(MAX) NULL,
    RetryCount INT DEFAULT 0,
    
    -- Operator
    ProcessedBy NVARCHAR(255) NULL,
    OperatorNotes NVARCHAR(MAX) NULL,
    
    -- Audit
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    
    CONSTRAINT FK_DocumentProcessing_Agent FOREIGN KEY (AgentId) 
        REFERENCES agent.HospitalAgents(Id),
    CONSTRAINT FK_DocumentProcessing_Session FOREIGN KEY (SessionId) 
        REFERENCES agent.AgentSessions(Id),
    
    INDEX IX_DocumentProcessing_Agent (AgentId),
    INDEX IX_DocumentProcessing_ReceivedAt (ReceivedAt),
    INDEX IX_DocumentProcessing_Patient (PatientId),
    INDEX IX_DocumentProcessing_OCRStatus (OCRStatus),
    INDEX IX_DocumentProcessing_ProcessingStatus (ProcessingStatus)
)
GO

-- =============================================
-- EMR INTEGRATIONS TABLE
-- =============================================
CREATE TABLE agent.EMRIntegrations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AgentId UNIQUEIDENTIFIER NOT NULL,
    
    -- EMR System
    EMRSystemName NVARCHAR(255) NOT NULL, -- Epic, Cerner, Athena, etc.
    EMRSystemVersion NVARCHAR(100) NULL,
    EMRVendor NVARCHAR(255) NULL,
    
    -- Integration Method
    IntegrationMethod NVARCHAR(100) NOT NULL, -- API, Clipboard, File Export, HL7
    IntegrationStatus NVARCHAR(50) DEFAULT 'Active',
    
    -- API Details (if applicable)
    ApiEndpoint NVARCHAR(1000) NULL,
    ApiVersion NVARCHAR(50) NULL,
    AuthenticationType NVARCHAR(100) NULL,
    
    -- File Export Details
    ExportFormat NVARCHAR(50) NULL, -- FHIR JSON, FHIR XML, HL7, CCD/CDA
    ExportPath NVARCHAR(1000) NULL,
    
    -- Usage Statistics
    TotalIntegrations INT DEFAULT 0,
    LastIntegrationAt DATETIME2 NULL,
    SuccessRate DECIMAL(5,2) NULL,
    
    -- Configuration
    IsActive BIT DEFAULT 1,
    AutoFillEnabled BIT DEFAULT 1,
    
    -- Audit
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    
    CONSTRAINT FK_EMRIntegration_Agent FOREIGN KEY (AgentId) 
        REFERENCES agent.HospitalAgents(Id),
    
    INDEX IX_EMRIntegrations_Agent (AgentId),
    INDEX IX_EMRIntegrations_Status (IntegrationStatus)
)
GO

-- =============================================
-- AGENT ACTIVITY LOG
-- =============================================
CREATE TABLE agent.AgentActivityLog (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AgentId UNIQUEIDENTIFIER NOT NULL,
    SessionId UNIQUEIDENTIFIER NULL,
    
    -- Activity Information
    ActivityType NVARCHAR(100) NOT NULL,
    ActivityCategory NVARCHAR(100) NOT NULL,
    ActivityDescription NVARCHAR(MAX) NULL,
    ActivityTimestamp DATETIME2 DEFAULT GETUTCDATE(),
    
    -- Status
    Status NVARCHAR(50) NOT NULL, -- Success, Failure, Warning, Info
    ErrorMessage NVARCHAR(MAX) NULL,
    ErrorCode NVARCHAR(50) NULL,
    
    -- Related Entities
    RelatedPatientId UNIQUEIDENTIFIER NULL,
    RelatedQRScanId UNIQUEIDENTIFIER NULL,
    RelatedDocumentId UNIQUEIDENTIFIER NULL,
    
    -- Performance
    DurationMs INT NULL,
    
    -- Operator
    OperatorName NVARCHAR(255) NULL,
    
    -- System Info
    MachineName NVARCHAR(255) NULL,
    IPAddress NVARCHAR(50) NULL,
    
    CONSTRAINT FK_AgentActivityLog_Agent FOREIGN KEY (AgentId) 
        REFERENCES agent.HospitalAgents(Id),
    CONSTRAINT FK_AgentActivityLog_Session FOREIGN KEY (SessionId) 
        REFERENCES agent.AgentSessions(Id),
    
    INDEX IX_AgentActivityLog_Agent (AgentId),
    INDEX IX_AgentActivityLog_Timestamp (ActivityTimestamp),
    INDEX IX_AgentActivityLog_Type (ActivityType),
    INDEX IX_AgentActivityLog_Status (Status)
)
GO

-- =============================================
-- STORED PROCEDURE: Register New Agent
-- =============================================
CREATE OR ALTER PROCEDURE agent.usp_Agent_Register
    @HospitalName NVARCHAR(255),
    @DeviceId NVARCHAR(255),
    @AgentVersion NVARCHAR(50),
    @MachineName NVARCHAR(255) = NULL,
    @IPAddress NVARCHAR(50) = NULL,
    @CreatedBy NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @NewAgentId UNIQUEIDENTIFIER = NEWID();
    DECLARE @ApiKey NVARCHAR(500) = 'AGENT-' + CONVERT(NVARCHAR(50), NEWID());
    
    INSERT INTO agent.HospitalAgents (
        Id, HospitalName, DeviceId, AgentVersion,
        MachineName, IPAddress, ApiKey, Status, IsOnline,
        CreatedBy, CreatedAt
    )
    VALUES (
        @NewAgentId, @HospitalName, @DeviceId, @AgentVersion,
        @MachineName, @IPAddress, @ApiKey, 'Active', 1,
        @CreatedBy, GETUTCDATE()
    );
    
    -- Log registration
    INSERT INTO agent.AgentActivityLog (
        AgentId, ActivityType, ActivityCategory, ActivityDescription,
        Status, MachineName, IPAddress
    )
    VALUES (
        @NewAgentId, 'AgentRegistration', 'System', 
        'Hospital agent registered: ' + @HospitalName,
        'Success', @MachineName, @IPAddress
    );
    
    SELECT * FROM agent.HospitalAgents WHERE Id = @NewAgentId;
END
GO

PRINT '✓ Hospital Agent Service schema created successfully'
PRINT '  - 7 core tables created'
PRINT '  - HospitalAgents (registration, monitoring)'
PRINT '  - AgentSessions (session tracking)'
PRINT '  - QRScans (QR code scanning)'
PRINT '  - DocumentProcessing (OCR, FHIR conversion)'
PRINT '  - EMRIntegrations (EMR system connections)'
PRINT '  - AgentActivityLog (comprehensive logging)'
PRINT '  - Registration stored procedure created'
GO
