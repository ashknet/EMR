/*
  EMR Master SQL Script
  - SQL Server compatible
  - Database-first canonical schema, constraints, indexes, audit columns, outbox, and seed data
  - Representative stored procedures included (non-exhaustive)
*/

SET NOCOUNT ON;
GO

/* Database (optional) — assume context set by deployment */
-- CREATE DATABASE EMR;
-- GO
-- USE EMR;
-- GO

/* Schemas */
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'core') EXEC('CREATE SCHEMA core');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'patient') EXEC('CREATE SCHEMA patient');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'hospital') EXEC('CREATE SCHEMA hospital');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'lab') EXEC('CREATE SCHEMA lab');
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'integration') EXEC('CREATE SCHEMA integration');
GO

/* Utility: default constraints helpers */
IF OBJECT_ID('core.GetUtcDateTime', 'FN') IS NOT NULL DROP FUNCTION core.GetUtcDateTime;
GO
CREATE FUNCTION core.GetUtcDateTime()
RETURNS DATETIME2(7)
AS
BEGIN
    RETURN SYSUTCDATETIME();
END;
GO

/* Core lookup tables */
IF OBJECT_ID('core.Specialties', 'U') IS NOT NULL DROP TABLE core.Specialties;
GO
CREATE TABLE core.Specialties (
    SpecialtyId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Specialties_Id DEFAULT NEWID(),
    Code NVARCHAR(50) NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_Specialties_IsActive DEFAULT(1),
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_Specialties_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME2(7) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    CONSTRAINT PK_Specialties PRIMARY KEY CLUSTERED (SpecialtyId),
    CONSTRAINT UQ_Specialties_Code UNIQUE (Code)
);
GO

IF OBJECT_ID('core.TestCodes', 'U') IS NOT NULL DROP TABLE core.TestCodes;
GO
CREATE TABLE core.TestCodes (
    TestCodeId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_TestCodes_Id DEFAULT NEWID(),
    Code NVARCHAR(50) NOT NULL, -- e.g., LOINC
    Name NVARCHAR(200) NOT NULL,
    Unit NVARCHAR(50) NULL,
    ReferenceRange NVARCHAR(200) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_TestCodes_IsActive DEFAULT(1),
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_TestCodes_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME2(7) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    CONSTRAINT PK_TestCodes PRIMARY KEY CLUSTERED (TestCodeId),
    CONSTRAINT UQ_TestCodes_Code UNIQUE (Code)
);
GO

/* File metadata (store paths only) */
IF OBJECT_ID('core.Files', 'U') IS NOT NULL DROP TABLE core.Files;
GO
CREATE TABLE core.Files (
    FileId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Files_Id DEFAULT NEWID(),
    OwnerType NVARCHAR(50) NOT NULL, -- Patient, Hospital, Lab, Encounter, etc.
    OwnerId UNIQUEIDENTIFIER NOT NULL,
    FileName NVARCHAR(255) NOT NULL,
    ContentType NVARCHAR(100) NOT NULL,
    BlobPath NVARCHAR(1000) NOT NULL,
    SizeBytes BIGINT NOT NULL,
    Hash NVARCHAR(128) NULL,
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_Files_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_Files_IsDeleted DEFAULT(0),
    CONSTRAINT PK_Files PRIMARY KEY CLUSTERED (FileId),
    INDEX IX_Files_Owner NONCLUSTERED (OwnerType, OwnerId) WHERE IsDeleted = 0
);
GO

/* Outbox for integration */
IF OBJECT_ID('core.OutboxMessages', 'U') IS NOT NULL DROP TABLE core.OutboxMessages;
GO
CREATE TABLE core.OutboxMessages (
    OutboxMessageId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_OutboxMessages_Id DEFAULT NEWID(),
    MessageId UNIQUEIDENTIFIER NOT NULL,
    CorrelationId UNIQUEIDENTIFIER NULL,
    Domain NVARCHAR(50) NOT NULL,
    EventType NVARCHAR(100) NOT NULL,
    Payload NVARCHAR(MAX) NOT NULL,
    OccurredAt DATETIME2(7) NOT NULL CONSTRAINT DF_OutboxMessages_OccurredAt DEFAULT SYSUTCDATETIME(),
    RetryCount INT NOT NULL CONSTRAINT DF_OutboxMessages_Retry DEFAULT(0),
    PublishedAt DATETIME2(7) NULL,
    Error NVARCHAR(1000) NULL,
    IdempotencyKey NVARCHAR(100) NULL,
    CONSTRAINT PK_OutboxMessages PRIMARY KEY CLUSTERED(OutboxMessageId),
    CONSTRAINT UQ_Outbox_MessageId UNIQUE (MessageId),
    INDEX IX_Outbox_Unpublished NONCLUSTERED (PublishedAt, RetryCount) WHERE PublishedAt IS NULL
);
GO

/* Patient domain */
IF OBJECT_ID('patient.Patients', 'U') IS NOT NULL DROP TABLE patient.Patients;
GO
CREATE TABLE patient.Patients (
    PatientId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Patients_Id DEFAULT NEWID(),
    ExternalId NVARCHAR(100) NULL,
    FirstName NVARCHAR(100) NOT NULL,
    MiddleName NVARCHAR(100) NULL,
    LastName NVARCHAR(100) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Gender NVARCHAR(50) NULL,
    MaritalStatus NVARCHAR(50) NULL,
    Ethnicity NVARCHAR(100) NULL,
    PreferredLanguage NVARCHAR(50) NULL,
    Phone NVARCHAR(50) NULL,
    Email NVARCHAR(200) NULL,
    AddressLine1 NVARCHAR(200) NULL,
    AddressLine2 NVARCHAR(200) NULL,
    City NVARCHAR(100) NULL,
    State NVARCHAR(100) NULL,
    PostalCode NVARCHAR(50) NULL,
    Country NVARCHAR(100) NULL,
    CommunicationPreferences NVARCHAR(200) NULL,
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_Patients_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME2(7) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_Patients_IsDeleted DEFAULT(0),
    CONSTRAINT PK_Patients PRIMARY KEY CLUSTERED (PatientId)
);
GO

IF OBJECT_ID('patient.PatientInsurance', 'U') IS NOT NULL DROP TABLE patient.PatientInsurance;
GO
CREATE TABLE patient.PatientInsurance (
    PatientInsuranceId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_PatientInsurance_Id DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    Provider NVARCHAR(200) NOT NULL,
    PolicyNumber NVARCHAR(100) NOT NULL,
    GroupNumber NVARCHAR(100) NULL,
    CoverageDetails NVARCHAR(1000) NULL,
    EffectiveFrom DATE NOT NULL,
    EffectiveTo DATE NULL,
    IsPrimary BIT NOT NULL CONSTRAINT DF_PatientInsurance_IsPrimary DEFAULT(0),
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_PatientInsurance_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME2(7) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_PatientInsurance_IsDeleted DEFAULT(0),
    CONSTRAINT PK_PatientInsurance PRIMARY KEY CLUSTERED (PatientInsuranceId),
    CONSTRAINT FK_PatientInsurance_Patient FOREIGN KEY (PatientId) REFERENCES patient.Patients(PatientId),
    INDEX IX_PatientInsurance_Patient NONCLUSTERED (PatientId) WHERE IsDeleted = 0
);
GO

IF OBJECT_ID('patient.PatientAllergies', 'U') IS NOT NULL DROP TABLE patient.PatientAllergies;
GO
CREATE TABLE patient.PatientAllergies (
    PatientAllergyId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_PatientAllergies_Id DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    Substance NVARCHAR(200) NOT NULL,
    Reaction NVARCHAR(500) NULL,
    Severity NVARCHAR(50) NULL,
    NotedDate DATE NULL,
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_PatientAllergies_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME2(7) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_PatientAllergies_IsDeleted DEFAULT(0),
    CONSTRAINT PK_PatientAllergies PRIMARY KEY CLUSTERED (PatientAllergyId),
    CONSTRAINT FK_PatientAllergies_Patient FOREIGN KEY (PatientId) REFERENCES patient.Patients(PatientId),
    INDEX IX_PatientAllergies_Patient NONCLUSTERED (PatientId) WHERE IsDeleted = 0
);
GO

IF OBJECT_ID('patient.Appointments', 'U') IS NOT NULL DROP TABLE patient.Appointments;
GO
CREATE TABLE patient.Appointments (
    AppointmentId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Appointments_Id DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    ScheduledAt DATETIME2(7) NOT NULL,
    Status NVARCHAR(50) NOT NULL, -- Requested, Confirmed, Cancelled, Completed
    Location NVARCHAR(200) NULL,
    TelehealthLink NVARCHAR(500) NULL,
    Notes NVARCHAR(1000) NULL,
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_Appointments_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME2(7) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_Appointments_IsDeleted DEFAULT(0),
    CONSTRAINT PK_Appointments PRIMARY KEY CLUSTERED (AppointmentId),
    CONSTRAINT FK_Appointments_Patient FOREIGN KEY (PatientId) REFERENCES patient.Patients(PatientId),
    INDEX IX_Appointments_Patient NONCLUSTERED (PatientId) WHERE IsDeleted = 0,
    INDEX IX_Appointments_ScheduledAt NONCLUSTERED (ScheduledAt)
);
GO

IF OBJECT_ID('patient.Consents', 'U') IS NOT NULL DROP TABLE patient.Consents;
GO
CREATE TABLE patient.Consents (
    ConsentId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Consents_Id DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    Scope NVARCHAR(100) NOT NULL, -- demographics, labs, treatment, all
    GrantedTo NVARCHAR(100) NOT NULL, -- Hospital, Lab, Integration, SpecificOrg
    GrantedToId UNIQUEIDENTIFIER NULL, -- Optional org id
    EffectiveFrom DATETIME2(7) NOT NULL,
    EffectiveTo DATETIME2(7) NULL,
    IsRevoked BIT NOT NULL CONSTRAINT DF_Consents_IsRevoked DEFAULT(0),
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_Consents_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME2(7) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_Consents_IsDeleted DEFAULT(0),
    CONSTRAINT PK_Consents PRIMARY KEY CLUSTERED (ConsentId),
    CONSTRAINT FK_Consents_Patient FOREIGN KEY (PatientId) REFERENCES patient.Patients(PatientId),
    INDEX IX_Consents_Patient NONCLUSTERED (PatientId) WHERE IsDeleted = 0,
    INDEX IX_Consents_Range NONCLUSTERED (EffectiveFrom, EffectiveTo)
);
GO

/* Hospital domain */
IF OBJECT_ID('hospital.Hospitals', 'U') IS NOT NULL DROP TABLE hospital.Hospitals;
GO
CREATE TABLE hospital.Hospitals (
    HospitalId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Hospitals_Id DEFAULT NEWID(),
    Name NVARCHAR(200) NOT NULL,
    Type NVARCHAR(100) NULL,
    AddressLine1 NVARCHAR(200) NULL,
    AddressLine2 NVARCHAR(200) NULL,
    City NVARCHAR(100) NULL,
    State NVARCHAR(100) NULL,
    PostalCode NVARCHAR(50) NULL,
    Country NVARCHAR(100) NULL,
    ContactPhone NVARCHAR(50) NULL,
    ContactEmail NVARCHAR(200) NULL,
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_Hospitals_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME2(7) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_Hospitals_IsDeleted DEFAULT(0),
    CONSTRAINT PK_Hospitals PRIMARY KEY CLUSTERED (HospitalId)
);
GO

IF OBJECT_ID('hospital.Doctors', 'U') IS NOT NULL DROP TABLE hospital.Doctors;
GO
CREATE TABLE hospital.Doctors (
    DoctorId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Doctors_Id DEFAULT NEWID(),
    HospitalId UNIQUEIDENTIFIER NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Credentials NVARCHAR(200) NULL,
    SpecialtyId UNIQUEIDENTIFIER NULL,
    ScheduleJson NVARCHAR(MAX) NULL,
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_Doctors_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME2(7) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_Doctors_IsDeleted DEFAULT(0),
    CONSTRAINT PK_Doctors PRIMARY KEY CLUSTERED (DoctorId),
    CONSTRAINT FK_Doctors_Hospital FOREIGN KEY (HospitalId) REFERENCES hospital.Hospitals(HospitalId),
    CONSTRAINT FK_Doctors_Specialty FOREIGN KEY (SpecialtyId) REFERENCES core.Specialties(SpecialtyId),
    INDEX IX_Doctors_Hospital NONCLUSTERED (HospitalId) WHERE IsDeleted = 0
);
GO

IF OBJECT_ID('hospital.Encounters', 'U') IS NOT NULL DROP TABLE hospital.Encounters;
GO
CREATE TABLE hospital.Encounters (
    EncounterId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Encounters_Id DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    HospitalId UNIQUEIDENTIFIER NOT NULL,
    DoctorId UNIQUEIDENTIFIER NULL,
    StartAt DATETIME2(7) NOT NULL,
    EndAt DATETIME2(7) NULL,
    DischargeSummaryId UNIQUEIDENTIFIER NULL, -- file id
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_Encounters_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME2(7) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_Encounters_IsDeleted DEFAULT(0),
    CONSTRAINT PK_Encounters PRIMARY KEY CLUSTERED (EncounterId),
    CONSTRAINT FK_Encounters_Patient FOREIGN KEY (PatientId) REFERENCES patient.Patients(PatientId),
    CONSTRAINT FK_Encounters_Hospital FOREIGN KEY (HospitalId) REFERENCES hospital.Hospitals(HospitalId),
    CONSTRAINT FK_Encounters_Doctor FOREIGN KEY (DoctorId) REFERENCES hospital.Doctors(DoctorId),
    CONSTRAINT FK_Encounters_DischargeFile FOREIGN KEY (DischargeSummaryId) REFERENCES core.Files(FileId),
    INDEX IX_Encounters_Patient NONCLUSTERED (PatientId) WHERE IsDeleted = 0,
    INDEX IX_Encounters_Hospital NONCLUSTERED (HospitalId) WHERE IsDeleted = 0
);
GO

IF OBJECT_ID('hospital.Vitals', 'U') IS NOT NULL DROP TABLE hospital.Vitals;
GO
CREATE TABLE hospital.Vitals (
    VitalId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Vitals_Id DEFAULT NEWID(),
    EncounterId UNIQUEIDENTIFIER NOT NULL,
    TakenAt DATETIME2(7) NOT NULL,
    Name NVARCHAR(100) NOT NULL, -- BP, HR, Temp, etc.
    Value NVARCHAR(50) NOT NULL,
    Unit NVARCHAR(50) NULL,
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_Vitals_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME2(7) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_Vitals_IsDeleted DEFAULT(0),
    CONSTRAINT PK_Vitals PRIMARY KEY CLUSTERED (VitalId),
    CONSTRAINT FK_Vitals_Encounter FOREIGN KEY (EncounterId) REFERENCES hospital.Encounters(EncounterId),
    INDEX IX_Vitals_Encounter NONCLUSTERED (EncounterId) WHERE IsDeleted = 0
);
GO

/* Lab domain */
IF OBJECT_ID('lab.Laboratories', 'U') IS NOT NULL DROP TABLE lab.Laboratories;
GO
CREATE TABLE lab.Laboratories (
    LaboratoryId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Laboratories_Id DEFAULT NEWID(),
    Name NVARCHAR(200) NOT NULL,
    AddressLine1 NVARCHAR(200) NULL,
    AddressLine2 NVARCHAR(200) NULL,
    City NVARCHAR(100) NULL,
    State NVARCHAR(100) NULL,
    PostalCode NVARCHAR(50) NULL,
    Country NVARCHAR(100) NULL,
    ContactPhone NVARCHAR(50) NULL,
    ContactEmail NVARCHAR(200) NULL,
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_Laboratories_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME2(7) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_Laboratories_IsDeleted DEFAULT(0),
    CONSTRAINT PK_Laboratories PRIMARY KEY CLUSTERED (LaboratoryId)
);
GO

IF OBJECT_ID('lab.LabServices', 'U') IS NOT NULL DROP TABLE lab.LabServices;
GO
CREATE TABLE lab.LabServices (
    LabServiceId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_LabServices_Id DEFAULT NEWID(),
    LaboratoryId UNIQUEIDENTIFIER NOT NULL,
    TestCodeId UNIQUEIDENTIFIER NOT NULL,
    Price DECIMAL(18,2) NULL,
    IsActive BIT NOT NULL CONSTRAINT DF_LabServices_IsActive DEFAULT(1),
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_LabServices_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME2(7) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_LabServices_IsDeleted DEFAULT(0),
    CONSTRAINT PK_LabServices PRIMARY KEY CLUSTERED (LabServiceId),
    CONSTRAINT FK_LabServices_Lab FOREIGN KEY (LaboratoryId) REFERENCES lab.Laboratories(LaboratoryId),
    CONSTRAINT FK_LabServices_TestCode FOREIGN KEY (TestCodeId) REFERENCES core.TestCodes(TestCodeId),
    INDEX IX_LabServices_Lab NONCLUSTERED (LaboratoryId) WHERE IsDeleted = 0
);
GO

IF OBJECT_ID('lab.LabTestOrders', 'U') IS NOT NULL DROP TABLE lab.LabTestOrders;
GO
CREATE TABLE lab.LabTestOrders (
    LabTestOrderId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_LabTestOrders_Id DEFAULT NEWID(),
    PatientId UNIQUEIDENTIFIER NOT NULL,
    OrderingHospitalId UNIQUEIDENTIFIER NULL,
    OrderingDoctorId UNIQUEIDENTIFIER NULL,
    LaboratoryId UNIQUEIDENTIFIER NOT NULL,
    Priority NVARCHAR(50) NULL,
    Specimen NVARCHAR(100) NULL,
    Instructions NVARCHAR(1000) NULL,
    Status NVARCHAR(50) NOT NULL, -- Ordered, InProgress, Completed, Cancelled
    OrderedAt DATETIME2(7) NOT NULL CONSTRAINT DF_LabTestOrders_OrderedAt DEFAULT SYSUTCDATETIME(),
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_LabTestOrders_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME2(7) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_LabTestOrders_IsDeleted DEFAULT(0),
    CONSTRAINT PK_LabTestOrders PRIMARY KEY CLUSTERED (LabTestOrderId),
    CONSTRAINT FK_LabTestOrders_Patient FOREIGN KEY (PatientId) REFERENCES patient.Patients(PatientId),
    CONSTRAINT FK_LabTestOrders_Hospital FOREIGN KEY (OrderingHospitalId) REFERENCES hospital.Hospitals(HospitalId),
    CONSTRAINT FK_LabTestOrders_Doctor FOREIGN KEY (OrderingDoctorId) REFERENCES hospital.Doctors(DoctorId),
    CONSTRAINT FK_LabTestOrders_Lab FOREIGN KEY (LaboratoryId) REFERENCES lab.Laboratories(LaboratoryId),
    INDEX IX_LabTestOrders_Patient NONCLUSTERED (PatientId) WHERE IsDeleted = 0
);
GO

IF OBJECT_ID('lab.LabTestOrderItems', 'U') IS NOT NULL DROP TABLE lab.LabTestOrderItems;
GO
CREATE TABLE lab.LabTestOrderItems (
    LabTestOrderItemId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_LabTestOrderItems_Id DEFAULT NEWID(),
    LabTestOrderId UNIQUEIDENTIFIER NOT NULL,
    TestCodeId UNIQUEIDENTIFIER NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_LabTestOrderItems_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME2(7) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_LabTestOrderItems_IsDeleted DEFAULT(0),
    CONSTRAINT PK_LabTestOrderItems PRIMARY KEY CLUSTERED (LabTestOrderItemId),
    CONSTRAINT FK_LabTestOrderItems_Order FOREIGN KEY (LabTestOrderId) REFERENCES lab.LabTestOrders(LabTestOrderId),
    CONSTRAINT FK_LabTestOrderItems_TestCode FOREIGN KEY (TestCodeId) REFERENCES core.TestCodes(TestCodeId),
    INDEX IX_LabTestOrderItems_Order NONCLUSTERED (LabTestOrderId) WHERE IsDeleted = 0
);
GO

IF OBJECT_ID('lab.LabTestResults', 'U') IS NOT NULL DROP TABLE lab.LabTestResults;
GO
CREATE TABLE lab.LabTestResults (
    LabTestResultId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_LabTestResults_Id DEFAULT NEWID(),
    LabTestOrderItemId UNIQUEIDENTIFIER NOT NULL,
    Status NVARCHAR(50) NOT NULL, -- Pending, Final, Amended
    ResultFileId UNIQUEIDENTIFIER NULL,
    ReportedAt DATETIME2(7) NULL,
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_LabTestResults_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME2(7) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_LabTestResults_IsDeleted DEFAULT(0),
    CONSTRAINT PK_LabTestResults PRIMARY KEY CLUSTERED (LabTestResultId),
    CONSTRAINT FK_LabTestResults_OrderItem FOREIGN KEY (LabTestOrderItemId) REFERENCES lab.LabTestOrderItems(LabTestOrderItemId),
    CONSTRAINT FK_LabTestResults_File FOREIGN KEY (ResultFileId) REFERENCES core.Files(FileId),
    INDEX IX_LabTestResults_OrderItem NONCLUSTERED (LabTestOrderItemId) WHERE IsDeleted = 0
);
GO

IF OBJECT_ID('lab.LabResultValues', 'U') IS NOT NULL DROP TABLE lab.LabResultValues;
GO
CREATE TABLE lab.LabResultValues (
    LabResultValueId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_LabResultValues_Id DEFAULT NEWID(),
    LabTestResultId UNIQUEIDENTIFIER NOT NULL,
    TestCodeId UNIQUEIDENTIFIER NOT NULL,
    Value NVARCHAR(100) NOT NULL,
    Unit NVARCHAR(50) NULL,
    IsCritical BIT NOT NULL CONSTRAINT DF_LabResultValues_IsCritical DEFAULT(0),
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_LabResultValues_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME2(7) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_LabResultValues_IsDeleted DEFAULT(0),
    CONSTRAINT PK_LabResultValues PRIMARY KEY CLUSTERED (LabResultValueId),
    CONSTRAINT FK_LabResultValues_Result FOREIGN KEY (LabTestResultId) REFERENCES lab.LabTestResults(LabTestResultId),
    CONSTRAINT FK_LabResultValues_TestCode FOREIGN KEY (TestCodeId) REFERENCES core.TestCodes(TestCodeId),
    INDEX IX_LabResultValues_Result NONCLUSTERED (LabTestResultId) WHERE IsDeleted = 0
);
GO

/* Integration domain */
IF OBJECT_ID('integration.AuditLogs', 'U') IS NOT NULL DROP TABLE integration.AuditLogs;
GO
CREATE TABLE integration.AuditLogs (
    AuditLogId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_AuditLogs_Id DEFAULT NEWID(),
    CorrelationId UNIQUEIDENTIFIER NULL,
    Actor NVARCHAR(200) NULL,
    Action NVARCHAR(200) NOT NULL,
    Target NVARCHAR(200) NULL,
    Details NVARCHAR(2000) NULL,
    OccurredAt DATETIME2(7) NOT NULL CONSTRAINT DF_AuditLogs_OccurredAt DEFAULT SYSUTCDATETIME(),
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_AuditLogs_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_AuditLogs_IsDeleted DEFAULT(0),
    CONSTRAINT PK_AuditLogs PRIMARY KEY CLUSTERED (AuditLogId),
    INDEX IX_AuditLogs_Correlation NONCLUSTERED (CorrelationId)
);
GO

IF OBJECT_ID('integration.Notifications', 'U') IS NOT NULL DROP TABLE integration.Notifications;
GO
CREATE TABLE integration.Notifications (
    NotificationId UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Notifications_Id DEFAULT NEWID(),
    Recipient NVARCHAR(200) NOT NULL,
    Channel NVARCHAR(50) NOT NULL, -- Email, SMS, Push, Console
    Subject NVARCHAR(200) NULL,
    Body NVARCHAR(2000) NULL,
    SentAt DATETIME2(7) NULL,
    Status NVARCHAR(50) NOT NULL CONSTRAINT DF_Notifications_Status DEFAULT('Pending'),
    CorrelationId UNIQUEIDENTIFIER NULL,
    CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_Notifications_CreatedDate DEFAULT SYSUTCDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    ModifiedDate DATETIME2(7) NULL,
    ModifiedBy NVARCHAR(100) NULL,
    IsDeleted BIT NOT NULL CONSTRAINT DF_Notifications_IsDeleted DEFAULT(0),
    CONSTRAINT PK_Notifications PRIMARY KEY CLUSTERED (NotificationId),
    INDEX IX_Notifications_Status NONCLUSTERED (Status)
);
GO

/* Indexing examples for performance */
CREATE OR ALTER VIEW patient.vwPatientSearch AS
SELECT p.PatientId,
       p.FirstName,
        p.LastName,
        p.DateOfBirth,
        p.Phone,
        p.Email,
        p.City,
        p.State
FROM patient.Patients p
WHERE p.IsDeleted = 0;
GO

/* Representative Stored Procedures */
IF OBJECT_ID('patient.GetPatientLongitudinalHistory', 'P') IS NOT NULL DROP PROCEDURE patient.GetPatientLongitudinalHistory;
GO
CREATE PROCEDURE patient.GetPatientLongitudinalHistory
    @PatientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 'Demographics' AS Section,
           p.PatientId,
           p.FirstName,
           p.LastName,
           p.DateOfBirth,
           p.Gender
    FROM patient.Patients p
    WHERE p.PatientId = @PatientId;

    SELECT 'Allergies' AS Section,
           a.Substance,
           a.Reaction,
           a.Severity,
           a.NotedDate
    FROM patient.PatientAllergies a
    WHERE a.PatientId = @PatientId AND a.IsDeleted = 0;

    SELECT 'Encounters' AS Section,
           e.EncounterId,
           e.StartAt,
           e.EndAt,
           h.Name AS HospitalName
    FROM hospital.Encounters e
    JOIN hospital.Hospitals h ON h.HospitalId = e.HospitalId
    WHERE e.PatientId = @PatientId AND e.IsDeleted = 0;

    SELECT 'LabResults' AS Section,
           r.LabTestResultId,
           tc.Code,
           tc.Name,
           v.Value,
           v.Unit,
           v.IsCritical,
           r.ReportedAt
    FROM lab.LabTestResults r
    JOIN lab.LabResultValues v ON v.LabTestResultId = r.LabTestResultId
    JOIN core.TestCodes tc ON tc.TestCodeId = v.TestCodeId
    JOIN lab.LabTestOrderItems oi ON oi.LabTestOrderItemId = r.LabTestOrderItemId
    JOIN lab.LabTestOrders o ON o.LabTestOrderId = oi.LabTestOrderId
    WHERE o.PatientId = @PatientId AND r.IsDeleted = 0 AND v.IsDeleted = 0;
END;
GO

IF OBJECT_ID('lab.AggregateLabResultsByTestCode', 'P') IS NOT NULL DROP PROCEDURE lab.AggregateLabResultsByTestCode;
GO
CREATE PROCEDURE lab.AggregateLabResultsByTestCode
    @PatientId UNIQUEIDENTIFIER,
    @TestCode NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT tc.Code,
           tc.Name,
           COUNT(*) AS ResultCount,
           SUM(CASE WHEN v.IsCritical = 1 THEN 1 ELSE 0 END) AS CriticalCount,
           MIN(r.ReportedAt) AS FirstReported,
           MAX(r.ReportedAt) AS LastReported
    FROM lab.LabTestResults r
    JOIN lab.LabResultValues v ON v.LabTestResultId = r.LabTestResultId
    JOIN core.TestCodes tc ON tc.TestCodeId = v.TestCodeId
    JOIN lab.LabTestOrderItems oi ON oi.LabTestOrderItemId = r.LabTestOrderItemId
    JOIN lab.LabTestOrders o ON o.LabTestOrderId = oi.LabTestOrderId
    WHERE o.PatientId = @PatientId AND tc.Code = @TestCode AND r.IsDeleted = 0 AND v.IsDeleted = 0
    GROUP BY tc.Code, tc.Name;
END;
GO

IF OBJECT_ID('hospital.SummarizeTreatmentEpisode', 'P') IS NOT NULL DROP PROCEDURE hospital.SummarizeTreatmentEpisode;
GO
CREATE PROCEDURE hospital.SummarizeTreatmentEpisode
    @EncounterId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT e.EncounterId,
           e.StartAt,
           e.EndAt,
           h.Name AS HospitalName,
           d.FirstName + ' ' + d.LastName AS DoctorName
    FROM hospital.Encounters e
    JOIN hospital.Hospitals h ON h.HospitalId = e.HospitalId
    LEFT JOIN hospital.Doctors d ON d.DoctorId = e.DoctorId
    WHERE e.EncounterId = @EncounterId;

    SELECT v.TakenAt, v.Name, v.Value, v.Unit
    FROM hospital.Vitals v
    WHERE v.EncounterId = @EncounterId AND v.IsDeleted = 0
    ORDER BY v.TakenAt;
END;
GO

/* Seed Data */
IF NOT EXISTS (SELECT 1 FROM core.Specialties)
BEGIN
    INSERT INTO core.Specialties (SpecialtyId, Code, Name, CreatedBy)
    VALUES
        (NEWID(), 'CARD', 'Cardiology', 'seed'),
        (NEWID(), 'NEUR', 'Neurology', 'seed'),
        (NEWID(), 'ORTH', 'Orthopedics', 'seed');
END
GO

IF NOT EXISTS (SELECT 1 FROM core.TestCodes)
BEGIN
    INSERT INTO core.TestCodes (TestCodeId, Code, Name, Unit, ReferenceRange, CreatedBy)
    VALUES
        (NEWID(), '718-7', 'Hemoglobin', 'g/dL', '13.5-17.5', 'seed'),
        (NEWID(), '4548-4', 'Hematocrit', '%', '38.8-50.0', 'seed'),
        (NEWID(), '2951-2', 'Sodium', 'mmol/L', '135-145', 'seed');
END
GO

/* Helpful permissions for local dev (optional, commented) */
-- GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::patient TO PUBLIC;
-- GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::hospital TO PUBLIC;
-- GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::lab TO PUBLIC;
-- GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::integration TO PUBLIC;

PRINT 'EMR master schema deployed.';
GO

