-- =============================================
-- Healthcare Platform - Complete Test Data for All Services
-- Comprehensive sample data for development and testing
-- =============================================

-- First ensure Patient Service test data is loaded
-- (See 01_PatientService_TestData.sql)

-- =============================================
-- FAMILY SERVICE TEST DATA
-- =============================================
USE [HealthcarePlatform_Family]
GO

-- Get John Smith's ID (primary account holder)
DECLARE @JohnSmithId UNIQUEIDENTIFIER = (
    SELECT TOP 1 Id FROM [HealthcarePlatform_Patient].patient.Patients 
    WHERE FirstName = 'John' AND LastName = 'Smith' AND IsPrimaryAccountHolder = 1
)

DECLARE @JaneSmithId UNIQUEIDENTIFIER = (
    SELECT TOP 1 Id FROM [HealthcarePlatform_Patient].patient.Patients 
    WHERE FirstName = 'Jane' AND LastName = 'Smith' AND ProfileType = 'Spouse'
)

-- Create Smith Family Group
DECLARE @SmithFamilyId UNIQUEIDENTIFIER = NEWID()

INSERT INTO family.FamilyGroups (
    Id, FamilyName, PrimaryAccountHolderId, PrimaryEmail, PrimaryPhone,
    AddressLine1, City, State, ZipCode, Country,
    IsActive, CreatedBy, CreatedAt
)
VALUES (
    @SmithFamilyId, 'Smith Family', @JohnSmithId, 'smith.family@email.com', '+1-555-0101',
    '123 Main Street', 'New York', 'NY', '10001', 'USA',
    1, 'system', GETUTCDATE()
)

-- Add Family Members
INSERT INTO family.FamilyMembers (
    Id, FamilyGroupId, PatientId, RelationshipType, RelationshipToHead,
    IsHead, CanManageFamilyData, CanViewAllRecords, CanShareOnBehalf,
    IsActive, JoinedDate, CreatedBy, CreatedAt
)
VALUES
-- John (Head)
(NEWID(), @SmithFamilyId, @JohnSmithId, 'Self', 'Self',
 1, 1, 1, 1, 1, GETUTCDATE(), 'system', GETUTCDATE()),

-- Jane (Spouse)
(NEWID(), @SmithFamilyId, @JaneSmithId, 'Spouse', 'Spouse',
 0, 1, 1, 1, 1, GETUTCDATE(), 'system', GETUTCDATE())

-- Add Emergency Contact for John
INSERT INTO family.EmergencyContacts (
    Id, PatientId, ContactName, Relationship, PhoneNumber, Email,
    ContactPriority, CanReceiveMedicalInfo, CanMakeDecisions,
    IsActive, CreatedBy, CreatedAt
)
VALUES (
    NEWID(), @JohnSmithId, 'Jane Smith', 'Spouse', '+1-555-0103', 'jane.smith@email.com',
    1, 1, 1, 1, 'system', GETUTCDATE()
)

PRINT '✓ Family Service test data inserted'
GO

-- =============================================
-- HEALTH HISTORY SERVICE TEST DATA
-- =============================================
USE [HealthcarePlatform_HealthHistory]
GO

DECLARE @JohnSmithId UNIQUEIDENTIFIER = (
    SELECT TOP 1 Id FROM [HealthcarePlatform_Patient].patient.Patients 
    WHERE FirstName = 'John' AND LastName = 'Smith' AND IsPrimaryAccountHolder = 1
)

DECLARE @EmmaSmithId UNIQUEIDENTIFIER = (
    SELECT TOP 1 Id FROM [HealthcarePlatform_Patient].patient.Patients 
    WHERE FirstName = 'Emma' AND LastName = 'Smith'
)

DECLARE @RobertJohnsonId UNIQUEIDENTIFIER = (
    SELECT TOP 1 Id FROM [HealthcarePlatform_Patient].patient.Patients 
    WHERE FirstName = 'Robert' AND LastName = 'Johnson'
)

-- Insert Allergies
INSERT INTO health.Allergies (
    Id, PatientId, AllergenName, AllergenCode, Category, AllergyType,
    Criticality, Severity, ClinicalStatus, VerificationStatus,
    ReactionDescription, OnsetDate, RecordedDate,
    RecordedBy, SourceType, CreatedBy, CreatedAt
)
VALUES
-- John's Penicillin allergy
(NEWID(), @JohnSmithId, 'Penicillin', 'J01CA', 'medication', 'allergy',
 'high', 'severe', 'active', 'confirmed',
 'Severe rash, hives, and difficulty breathing', '2010-05-15', GETUTCDATE(),
 'Dr. Smith', 'provider-entered', 'system', GETUTCDATE()),

-- Emma's Peanut allergy
(NEWID(), @EmmaSmithId, 'Peanuts', 'F13.1', 'food', 'allergy',
 'high', 'severe', 'active', 'confirmed',
 'Anaphylaxis - requires immediate EpiPen administration', '2020-03-10', GETUTCDATE(),
 'Dr. Martinez', 'provider-entered', 'system', GETUTCDATE())

-- Insert Medications
INSERT INTO health.Medications (
    Id, PatientId, MedicationName, MedicationCode, GenericName, BrandName,
    Dosage, DosageForm, Route, Frequency, Status,
    StartDate, PrescribedBy, PrescribedDate,
    ReasonDescription, Instructions, SourceType, CreatedBy, CreatedAt
)
VALUES
-- Robert's Hypertension medication
(NEWID(), @RobertJohnsonId, 'Lisinopril', '314076', 'Lisinopril', 'Prinivil',
 '10mg', 'Tablet', 'Oral', 'Once daily', 'active',
 '2020-01-15', 'Dr. Johnson', '2020-01-15',
 'High blood pressure management', 'Take with water in the morning', 'provider-entered', 'system', GETUTCDATE()),

-- Emma's Asthma medication
(NEWID(), @EmmaSmithId, 'Albuterol Inhaler', '745790', 'Albuterol', 'Proventil',
 '90mcg', 'Inhaler', 'Inhalation', 'As needed', 'active',
 '2022-06-01', 'Dr. Martinez', '2022-06-01',
 'Asthma rescue inhaler', 'Use when experiencing difficulty breathing', 'provider-entered', 'system', GETUTCDATE())

-- Insert Immunizations
INSERT INTO health.Immunizations (
    Id, PatientId, VaccineName, VaccineCode, AdministrationDate,
    DoseNumber, SeriesCount, Status, BodySite, Route, DoseQuantity,
    AdministeredBy, AdministeredAt, SourceType, CreatedBy, CreatedAt
)
VALUES
-- Emma's COVID-19 vaccine
(NEWID(), @EmmaSmithId, 'COVID-19, mRNA (Pfizer-BioNTech)', '208', '2024-01-15',
 1, 2, 'completed', 'Left arm', 'Intramuscular', '0.3mL',
 'Nurse Johnson', 'Children\'s Clinic', 'provider-entered', 'system', GETUTCDATE()),

-- Emma's COVID-19 vaccine dose 2
(NEWID(), @EmmaSmithId, 'COVID-19, mRNA (Pfizer-BioNTech)', '208', '2024-02-12',
 2, 2, 'completed', 'Left arm', 'Intramuscular', '0.3mL',
 'Nurse Johnson', 'Children\'s Clinic', 'provider-entered', 'system', GETUTCDATE())

-- Insert Conditions
INSERT INTO health.Conditions (
    Id, PatientId, ConditionName, ConditionCode, CodingSystem, Category,
    ClinicalStatus, VerificationStatus, Severity,
    OnsetDate, RecordedDate, RecordedBy, DiagnosedBy,
    Notes, SourceType, CreatedBy, CreatedAt
)
VALUES
-- Emma's Asthma
(NEWID(), @EmmaSmithId, 'Asthma', 'J45.909', 'ICD-10-CM', 'problem-list-item',
 'active', 'confirmed', 'mild',
 '2022-06-01', GETUTCDATE(), 'Dr. Martinez', 'Dr. Martinez',
 'Mild persistent asthma. Using albuterol as rescue inhaler.', 'provider-entered', 'system', GETUTCDATE()),

-- Robert's Hypertension
(NEWID(), @RobertJohnsonId, 'Essential (primary) hypertension', 'I10', 'ICD-10-CM', 'problem-list-item',
 'active', 'confirmed', 'moderate',
 '2020-01-15', GETUTCDATE(), 'Dr. Johnson', 'Dr. Johnson',
 'Well-controlled with medication. Monitor blood pressure weekly.', 'provider-entered', 'system', GETUTCDATE())

PRINT '✓ Health History Service test data inserted'
GO

-- =============================================
-- INSURANCE SERVICE TEST DATA
-- =============================================
USE [HealthcarePlatform_Insurance]
GO

DECLARE @JohnSmithId UNIQUEIDENTIFIER = (
    SELECT TOP 1 Id FROM [HealthcarePlatform_Patient].patient.Patients 
    WHERE FirstName = 'John' AND LastName = 'Smith' AND IsPrimaryAccountHolder = 1
)

DECLARE @MariaGarciaId UNIQUEIDENTIFIER = (
    SELECT TOP 1 Id FROM [HealthcarePlatform_Patient].patient.Patients 
    WHERE FirstName = 'Maria' AND LastName = 'Garcia'
)

-- Insert Insurance Policies
DECLARE @JohnPolicyId UNIQUEIDENTIFIER = NEWID()
DECLARE @MariaPolicyId UNIQUEIDENTIFIER = NEWID()

INSERT INTO insurance.InsurancePolicies (
    Id, PatientId, PolicyNumber, GroupNumber, SubscriberId,
    InsuranceCompanyName, InsuranceCompanyCode, CompanyPhone,
    PolicyType, CoverageType, PlanName, PlanNetwork,
    IsSubscriber, EffectiveDate, TerminationDate,
    AnnualDeductible, DeductibleMet, OutOfPocketMax, OutOfPocketMet,
    CopayPrimaryCare, CopaySpecialist, CopayEmergency, CoinsurancePercentage,
    Status, IsPrimary, Priority, IsVerified, VerifiedAt,
    CreatedBy, CreatedAt
)
VALUES
-- John's primary insurance
(@JohnPolicyId, @JohnSmithId, 'BCBS-123456789', 'GRP-999', 'SUB-123456',
 'Blue Cross Blue Shield', '12345', '1-800-BCBS-123',
 'Medical', 'Family', 'Gold Plus Plan', 'PPO',
 1, '2025-01-01', NULL,
 5000.00, 1500.00, 8000.00, 2500.00,
 25.00, 50.00, 200.00, 20.00,
 'active', 1, 1, 1, GETUTCDATE(),
 'system', GETUTCDATE()),

-- Maria's insurance
(@MariaPolicyId, @MariaGarciaId, 'AETNA-987654321', 'GRP-777', 'SUB-987654',
 'Aetna', '54321', '1-800-AETNA-01',
 'Medical', 'Individual', 'Basic Plan', 'HMO',
 1, '2025-01-01', NULL,
 3000.00, 500.00, 6000.00, 1000.00,
 15.00, 40.00, 150.00, 20.00,
 'active', 1, 1, 1, GETUTCDATE(),
 'system', GETUTCDATE())

-- Insert Sample Claims
INSERT INTO insurance.Claims (
    Id, PatientId, InsurancePolicyId, ClaimNumber, ClaimType,
    ServiceDate, ClaimDate, ReceivedDate, ProcessedDate,
    ProviderName, ProviderNPI, FacilityName,
    TotalCharges, AllowedAmount, PaidAmount, PatientResponsibility,
    Deductible, Coinsurance, Copay,
    Status, PrimaryDiagnosisCode, PrimaryDiagnosisDescription,
    CreatedBy, CreatedAt
)
VALUES
-- John's recent claim
(NEWID(), @JohnSmithId, @JohnPolicyId, 'CLM-2025-001234', 'professional',
 '2025-09-15', '2025-09-16', '2025-09-17', '2025-09-20',
 'Dr. Johnson', '1234567890', 'St. Mary\'s Hospital',
 1250.00, 1000.00, 800.00, 200.00,
 150.00, 40.00, 25.00,
 'approved', 'Z00.00', 'Annual physical examination',
 'system', GETUTCDATE())

PRINT '✓ Insurance Service test data inserted'
GO

-- =============================================
-- CONSENT/AUDIT SERVICE TEST DATA
-- =============================================
USE [HealthcarePlatform_Consent]
GO

DECLARE @JohnSmithId UNIQUEIDENTIFIER = (
    SELECT TOP 1 Id FROM [HealthcarePlatform_Patient].patient.Patients 
    WHERE FirstName = 'John' AND LastName = 'Smith' AND IsPrimaryAccountHolder = 1
)

-- Insert Sample Consents
DECLARE @ConsentId1 UNIQUEIDENTIFIER = NEWID()
DECLARE @ConsentId2 UNIQUEIDENTIFIER = NEWID()

INSERT INTO consent.Consents (
    Id, PatientId, ConsentType, ConsentScope, ConsentCategory,
    Status, EffectiveDate, ExpirationDate, IsIndefinite,
    PolicyRule, CanView, CanUpdate, CanShare, CanExport,
    CoverageScope, GrantedToOrganization,
    IsSignedElectronically, SignedDate,
    QRCodeData, QRCodeExpiration, QRCodeMaxUsage, QRCodeUsageCount,
    CreatedBy, CreatedAt
)
VALUES
-- General consent for treatment
(@ConsentId1, @JohnSmithId, 'Treatment', 'PatientPrivacy', 'HIPAA',
 'active', GETUTCDATE(), NULL, 1,
 'permit', 1, 1, 1, 1,
 'all', 'St. Mary\'s Hospital',
 1, GETUTCDATE(),
 NULL, NULL, NULL, 0,
 'john.smith@email.com', GETUTCDATE()),

-- QR Code consent (time-limited)
(@ConsentId2, @JohnSmithId, 'QRCodeSharing', 'EmergencyIntake', 'Emergency',
 'active', GETUTCDATE(), DATEADD(HOUR, 24, GETUTCDATE()), 0,
 'permit', 1, 0, 0, 0,
 'all', NULL,
 1, GETUTCDATE(),
 'QR-CODE-DATA-BASE64-ENCODED', DATEADD(HOUR, 24, GETUTCDATE()), 5, 0,
 'john.smith@email.com', GETUTCDATE())

-- Insert Sample Audit Logs
INSERT INTO consent.AuditLog (
    Id, EventTimestamp, EventType, EventCategory, EventAction, EventOutcome,
    ActorId, ActorType, ActorName, ActorRole,
    TargetType, TargetId, PatientId,
    SourceType, IPAddress, IsAuthorized, HasValidConsent
)
VALUES
(NEWID(), GETUTCDATE(), 'PatientView', 'DataAccess', 'View', 'Success',
 'john.smith@email.com', 'Patient', 'John Smith', 'Patient',
 'Patient', @JohnSmithId, @JohnSmithId,
 'Portal', '192.168.1.100', 1, 1),

(NEWID(), DATEADD(HOUR, -2, GETUTCDATE()), 'AllergyCreate', 'DataAccess', 'Create', 'Success',
 'john.smith@email.com', 'Patient', 'John Smith', 'Patient',
 'Allergy', NULL, @JohnSmithId,
 'Portal', '192.168.1.100', 1, 1)

-- Insert Data Sharing Events
INSERT INTO consent.DataSharingEvents (
    Id, PatientId, ConsentId, ShareMethod, SharedWith, SharedWithType,
    DataShared, ShareScope, ShareInitiatedAt, ShareExpiresAt,
    Status, AccessCount, MaxAccessCount, QRCodeToken,
    SharePurpose, CreatedBy, CreatedAt
)
VALUES
(NEWID(), @JohnSmithId, @ConsentId2, 'QRCode', 'St. Mary\'s Hospital', 'Hospital',
 '["Patient", "AllergyIntolerance", "Medication"]', 'Partial',
 GETUTCDATE(), DATEADD(HOUR, 24, GETUTCDATE()),
 'active', 0, 5, 'QR-TOKEN-12345',
 'Emergency intake at hospital', 'john.smith@email.com', GETUTCDATE())

PRINT '✓ Consent/Audit Service test data inserted'
GO

-- =============================================
-- SUMMARY
-- =============================================
PRINT ''
PRINT '========================================='
PRINT 'ALL TEST DATA LOADED SUCCESSFULLY'
PRINT '========================================='
PRINT ''
PRINT 'Services with Test Data:'
PRINT '  ✓ Patient Service (6 patients)'
PRINT '  ✓ Family Service (1 family group, 2 members, 1 emergency contact)'
PRINT '  ✓ Health History Service (2 allergies, 2 medications, 2 immunizations, 2 conditions)'
PRINT '  ✓ Insurance Service (2 policies, 1 claim)'
PRINT '  ✓ Consent/Audit Service (2 consents, 2 audit logs, 1 sharing event)'
PRINT ''
PRINT 'Test Accounts:'
PRINT '  - Smith Family (John, Jane, Emma, Oliver) - Complete family'
PRINT '  - Maria Garcia - Independent account with insurance'
PRINT '  - Robert Johnson - Senior with chronic condition'
PRINT ''
PRINT 'You can now test all services!'
PRINT '========================================='
GO
