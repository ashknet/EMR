-- =============================================
-- Healthcare Platform - Security/Compliance Service Database Schema
-- Policy enforcement, breach detection, compliance monitoring
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'security')
BEGIN
    EXEC('CREATE SCHEMA security')
END
GO

CREATE TABLE security.ComplianceReports (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    
    -- Report Information
    ReportType NVARCHAR(100) NOT NULL, -- HIPAA, HITRUST, SOC2, Monthly, Annual
    ReportPeriodStart DATE NOT NULL,
    ReportPeriodEnd DATE NOT NULL,
    GeneratedAt DATETIME2 DEFAULT GETUTCDATE(),
    GeneratedBy NVARCHAR(255) NOT NULL,
    
    -- Findings
    TotalIssues INT DEFAULT 0,
    CriticalIssues INT DEFAULT 0,
    MediumIssues INT DEFAULT 0,
    LowIssues INT DEFAULT 0,
    
    -- Status
    ComplianceScore DECIMAL(5,2) NULL,
    Status NVARCHAR(50) DEFAULT 'Draft', -- Draft, Final, Submitted
    
    -- Report Data
    ReportDataJson NVARCHAR(MAX) NULL,
    ReportDocumentUrl NVARCHAR(1000) NULL,
    
    INDEX IX_ComplianceReports_Type (ReportType),
    INDEX IX_ComplianceReports_GeneratedAt (GeneratedAt)
)
GO

CREATE TABLE security.SecurityIncidents (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    
    -- Incident Information
    IncidentType NVARCHAR(100) NOT NULL, -- UnauthorizedAccess, DataBreach, SystemAnomaly
    Severity NVARCHAR(20) NOT NULL, -- Low, Medium, High, Critical
    Description NVARCHAR(MAX) NOT NULL,
    
    -- Detection
    DetectedAt DATETIME2 DEFAULT GETUTCDATE(),
    DetectedBy NVARCHAR(255) NULL,
    DetectionMethod NVARCHAR(100) NULL,
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'Open', -- Open, InvestigationBegun, Resolved, False Positive
    ResolvedAt DATETIME2 NULL,
    ResolvedBy NVARCHAR(255) NULL,
    
    -- Impact
    AffectedPatients INT DEFAULT 0,
    AffectedRecords INT DEFAULT 0,
    
    -- Response
    ResponseActions NVARCHAR(MAX) NULL,
    
    INDEX IX_SecurityIncidents_Severity (Severity),
    INDEX IX_SecurityIncidents_Status (Status),
    INDEX IX_SecurityIncidents_DetectedAt (DetectedAt)
)
GO
