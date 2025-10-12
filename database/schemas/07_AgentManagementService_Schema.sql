-- =============================================
-- Healthcare Platform - Agent Management Service Database Schema
-- Hospital agent deployment, updates, and communication
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'agent')
BEGIN
    EXEC('CREATE SCHEMA agent')
END
GO

CREATE TABLE agent.HospitalAgents (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    
    -- Hospital Information
    HospitalName NVARCHAR(255) NOT NULL,
    HospitalNPI NVARCHAR(50) NULL,
    HospitalAddress NVARCHAR(500) NULL,
    
    -- Agent Information
    AgentVersion NVARCHAR(50) NOT NULL,
    InstallationDate DATETIME2 DEFAULT GETUTCDATE(),
    LastUpdateDate DATETIME2 NULL,
    
    -- Device Information
    DeviceId NVARCHAR(255) NOT NULL UNIQUE,
    MachineName NVARCHAR(255) NULL,
    IPAddress NVARCHAR(50) NULL,
    WindowsVersion NVARCHAR(100) NULL,
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'Active', -- Active, Inactive, Suspended, Offline
    IsOnline BIT DEFAULT 0,
    LastHeartbeat DATETIME2 NULL,
    LastActivityAt DATETIME2 NULL,
    
    -- Security
    ApiKey NVARCHAR(500) NULL, -- Encrypted
    CertificateThumbprint NVARCHAR(100) NULL,
    
    -- Statistics
    TotalScansProcessed INT DEFAULT 0,
    TotalDocumentsIngested INT DEFAULT 0,
    TotalTransfersCompleted INT DEFAULT 0,
    
    -- Base Entity Fields
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    
    INDEX IX_HospitalAgents_DeviceId (DeviceId),
    INDEX IX_HospitalAgents_Status (Status),
    INDEX IX_HospitalAgents_IsOnline (IsOnline)
)
GO

CREATE TABLE agent.AgentActivityLog (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AgentId UNIQUEIDENTIFIER NOT NULL,
    
    -- Activity Information
    ActivityType NVARCHAR(100) NOT NULL, -- Scan, Upload, Transfer, Heartbeat, Error
    ActivityDescription NVARCHAR(MAX) NULL,
    ActivityTimestamp DATETIME2 DEFAULT GETUTCDATE(),
    
    -- Status
    Status NVARCHAR(50) NOT NULL, -- Success, Failure, Warning
    ErrorMessage NVARCHAR(MAX) NULL,
    
    CONSTRAINT FK_AgentActivityLog_Agent FOREIGN KEY (AgentId) 
        REFERENCES agent.HospitalAgents(Id),
    
    INDEX IX_AgentActivityLog_AgentId (AgentId),
    INDEX IX_AgentActivityLog_Timestamp (ActivityTimestamp)
)
GO
