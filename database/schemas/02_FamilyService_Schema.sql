-- =============================================
-- Healthcare Platform - Family Service Database Schema
-- Manages family relationships, proxies, and guardianship
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'family')
BEGIN
    EXEC('CREATE SCHEMA family')
END
GO

-- =============================================
-- Table: FamilyGroups
-- Purpose: Represent family units/households
-- =============================================
CREATE TABLE family.FamilyGroups (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FamilyName NVARCHAR(200) NOT NULL,
    PrimaryAccountHolderId UNIQUEIDENTIFIER NOT NULL,
    
    -- Contact Information
    PrimaryEmail NVARCHAR(255) NULL,
    PrimaryPhone NVARCHAR(50) NULL,
    
    -- Address (inherited by all members by default)
    AddressLine1 NVARCHAR(200) NULL,
    AddressLine2 NVARCHAR(200) NULL,
    City NVARCHAR(100) NULL,
    State NVARCHAR(50) NULL,
    ZipCode NVARCHAR(20) NULL,
    Country NVARCHAR(50) DEFAULT 'USA',
    
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
    RowVersion ROWVERSION,
    
    INDEX IX_FamilyGroups_PrimaryAccountHolder (PrimaryAccountHolderId),
    INDEX IX_FamilyGroups_IsActive (IsActive),
    INDEX IX_FamilyGroups_IsDeleted (IsDeleted)
)
GO

-- =============================================
-- Table: FamilyMembers
-- Purpose: Link patients to family groups with relationships
-- =============================================
CREATE TABLE family.FamilyMembers (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FamilyGroupId UNIQUEIDENTIFIER NOT NULL,
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Relationship Information
    RelationshipType NVARCHAR(50) NOT NULL, -- Self, Spouse, Child, Parent, Sibling, Guardian, Other
    RelationshipToHead NVARCHAR(100) NULL, -- More specific relationship description
    
    -- Role and Permissions
    IsHead BIT DEFAULT 0,
    CanManageFamilyData BIT DEFAULT 0,
    CanViewAllRecords BIT DEFAULT 0,
    CanShareOnBehalf BIT DEFAULT 0,
    
    -- Status
    IsActive BIT DEFAULT 1,
    JoinedDate DATETIME2 DEFAULT GETUTCDATE(),
    LeftDate DATETIME2 NULL,
    
    -- Base Entity Fields
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(255) NULL,
    RowVersion ROWVERSION,
    
    CONSTRAINT FK_FamilyMembers_FamilyGroup FOREIGN KEY (FamilyGroupId) 
        REFERENCES family.FamilyGroups(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_FamilyMembers_PatientFamily UNIQUE (FamilyGroupId, PatientId),
    
    INDEX IX_FamilyMembers_FamilyGroupId (FamilyGroupId),
    INDEX IX_FamilyMembers_PatientId (PatientId),
    INDEX IX_FamilyMembers_RelationshipType (RelationshipType),
    INDEX IX_FamilyMembers_IsDeleted (IsDeleted)
)
GO

-- =============================================
-- Table: ProxyAuthorizations
-- Purpose: Manage proxy/guardian authorizations for minors or incapacitated patients
-- =============================================
CREATE TABLE family.ProxyAuthorizations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    
    -- Authorization Details
    PatientId UNIQUEIDENTIFIER NOT NULL, -- Person being represented
    ProxyPatientId UNIQUEIDENTIFIER NOT NULL, -- Person authorized to act
    AuthorizationType NVARCHAR(50) NOT NULL, -- Guardian, PowerOfAttorney, MedicalProxy, Temporary
    
    -- Scope of Authorization
    CanViewMedicalRecords BIT DEFAULT 0,
    CanUpdateDemographics BIT DEFAULT 0,
    CanShareRecords BIT DEFAULT 0,
    CanMakeMedicalDecisions BIT DEFAULT 0,
    CanAccessInsurance BIT DEFAULT 0,
    CanManageConsents BIT DEFAULT 0,
    
    -- Authorization Period
    StartDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    EndDate DATETIME2 NULL,
    IsIndefinite BIT DEFAULT 0,
    
    -- Legal Documentation
    LegalDocumentUrl NVARCHAR(1000) NULL, -- Court order, POA document, etc.
    DocumentType NVARCHAR(100) NULL,
    IssuedBy NVARCHAR(255) NULL,
    IssuedDate DATETIME2 NULL,
    
    -- Status
    IsActive BIT DEFAULT 1,
    Status NVARCHAR(50) DEFAULT 'Active', -- Active, Expired, Revoked, Pending
    RevokedAt DATETIME2 NULL,
    RevokedBy NVARCHAR(255) NULL,
    RevocationReason NVARCHAR(500) NULL,
    
    -- Base Entity Fields
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(255) NOT NULL,
    UpdatedBy NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(255) NULL,
    RowVersion ROWVERSION,
    
    INDEX IX_ProxyAuth_PatientId (PatientId),
    INDEX IX_ProxyAuth_ProxyPatientId (ProxyPatientId),
    INDEX IX_ProxyAuth_Status (Status),
    INDEX IX_ProxyAuth_IsActive (IsActive),
    INDEX IX_ProxyAuth_IsDeleted (IsDeleted)
)
GO

-- =============================================
-- Table: EmergencyContacts
-- Purpose: Store emergency contacts for family members
-- =============================================
CREATE TABLE family.EmergencyContacts (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Contact Information (Encrypted)
    ContactName NVARCHAR(500) NOT NULL, -- Encrypted
    Relationship NVARCHAR(100) NULL,
    PhoneNumber NVARCHAR(500) NOT NULL, -- Encrypted
    AlternatePhone NVARCHAR(500) NULL, -- Encrypted
    Email NVARCHAR(500) NULL, -- Encrypted
    
    -- Address
    AddressLine1 NVARCHAR(500) NULL, -- Encrypted
    AddressLine2 NVARCHAR(500) NULL, -- Encrypted
    City NVARCHAR(200) NULL,
    State NVARCHAR(50) NULL,
    ZipCode NVARCHAR(20) NULL,
    
    -- Priority
    ContactPriority INT DEFAULT 1, -- 1=Primary, 2=Secondary, etc.
    
    -- Permissions
    CanReceiveMedicalInfo BIT DEFAULT 1,
    CanMakeDecisions BIT DEFAULT 0,
    
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
    RowVersion ROWVERSION,
    
    INDEX IX_EmergencyContacts_PatientId (PatientId),
    INDEX IX_EmergencyContacts_Priority (ContactPriority),
    INDEX IX_EmergencyContacts_IsDeleted (IsDeleted)
)
GO

-- =============================================
-- Table: FamilyAccessLog
-- Purpose: Audit log for family-related access and changes
-- =============================================
CREATE TABLE family.FamilyAccessLog (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    FamilyGroupId UNIQUEIDENTIFIER NULL,
    PatientId UNIQUEIDENTIFIER NULL,
    
    -- Access Information
    ActionType NVARCHAR(100) NOT NULL, -- AddMember, RemoveMember, GrantProxy, RevokeProxy, ViewFamily, etc.
    PerformedBy NVARCHAR(255) NOT NULL,
    PerformedByRole NVARCHAR(100) NULL,
    PerformedAt DATETIME2 DEFAULT GETUTCDATE(),
    
    -- Details
    Details NVARCHAR(MAX) NULL, -- JSON with action details
    IPAddress NVARCHAR(50) NULL,
    
    INDEX IX_FamilyAccessLog_FamilyGroupId (FamilyGroupId),
    INDEX IX_FamilyAccessLog_PatientId (PatientId),
    INDEX IX_FamilyAccessLog_PerformedAt (PerformedAt)
)
GO

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Family groups representing households with shared management',
    @level0type = N'SCHEMA', @level0name = 'family',
    @level1type = N'TABLE', @level1name = 'FamilyGroups'
GO

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Links patients to family groups with relationship information',
    @level0type = N'SCHEMA', @level0name = 'family',
    @level1type = N'TABLE', @level1name = 'FamilyMembers'
GO

EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Proxy and guardian authorizations for managing patient data',
    @level0type = N'SCHEMA', @level0name = 'family',
    @level1type = N'TABLE', @level1name = 'ProxyAuthorizations'
GO
