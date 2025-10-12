-- =============================================
-- Healthcare Platform - Data Integration Service Database Schema
-- FHIR/HL7 integration, EMR connectors, document ingestion
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'integration')
BEGIN
    EXEC('CREATE SCHEMA integration')
END
GO

-- =============================================
-- Table: FHIRResources
-- Purpose: Store FHIR resource cache for quick access
-- =============================================
CREATE TABLE integration.FHIRResources (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    
    -- Resource Information
    ResourceType NVARCHAR(50) NOT NULL, -- Patient, AllergyIntolerance, Condition, etc.
    ResourceId NVARCHAR(100) NOT NULL,
    PatientId UNIQUEIDENTIFIER NULL,
    
    -- FHIR Data
    FHIRVersion NVARCHAR(20) DEFAULT 'R4',
    ResourceJson NVARCHAR(MAX) NOT NULL,
    ResourceXml NVARCHAR(MAX) NULL,
    
    -- Metadata
    LastUpdated DATETIME2 DEFAULT GETUTCDATE(),
    VersionId INT DEFAULT 1,
    
    -- Source
    SourceSystem NVARCHAR(255) NULL,
    SourceId NVARCHAR(255) NULL,
    
    -- Status
    IsActive BIT DEFAULT 1,
    
    -- Base Entity Fields
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(255) NULL,
    
    INDEX IX_FHIRResources_ResourceType (ResourceType),
    INDEX IX_FHIRResources_ResourceId (ResourceId),
    INDEX IX_FHIRResources_PatientId (PatientId)
)
GO

-- =============================================
-- Table: DocumentIngestionQueue
-- Purpose: Queue for processing incoming documents from email/fax
-- =============================================
CREATE TABLE integration.DocumentIngestionQueue (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    
    -- Document Source
    SourceType NVARCHAR(50) NOT NULL, -- Email, Fax, Upload, API
    SourceIdentifier NVARCHAR(255) NULL,
    ReceivedAt DATETIME2 DEFAULT GETUTCDATE(),
    
    -- Document Information
    DocumentName NVARCHAR(255) NOT NULL,
    FileType NVARCHAR(50) NOT NULL,
    FileSizeBytes BIGINT NOT NULL,
    BlobStorageUrl NVARCHAR(1000) NOT NULL,
    
    -- Patient Matching
    PatientId UNIQUEIDENTIFIER NULL,
    MatchConfidence DECIMAL(5,2) NULL,
    IsMatched BIT DEFAULT 0,
    
    -- Processing Status
    ProcessingStatus NVARCHAR(50) DEFAULT 'Pending', -- Pending, Processing, Completed, Failed
    ProcessedAt DATETIME2 NULL,
    OCRCompleted BIT DEFAULT 0,
    OCRText NVARCHAR(MAX) NULL,
    
    -- Error Handling
    ErrorMessage NVARCHAR(MAX) NULL,
    RetryCount INT DEFAULT 0,
    
    INDEX IX_DocumentIngestionQueue_Status (ProcessingStatus),
    INDEX IX_DocumentIngestionQueue_ReceivedAt (ReceivedAt),
    INDEX IX_DocumentIngestionQueue_PatientId (PatientId)
)
GO

PRINT 'Data Integration Service schema created successfully'
GO
