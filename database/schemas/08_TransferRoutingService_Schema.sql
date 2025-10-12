-- =============================================
-- Healthcare Platform - Transfer/Routing Service Database Schema
-- Secure inter-hospital record transfers
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'transfer')
BEGIN
    EXEC('CREATE SCHEMA transfer')
END
GO

CREATE TABLE transfer.RecordTransfers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    
    -- Patient Information
    PatientId UNIQUEIDENTIFIER NOT NULL,
    ConsentId UNIQUEIDENTIFIER NULL,
    
    -- Transfer Details
    TransferType NVARCHAR(50) NOT NULL, -- Full, Partial, Emergency
    TransferMethod NVARCHAR(50) NOT NULL, -- FHIR, HL7, DirectEmail, Download
    
    -- Source and Destination
    SourceFacilityName NVARCHAR(255) NOT NULL,
    SourceFacilityNPI NVARCHAR(50) NULL,
    DestinationFacilityName NVARCHAR(255) NOT NULL,
    DestinationFacilityNPI NVARCHAR(50) NULL,
    
    -- Data Transferred
    DataPackageUrl NVARCHAR(1000) NULL,
    DataPackageSize BIGINT NULL,
    ResourcesIncluded NVARCHAR(MAX) NULL, -- JSON array
    
    -- Status and Tracking
    Status NVARCHAR(50) DEFAULT 'Initiated', -- Initiated, InProgress, Completed, Failed, Cancelled
    InitiatedAt DATETIME2 DEFAULT GETUTCDATE(),
    CompletedAt DATETIME2 NULL,
    
    -- Security
    IsEncrypted BIT DEFAULT 1,
    EncryptionMethod NVARCHAR(50) DEFAULT 'AES-256',
    TransferToken NVARCHAR(500) NULL,
    
    -- Audit
    InitiatedBy NVARCHAR(255) NOT NULL,
    ReceivedBy NVARCHAR(255) NULL,
    ReceivedAt DATETIME2 NULL,
    
    INDEX IX_RecordTransfers_PatientId (PatientId),
    INDEX IX_RecordTransfers_Status (Status),
    INDEX IX_RecordTransfers_InitiatedAt (InitiatedAt)
)
GO
