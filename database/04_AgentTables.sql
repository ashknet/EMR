-- Hospital Agent Service Tables (ag schema)

USE HealthcarePlatform;
GO

-- Hospital Agents Table
CREATE TABLE ag.HospitalAgents (
    AgentId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    MachineName NVARCHAR(200) NOT NULL,
    HospitalName NVARCHAR(300) NOT NULL,
    HospitalNpi NVARCHAR(20),
    Department NVARCHAR(200),
    Version NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) DEFAULT 'Active',
    RegisteredAt DATETIME2 DEFAULT GETUTCDATE(),
    LastHeartbeat DATETIME2,
    IpAddress NVARCHAR(50),
    IsActive BIT DEFAULT 1,
    INDEX IX_MachineName (MachineName),
    INDEX IX_HospitalName (HospitalName)
);
GO

-- Agent Sessions Table
CREATE TABLE ag.AgentSessions (
    SessionId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AgentId UNIQUEIDENTIFIER NOT NULL,
    UserName NVARCHAR(200) NOT NULL,
    StartTime DATETIME2 DEFAULT GETUTCDATE(),
    EndTime DATETIME2,
    IpAddress NVARCHAR(50),
    FOREIGN KEY (AgentId) REFERENCES ag.HospitalAgents(AgentId),
    INDEX IX_AgentId (AgentId)
);
GO

-- Intake Requests Table
CREATE TABLE ag.IntakeRequests (
    RequestId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AgentId UNIQUEIDENTIFIER NOT NULL,
    RequestType NVARCHAR(50) NOT NULL, -- QR, Manual, Email, Fax
    QrToken NVARCHAR(200),
    PatientData NVARCHAR(MAX), -- JSON
    Status NVARCHAR(50) DEFAULT 'Pending',
    RequestedAt DATETIME2 DEFAULT GETUTCDATE(),
    ProcessedAt DATETIME2,
    ProcessedBy NVARCHAR(200),
    FOREIGN KEY (AgentId) REFERENCES ag.HospitalAgents(AgentId),
    INDEX IX_AgentId (AgentId),
    INDEX IX_Status (Status)
);
GO

-- Transfer Logs Table
CREATE TABLE ag.TransferLogs (
    LogId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AgentId UNIQUEIDENTIFIER NOT NULL,
    TransferType NVARCHAR(50) NOT NULL, -- Inbound, Outbound
    TrackingNumber NVARCHAR(50),
    Status NVARCHAR(50) NOT NULL,
    LogTime DATETIME2 DEFAULT GETUTCDATE(),
    Details NVARCHAR(MAX),
    FOREIGN KEY (AgentId) REFERENCES ag.HospitalAgents(AgentId),
    INDEX IX_AgentId (AgentId),
    INDEX IX_TrackingNumber (TrackingNumber)
);
GO

PRINT 'Agent Service tables created successfully';

