-- =============================================
-- Healthcare Platform - Health History Service Database Schema
-- Manages medical history, conditions, allergies, medications, immunizations
-- All data stored in FHIR-compliant format
-- =============================================

IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'health')
BEGIN
    EXEC('CREATE SCHEMA health')
END
GO

-- =============================================
-- Table: Conditions
-- Purpose: Medical conditions, diagnoses, and chronic illnesses
-- Maps to FHIR Condition resource
-- =============================================
CREATE TABLE health.Conditions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Condition Information
    ConditionName NVARCHAR(255) NOT NULL,
    ConditionCode NVARCHAR(50) NULL, -- ICD-10, SNOMED CT, etc.
    CodingSystem NVARCHAR(100) NULL, -- ICD-10-CM, SNOMED-CT, etc.
    Category NVARCHAR(100) NULL, -- problem-list-item, encounter-diagnosis
    
    -- Clinical Status
    ClinicalStatus NVARCHAR(50) NOT NULL DEFAULT 'active', -- active, recurrence, relapse, inactive, remission, resolved
    VerificationStatus NVARCHAR(50) DEFAULT 'confirmed', -- unconfirmed, provisional, differential, confirmed, refuted, entered-in-error
    
    -- Severity
    Severity NVARCHAR(50) NULL, -- mild, moderate, severe
    
    -- Dates
    OnsetDate DATETIME2 NULL,
    AbatementDate DATETIME2 NULL,
    RecordedDate DATETIME2 DEFAULT GETUTCDATE(),
    
    -- Additional Information
    BodySite NVARCHAR(255) NULL,
    Notes NVARCHAR(MAX) NULL,
    
    -- Source
    RecordedBy NVARCHAR(255) NULL,
    DiagnosedBy NVARCHAR(255) NULL,
    SourceType NVARCHAR(50) DEFAULT 'patient-reported', -- patient-reported, provider-entered, imported
    
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
    
    INDEX IX_Conditions_PatientId (PatientId),
    INDEX IX_Conditions_ClinicalStatus (ClinicalStatus),
    INDEX IX_Conditions_Code (ConditionCode),
    INDEX IX_Conditions_IsDeleted (IsDeleted)
)
GO

-- =============================================
-- Table: Allergies
-- Purpose: Allergies and intolerances
-- Maps to FHIR AllergyIntolerance resource
-- =============================================
CREATE TABLE health.Allergies (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Allergy Information
    AllergenName NVARCHAR(255) NOT NULL,
    AllergenCode NVARCHAR(50) NULL, -- RxNorm, SNOMED CT, etc.
    CodingSystem NVARCHAR(100) NULL,
    
    -- Type and Category
    AllergyType NVARCHAR(50) NOT NULL, -- allergy, intolerance
    Category NVARCHAR(50) NOT NULL, -- food, medication, environment, biologic
    
    -- Severity and Criticality
    Criticality NVARCHAR(50) NULL, -- low, high, unable-to-assess
    Severity NVARCHAR(50) NULL, -- mild, moderate, severe
    
    -- Clinical Status
    ClinicalStatus NVARCHAR(50) DEFAULT 'active', -- active, inactive, resolved
    VerificationStatus NVARCHAR(50) DEFAULT 'confirmed', -- unconfirmed, confirmed, refuted, entered-in-error
    
    -- Reaction Information
    ReactionDescription NVARCHAR(MAX) NULL,
    ReactionManifestations NVARCHAR(MAX) NULL, -- JSON array of manifestations
    ReactionOnset DATETIME2 NULL,
    
    -- Dates
    OnsetDate DATETIME2 NULL,
    RecordedDate DATETIME2 DEFAULT GETUTCDATE(),
    LastOccurrence DATETIME2 NULL,
    
    -- Source
    RecordedBy NVARCHAR(255) NULL,
    SourceType NVARCHAR(50) DEFAULT 'patient-reported',
    
    -- Additional Information
    Notes NVARCHAR(MAX) NULL,
    
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
    
    INDEX IX_Allergies_PatientId (PatientId),
    INDEX IX_Allergies_Category (Category),
    INDEX IX_Allergies_Criticality (Criticality),
    INDEX IX_Allergies_IsDeleted (IsDeleted)
)
GO

-- =============================================
-- Table: Medications
-- Purpose: Current and historical medications
-- Maps to FHIR MedicationStatement resource
-- =============================================
CREATE TABLE health.Medications (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Medication Information
    MedicationName NVARCHAR(255) NOT NULL,
    MedicationCode NVARCHAR(50) NULL, -- RxNorm code
    GenericName NVARCHAR(255) NULL,
    BrandName NVARCHAR(255) NULL,
    
    -- Dosage
    Dosage NVARCHAR(100) NULL, -- e.g., "500mg"
    DosageForm NVARCHAR(100) NULL, -- tablet, capsule, liquid, etc.
    Route NVARCHAR(100) NULL, -- oral, injection, topical, etc.
    Frequency NVARCHAR(100) NULL, -- e.g., "twice daily", "as needed"
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'active', -- active, completed, entered-in-error, intended, stopped, on-hold
    
    -- Dates
    StartDate DATETIME2 NULL,
    EndDate DATETIME2 NULL,
    RecordedDate DATETIME2 DEFAULT GETUTCDATE(),
    
    -- Prescriber Information
    PrescribedBy NVARCHAR(255) NULL,
    PrescribedDate DATETIME2 NULL,
    Pharmacy NVARCHAR(255) NULL,
    
    -- Reason
    ReasonCode NVARCHAR(50) NULL,
    ReasonDescription NVARCHAR(500) NULL,
    
    -- Additional Information
    Instructions NVARCHAR(MAX) NULL,
    Notes NVARCHAR(MAX) NULL,
    IsOverTheCounter BIT DEFAULT 0,
    
    -- Source
    SourceType NVARCHAR(50) DEFAULT 'patient-reported',
    
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
    
    INDEX IX_Medications_PatientId (PatientId),
    INDEX IX_Medications_Status (Status),
    INDEX IX_Medications_StartDate (StartDate),
    INDEX IX_Medications_IsDeleted (IsDeleted)
)
GO

-- =============================================
-- Table: Immunizations
-- Purpose: Vaccination records
-- Maps to FHIR Immunization resource
-- =============================================
CREATE TABLE health.Immunizations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Vaccine Information
    VaccineName NVARCHAR(255) NOT NULL,
    VaccineCode NVARCHAR(50) NULL, -- CVX code
    LotNumber NVARCHAR(100) NULL,
    Manufacturer NVARCHAR(255) NULL,
    
    -- Administration
    AdministrationDate DATETIME2 NOT NULL,
    ExpirationDate DATETIME2 NULL,
    DoseNumber INT NULL,
    SeriesCount INT NULL, -- Total doses in series
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'completed', -- completed, entered-in-error, not-done
    
    -- Site and Route
    BodySite NVARCHAR(100) NULL, -- left arm, right arm, etc.
    Route NVARCHAR(100) NULL, -- intramuscular, oral, etc.
    DoseQuantity NVARCHAR(50) NULL,
    
    -- Provider Information
    AdministeredBy NVARCHAR(255) NULL,
    AdministeredAt NVARCHAR(255) NULL, -- Facility/location
    
    -- Reaction/Notes
    ReactionDescription NVARCHAR(MAX) NULL,
    Notes NVARCHAR(MAX) NULL,
    
    -- Source
    SourceType NVARCHAR(50) DEFAULT 'provider-entered',
    RecordedDate DATETIME2 DEFAULT GETUTCDATE(),
    
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
    
    INDEX IX_Immunizations_PatientId (PatientId),
    INDEX IX_Immunizations_AdministrationDate (AdministrationDate),
    INDEX IX_Immunizations_VaccineCode (VaccineCode),
    INDEX IX_Immunizations_IsDeleted (IsDeleted)
)
GO

-- =============================================
-- Table: Procedures
-- Purpose: Surgical procedures and medical procedures
-- Maps to FHIR Procedure resource
-- =============================================
CREATE TABLE health.Procedures (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Procedure Information
    ProcedureName NVARCHAR(255) NOT NULL,
    ProcedureCode NVARCHAR(50) NULL, -- CPT, ICD-10-PCS, SNOMED CT
    CodingSystem NVARCHAR(100) NULL,
    Category NVARCHAR(100) NULL,
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'completed', -- preparation, in-progress, not-done, on-hold, stopped, completed, entered-in-error, unknown
    
    -- Dates
    PerformedDate DATETIME2 NULL,
    PerformedStartDate DATETIME2 NULL,
    PerformedEndDate DATETIME2 NULL,
    RecordedDate DATETIME2 DEFAULT GETUTCDATE(),
    
    -- Provider Information
    PerformedBy NVARCHAR(255) NULL,
    PerformedAt NVARCHAR(255) NULL, -- Facility
    
    -- Clinical Information
    ReasonCode NVARCHAR(50) NULL,
    ReasonDescription NVARCHAR(500) NULL,
    BodySite NVARCHAR(255) NULL,
    Outcome NVARCHAR(500) NULL,
    
    -- Additional Information
    Notes NVARCHAR(MAX) NULL,
    Complications NVARCHAR(MAX) NULL,
    
    -- Source
    SourceType NVARCHAR(50) DEFAULT 'provider-entered',
    
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
    
    INDEX IX_Procedures_PatientId (PatientId),
    INDEX IX_Procedures_PerformedDate (PerformedDate),
    INDEX IX_Procedures_Status (Status),
    INDEX IX_Procedures_IsDeleted (IsDeleted)
)
GO

-- =============================================
-- Table: Observations
-- Purpose: Lab results, vital signs, and other observations
-- Maps to FHIR Observation resource
-- =============================================
CREATE TABLE health.Observations (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    
    -- Observation Information
    ObservationType NVARCHAR(255) NOT NULL, -- vital-signs, laboratory, imaging, etc.
    ObservationName NVARCHAR(255) NOT NULL,
    ObservationCode NVARCHAR(50) NULL, -- LOINC code
    CodingSystem NVARCHAR(100) NULL,
    
    -- Value
    ValueType NVARCHAR(50) NOT NULL, -- Quantity, CodeableConcept, String, Boolean, Integer, Range, Ratio
    ValueQuantity DECIMAL(18,4) NULL,
    ValueUnit NVARCHAR(50) NULL,
    ValueString NVARCHAR(MAX) NULL,
    ValueCode NVARCHAR(50) NULL,
    
    -- Reference Range
    ReferenceRangeLow DECIMAL(18,4) NULL,
    ReferenceRangeHigh DECIMAL(18,4) NULL,
    ReferenceRangeText NVARCHAR(500) NULL,
    
    -- Interpretation
    Interpretation NVARCHAR(50) NULL, -- normal, abnormal, critical, etc.
    InterpretationText NVARCHAR(500) NULL,
    
    -- Status
    Status NVARCHAR(50) DEFAULT 'final', -- registered, preliminary, final, amended, corrected, cancelled, entered-in-error
    
    -- Dates
    EffectiveDate DATETIME2 NOT NULL,
    IssuedDate DATETIME2 DEFAULT GETUTCDATE(),
    
    -- Performer Information
    PerformedBy NVARCHAR(255) NULL,
    PerformedAt NVARCHAR(255) NULL,
    
    -- Method and Body Site
    Method NVARCHAR(255) NULL,
    BodySite NVARCHAR(255) NULL,
    
    -- Additional Information
    Notes NVARCHAR(MAX) NULL,
    
    -- Source
    SourceType NVARCHAR(50) DEFAULT 'provider-entered',
    
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
    
    INDEX IX_Observations_PatientId (PatientId),
    INDEX IX_Observations_Type (ObservationType),
    INDEX IX_Observations_EffectiveDate (EffectiveDate),
    INDEX IX_Observations_IsDeleted (IsDeleted)
)
GO

PRINT 'Health History Service schema created successfully'
GO
