-- =============================================
-- Healthcare Platform - Insurance Service Database Schema
-- Manages insurance coverage, claims, and eligibility
-- Maps to FHIR Coverage and Claim resources
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'insurance')
BEGIN
    EXEC('CREATE SCHEMA insurance')
END
GO

-- =============================================
-- Table: InsurancePolicies
-- Purpose: Insurance policies and coverage information
-- Maps to FHIR Coverage resource
-- =============================================
CREATE TABLE insurance.InsurancePolicies (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Policy Information
    PolicyNumber NVARCHAR(500) NOT NULL, -- Encrypted
    GroupNumber NVARCHAR(500) NULL, -- Encrypted
    SubscriberId NVARCHAR(500) NULL, -- Encrypted
    
    -- Insurance Company
    InsuranceCompanyName NVARCHAR(255) NOT NULL,
    InsuranceCompanyCode NVARCHAR(50) NULL, -- Payer ID
    CompanyPhone NVARCHAR(50) NULL,
    CompanyWebsite NVARCHAR(255) NULL,
    
    -- Policy Type
    PolicyType NVARCHAR(50) NOT NULL, -- Medical, Dental, Vision, Prescription
    CoverageType NVARCHAR(50) NOT NULL, -- Individual, Family, Employee, Dependent
    PlanName NVARCHAR(255) NULL,
    PlanNetwork NVARCHAR(100) NULL, -- HMO, PPO, EPO, POS
    
    -- Subscriber Information (if patient is dependent)
    IsSubscriber BIT DEFAULT 1,
    SubscriberPatientId UNIQUEIDENTIFIER NULL,
    RelationshipToSubscriber NVARCHAR(50) NULL, -- Self, Spouse, Child, Other
    
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
    Status NVARCHAR(50) DEFAULT 'active', -- active, cancelled, draft, entered-in-error
    IsPrimary BIT DEFAULT 1,
    Priority INT DEFAULT 1, -- 1=Primary, 2=Secondary, etc.
    
    -- Documents
    InsuranceCardFrontUrl NVARCHAR(1000) NULL,
    InsuranceCardBackUrl NVARCHAR(1000) NULL,
    PolicyDocumentUrl NVARCHAR(1000) NULL,
    
    -- Verification
    IsVerified BIT DEFAULT 0,
    VerifiedAt DATETIME2 NULL,
    VerifiedBy NVARCHAR(255) NULL,
    LastEligibilityCheck DATETIME2 NULL,
    
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
    
    INDEX IX_InsurancePolicies_PatientId (PatientId),
    INDEX IX_InsurancePolicies_Status (Status),
    INDEX IX_InsurancePolicies_IsPrimary (IsPrimary),
    INDEX IX_InsurancePolicies_IsDeleted (IsDeleted)
)
GO

-- =============================================
-- Table: Claims
-- Purpose: Insurance claims tracking
-- Maps to FHIR Claim resource
-- =============================================
CREATE TABLE insurance.Claims (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    InsurancePolicyId UNIQUEIDENTIFIER NOT NULL,
    
    -- Claim Information
    ClaimNumber NVARCHAR(100) NOT NULL,
    ClaimType NVARCHAR(50) NOT NULL, -- institutional, oral, pharmacy, professional, vision
    
    -- Dates
    ServiceDate DATETIME2 NOT NULL,
    ClaimDate DATETIME2 NOT NULL,
    ReceivedDate DATETIME2 NULL,
    ProcessedDate DATETIME2 NULL,
    
    -- Provider Information
    ProviderName NVARCHAR(255) NULL,
    ProviderNPI NVARCHAR(50) NULL,
    FacilityName NVARCHAR(255) NULL,
    FacilityNPI NVARCHAR(50) NULL,
    
    -- Financial Information
    TotalCharges DECIMAL(18,2) NOT NULL,
    AllowedAmount DECIMAL(18,2) NULL,
    PaidAmount DECIMAL(18,2) NULL,
    PatientResponsibility DECIMAL(18,2) NULL,
    Deductible DECIMAL(18,2) NULL,
    Coinsurance DECIMAL(18,2) NULL,
    Copay DECIMAL(18,2) NULL,
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'submitted', -- submitted, pending, processing, approved, denied, partially-approved
    DenialReason NVARCHAR(MAX) NULL,
    DenialCode NVARCHAR(50) NULL,
    
    -- Diagnosis and Services
    PrimaryDiagnosisCode NVARCHAR(50) NULL,
    PrimaryDiagnosisDescription NVARCHAR(500) NULL,
    ProcedureCodes NVARCHAR(MAX) NULL, -- JSON array of procedure codes
    
    -- EOB (Explanation of Benefits)
    EOBDocumentUrl NVARCHAR(1000) NULL,
    EOBReceivedDate DATETIME2 NULL,
    
    -- Appeals
    IsAppealed BIT DEFAULT 0,
    AppealDate DATETIME2 NULL,
    AppealStatus NVARCHAR(50) NULL,
    AppealOutcome NVARCHAR(MAX) NULL,
    
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
    
    CONSTRAINT FK_Claims_Policy FOREIGN KEY (InsurancePolicyId) 
        REFERENCES insurance.InsurancePolicies(Id),
    
    INDEX IX_Claims_PatientId (PatientId),
    INDEX IX_Claims_PolicyId (InsurancePolicyId),
    INDEX IX_Claims_ClaimNumber (ClaimNumber),
    INDEX IX_Claims_Status (Status),
    INDEX IX_Claims_ServiceDate (ServiceDate),
    INDEX IX_Claims_IsDeleted (IsDeleted)
)
GO

-- =============================================
-- Table: EligibilityVerifications
-- Purpose: Track insurance eligibility checks
-- =============================================
CREATE TABLE insurance.EligibilityVerifications (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    InsurancePolicyId UNIQUEIDENTIFIER NOT NULL,
    
    -- Verification Information
    VerificationDate DATETIME2 DEFAULT GETUTCDATE(),
    VerifiedBy NVARCHAR(255) NOT NULL,
    VerificationMethod NVARCHAR(50) NULL, -- API, Phone, Portal
    
    -- Results
    IsEligible BIT NOT NULL,
    EligibilityStatus NVARCHAR(50) NULL, -- active, inactive, unknown
    CoverageStatus NVARCHAR(MAX) NULL, -- JSON with detailed coverage
    
    -- Coverage Active Period
    CoverageStartDate DATE NULL,
    CoverageEndDate DATE NULL,
    
    -- Benefits
    BenefitsSummary NVARCHAR(MAX) NULL, -- JSON with benefit details
    
    -- Response Details
    PayerResponse NVARCHAR(MAX) NULL, -- Raw response from payer
    ResponseCode NVARCHAR(50) NULL,
    
    -- FHIR Compliance (maps to CoverageEligibilityResponse)
    FHIRResourceId NVARCHAR(100) NULL,
    FHIRResourceJson NVARCHAR(MAX) NULL,
    
    CONSTRAINT FK_EligibilityVerifications_Policy FOREIGN KEY (InsurancePolicyId) 
        REFERENCES insurance.InsurancePolicies(Id),
    
    INDEX IX_EligibilityVerifications_PatientId (PatientId),
    INDEX IX_EligibilityVerifications_PolicyId (InsurancePolicyId),
    INDEX IX_EligibilityVerifications_Date (VerificationDate)
)
GO

PRINT 'Insurance Service schema created successfully'
GO
