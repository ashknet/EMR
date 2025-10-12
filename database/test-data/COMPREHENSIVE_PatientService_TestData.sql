-- =============================================
-- Comprehensive Patient Service - Sample Data
-- Complete test data for all functionality
-- =============================================

USE [HealthcarePlatform]
GO

-- Clear existing data (for re-running)
DELETE FROM patient.AuditLog;
DELETE FROM patient.Transfers;
DELETE FROM patient.Visits;
DELETE FROM patient.Documents;
DELETE FROM patient.Consents;
DELETE FROM patient.Claims;
DELETE FROM patient.InsurancePolicies;
DELETE FROM patient.Immunizations;
DELETE FROM patient.Conditions;
DELETE FROM patient.Medications;
DELETE FROM patient.Allergies;
DELETE FROM patient.FamilyMembers;
DELETE FROM patient.Patients;
GO

PRINT 'Creating comprehensive sample data...'
GO

-- =============================================
-- PATIENTS (Smith Family + Independent)
-- =============================================
DECLARE @JohnId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
DECLARE @JaneId UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
DECLARE @EmmaId UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';
DECLARE @OliverId UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444444';
DECLARE @MariaId UNIQUEIDENTIFIER = '55555555-5555-5555-5555-555555555555';
DECLARE @RobertId UNIQUEIDENTIFIER = '66666666-6666-6666-6666-666666666666';

-- John Smith (Primary Account Holder)
INSERT INTO patient.Patients (
    Id, FirstName, MiddleName, LastName, DateOfBirth, Gender,
    Email, PhoneNumber, SSN,
    AddressLine1, AddressLine2, City, State, ZipCode,
    PreferredLanguage, Ethnicity, Race, MaritalStatus,
    MRN, ProfileType, IsPrimaryAccountHolder,
    EmergencyContactName, EmergencyContactPhone, EmergencyContactRelationship,
    IsActive, CreatedBy, CreatedAt
)
VALUES (
    @JohnId, 'John', 'Michael', 'Smith', '1987-05-15', 'Male',
    'john.smith@email.com', '+1-555-0101', 'XXX-XX-1234',
    '123 Main Street', 'Apt 4B', 'New York', 'NY', '10001',
    'English', 'Not Hispanic or Latino', 'White', 'Married',
    'MRN001234', 'Self', 1,
    'Jane Smith', '+1-555-0103', 'Spouse',
    1, 'system', GETUTCDATE()
);

-- Jane Smith (Spouse)
INSERT INTO patient.Patients (
    Id, FirstName, MiddleName, LastName, DateOfBirth, Gender,
    Email, PhoneNumber,
    AddressLine1, City, State, ZipCode,
    MRN, ProfileType, IsPrimaryAccountHolder, PrimaryAccountHolderId,
    RelationshipToPrimary,
    IsActive, CreatedBy, CreatedAt
)
VALUES (
    @JaneId, 'Jane', 'Elizabeth', 'Smith', '1989-08-22', 'Female',
    'jane.smith@email.com', '+1-555-0103',
    '123 Main Street', 'New York', 'NY', '10001',
    'MRN001235', 'Spouse', 0, @JohnId,
    'Spouse',
    1, 'system', GETUTCDATE()
);

-- Emma Smith (Daughter)
INSERT INTO patient.Patients (
    Id, FirstName, LastName, DateOfBirth, Gender,
    Email, PhoneNumber,
    AddressLine1, City, State, ZipCode,
    MRN, ProfileType, IsPrimaryAccountHolder, PrimaryAccountHolderId,
    RelationshipToPrimary,
    IsActive, CreatedBy, CreatedAt
)
VALUES (
    @EmmaId, 'Emma', 'Smith', '2017-03-10', 'Female',
    'john.smith@email.com', '+1-555-0101',
    '123 Main Street', 'New York', 'NY', '10001',
    'MRN001236', 'Child', 0, @JohnId,
    'Daughter',
    1, 'system', GETUTCDATE()
);

-- Oliver Smith (Son)
INSERT INTO patient.Patients (
    Id, FirstName, LastName, DateOfBirth, Gender,
    AddressLine1, City, State, ZipCode,
    MRN, ProfileType, IsPrimaryAccountHolder, PrimaryAccountHolderId,
    RelationshipToPrimary,
    IsActive, CreatedBy, CreatedAt
)
VALUES (
    @OliverId, 'Oliver', 'Smith', '2020-11-05', 'Male',
    '123 Main Street', 'New York', 'NY', '10001',
    'MRN001237', 'Child', 0, @JohnId,
    'Son',
    1, 'system', GETUTCDATE()
);

-- Maria Garcia (Independent)
INSERT INTO patient.Patients (
    Id, FirstName, LastName, DateOfBirth, Gender,
    Email, PhoneNumber,
    AddressLine1, City, State, ZipCode,
    Ethnicity, Race,
    MRN, ProfileType, IsPrimaryAccountHolder,
    IsActive, CreatedBy, CreatedAt
)
VALUES (
    @MariaId, 'Maria', 'Garcia', '1995-06-30', 'Female',
    'maria.garcia@email.com', '+1-555-0201',
    '456 Oak Avenue', 'Los Angeles', 'CA', '90001',
    'Hispanic or Latino', 'Other',
    'MRN002001', 'Self', 1,
    1, 'system', GETUTCDATE()
);

-- Robert Johnson (Senior)
INSERT INTO patient.Patients (
    Id, FirstName, LastName, DateOfBirth, Gender,
    Email, PhoneNumber,
    AddressLine1, City, State, ZipCode,
    MRN, ProfileType, IsPrimaryAccountHolder,
    IsActive, CreatedBy, CreatedAt
)
VALUES (
    @RobertId, 'Robert', 'Johnson', '1952-12-20', 'Male',
    'robert.johnson@email.com', '+1-555-0301',
    '789 Pine Road', 'Chicago', 'IL', '60601',
    'MRN003001', 'Self', 1,
    1, 'system', GETUTCDATE()
);

PRINT '✓ 6 Patients created (Smith family + 2 independent)'
GO

-- =============================================
-- FAMILY MEMBERS
-- =============================================
INSERT INTO patient.FamilyMembers (
    PrimaryPatientId, FamilyMemberPatientId, RelationshipType,
    IsDependent, IsPrimaryContact,
    CanViewMedicalRecords, CanManageAppointments, CanAccessInsurance,
    IsActive, CreatedBy
)
VALUES
-- Jane as family member of John
('11111111-1111-1111-1111-111111111111', '22222222-2222-2222-2222-222222222222', 'Spouse', 
 0, 1, 1, 1, 1, 1, 'system'),

-- Emma as family member of John
('11111111-1111-1111-1111-111111111111', '33333333-3333-3333-3333-333333333333', 'Child', 
 1, 0, 1, 1, 1, 1, 'system'),

-- Oliver as family member of John
('11111111-1111-1111-1111-111111111111', '44444444-4444-4444-4444-444444444444', 'Child', 
 1, 0, 1, 1, 1, 1, 'system');

PRINT '✓ Family relationships established'
GO

-- =============================================
-- ALLERGIES
-- =============================================
INSERT INTO patient.Allergies (
    PatientId, AllergenName, AllergenCode, Category, AllergyType,
    Criticality, Severity, ClinicalStatus, VerificationStatus,
    ReactionDescription, OnsetDate, RecordedBy, CreatedBy
)
VALUES
-- John's Penicillin allergy
('11111111-1111-1111-1111-111111111111', 'Penicillin', 'J01CA', 'medication', 'allergy',
 'high', 'severe', 'active', 'confirmed',
 'Severe rash, hives, and difficulty breathing. Required emergency treatment.', 
 '2010-05-15', 'Dr. Smith', 'system'),

-- Emma's Peanut allergy
('33333333-3333-3333-3333-333333333333', 'Peanuts', 'F13.1', 'food', 'allergy',
 'high', 'severe', 'active', 'confirmed',
 'Anaphylaxis - requires immediate EpiPen administration. Carries EpiPen at all times.',
 '2020-03-10', 'Dr. Martinez', 'system'),

-- Robert's Aspirin sensitivity
('66666666-6666-6666-6666-666666666666', 'Aspirin', 'N02BA01', 'medication', 'intolerance',
 'low', 'mild', 'active', 'confirmed',
 'Stomach upset and mild nausea',
 '2015-08-20', 'Dr. Johnson', 'system');

PRINT '✓ 3 Critical allergies added'
GO

-- =============================================
-- MEDICATIONS
-- =============================================
INSERT INTO patient.Medications (
    PatientId, MedicationName, MedicationCode, GenericName, BrandName,
    Dosage, DosageForm, Route, Frequency, Status,
    StartDate, PrescribedBy, PrescribedDate,
    ReasonDescription, Instructions, CreatedBy
)
VALUES
-- Robert's Hypertension medication
('66666666-6666-6666-6666-666666666666', 'Lisinopril', '314076', 'Lisinopril', 'Prinivil',
 '10mg', 'Tablet', 'Oral', 'Once daily', 'active',
 '2020-01-15', 'Dr. Johnson', '2020-01-15',
 'High blood pressure management', 'Take with water in the morning', 'system'),

-- Emma's Asthma medication
('33333333-3333-3333-3333-333333333333', 'Albuterol Inhaler', '745790', 'Albuterol', 'Proventil',
 '90mcg', 'Inhaler', 'Inhalation', 'As needed', 'active',
 '2022-06-01', 'Dr. Martinez', '2022-06-01',
 'Asthma rescue inhaler', 'Use when experiencing difficulty breathing. Maximum 4 times per day.', 'system'),

-- John's Multivitamin
('11111111-1111-1111-1111-111111111111', 'Multivitamin', NULL, 'Multivitamin', 'Centrum',
 '1 tablet', 'Tablet', 'Oral', 'Once daily', 'active',
 '2024-01-01', NULL, NULL,
 'General health and wellness', 'Take with food', 'system');

PRINT '✓ 3 Medications added'
GO

-- =============================================
-- CONDITIONS
-- =============================================
INSERT INTO patient.Conditions (
    PatientId, ConditionName, ConditionCode, CodingSystem,
    ClinicalStatus, VerificationStatus, Severity,
    OnsetDate, DiagnosedBy, Notes, CreatedBy
)
VALUES
-- Emma's Asthma
('33333333-3333-3333-3333-333333333333', 'Asthma', 'J45.909', 'ICD-10-CM',
 'active', 'confirmed', 'mild',
 '2022-06-01', 'Dr. Martinez',
 'Mild persistent asthma. Well-controlled with albuterol as rescue inhaler. No recent exacerbations.',
 'system'),

-- Robert's Hypertension
('66666666-6666-6666-6666-666666666666', 'Essential (primary) hypertension', 'I10', 'ICD-10-CM',
 'active', 'confirmed', 'moderate',
 '2020-01-15', 'Dr. Johnson',
 'Well-controlled with medication. Blood pressure monitored weekly. Last reading: 128/82.',
 'system');

PRINT '✓ 2 Conditions added'
GO

-- =============================================
-- IMMUNIZATIONS
-- =============================================
INSERT INTO patient.Immunizations (
    PatientId, VaccineName, VaccineCode,
    AdministrationDate, DoseNumber, SeriesCount, Status,
    BodySite, Route, DoseQuantity,
    AdministeredBy, AdministeredAt, CreatedBy
)
VALUES
-- Emma's COVID-19 vaccines
('33333333-3333-3333-3333-333333333333', 'COVID-19, mRNA (Pfizer-BioNTech)', '208',
 '2024-01-15', 1, 2, 'completed',
 'Left arm', 'Intramuscular', '0.3mL',
 'Nurse Johnson', 'Children''s Clinic', 'system'),

('33333333-3333-3333-3333-333333333333', 'COVID-19, mRNA (Pfizer-BioNTech)', '208',
 '2024-02-12', 2, 2, 'completed',
 'Left arm', 'Intramuscular', '0.3mL',
 'Nurse Johnson', 'Children''s Clinic', 'system'),

-- Robert's Flu vaccine
('66666666-6666-6666-6666-666666666666', 'Influenza, seasonal', '140',
 '2024-09-15', 1, 1, 'completed',
 'Left arm', 'Intramuscular', '0.5mL',
 'Nurse Williams', 'Senior Care Clinic', 'system');

PRINT '✓ 3 Immunizations added'
GO

-- =============================================
-- INSURANCE POLICIES
-- =============================================
DECLARE @JohnPolicyId UNIQUEIDENTIFIER = NEWID();
DECLARE @MariaPolicyId UNIQUEIDENTIFIER = NEWID();

INSERT INTO patient.InsurancePolicies (
    Id, PatientId, PolicyNumber, GroupNumber, SubscriberId,
    InsuranceCompanyName, PolicyType, CoverageType, PlanName, PlanNetwork,
    IsSubscriber, EffectiveDate,
    AnnualDeductible, DeductibleMet, OutOfPocketMax, OutOfPocketMet,
    CopayPrimaryCare, CopaySpecialist, CopayEmergency,
    Status, IsPrimary, Priority, IsVerified, VerifiedAt,
    CreatedBy
)
VALUES
-- John's Family Policy (covers John, Jane, Emma, Oliver)
(@JohnPolicyId, '11111111-1111-1111-1111-111111111111', 
 'BCBS-123456789', 'GRP-999', 'SUB-123456',
 'Blue Cross Blue Shield', 'Medical', 'Family', 'Gold Plus Plan', 'PPO',
 1, '2025-01-01',
 5000.00, 1500.00, 8000.00, 2500.00,
 25.00, 50.00, 200.00,
 'active', 1, 1, 1, GETUTCDATE(),
 'system'),

-- Maria's Individual Policy
(@MariaPolicyId, '55555555-5555-5555-5555-555555555555',
 'AETNA-987654321', 'GRP-777', 'SUB-987654',
 'Aetna', 'Medical', 'Individual', 'Basic Plan', 'HMO',
 1, '2025-01-01',
 3000.00, 500.00, 6000.00, 1000.00,
 15.00, 40.00, 150.00,
 'active', 1, 1, 1, GETUTCDATE(),
 'system');

PRINT '✓ 2 Insurance policies created'
GO

-- =============================================
-- CLAIMS
-- =============================================
INSERT INTO patient.Claims (
    PatientId, InsurancePolicyId, ClaimNumber, ClaimType,
    ServiceDate, ClaimDate, ReceivedDate, ProcessedDate,
    ProviderName, ProviderNPI, FacilityName,
    TotalCharges, AllowedAmount, PaidAmount, PatientResponsibility,
    Deductible, Coinsurance, Copay,
    Status, PrimaryDiagnosisCode, PrimaryDiagnosisDescription,
    CreatedBy
)
VALUES
-- John's Annual Physical
('11111111-1111-1111-1111-111111111111', @JohnPolicyId, 'CLM-2025-001234', 'professional',
 '2025-09-15', '2025-09-16', '2025-09-17', '2025-09-20',
 'Dr. Johnson', '1234567890', 'St. Mary''s Hospital',
 1250.00, 1000.00, 800.00, 200.00,
 150.00, 40.00, 25.00,
 'approved', 'Z00.00', 'Annual physical examination',
 'system');

PRINT '✓ 1 Insurance claim added'
GO

-- =============================================
-- CONSENTS
-- =============================================
DECLARE @ConsentId1 UNIQUEIDENTIFIER = NEWID();
DECLARE @ConsentId2 UNIQUEIDENTIFIER = NEWID();

INSERT INTO patient.Consents (
    Id, PatientId, ConsentType, ConsentScope, ConsentCategory,
    Status, EffectiveDate, ExpirationDate, IsIndefinite,
    CanView, CanShare, CanExport,
    GrantedToOrganization,
    IsSignedElectronically, SignedDate,
    CreatedBy
)
VALUES
-- General consent for treatment
(@ConsentId1, '11111111-1111-1111-1111-111111111111', 
 'Treatment', 'PatientPrivacy', 'HIPAA',
 'active', GETUTCDATE(), NULL, 1,
 1, 1, 1,
 'St. Mary''s Hospital',
 1, GETUTCDATE(),
 'john.smith@email.com'),

-- QR Code consent (time-limited)
(@ConsentId2, '11111111-1111-1111-1111-111111111111',
 'QRCodeSharing', 'EmergencyIntake', 'Emergency',
 'active', GETUTCDATE(), DATEADD(HOUR, 24, GETUTCDATE()), 0,
 1, 0, 0,
 NULL,
 1, GETUTCDATE(),
 'john.smith@email.com');

-- Update QR code data
UPDATE patient.Consents 
SET QRCodeData = 'QR-' + CONVERT(NVARCHAR(50), Id),
    QRCodeExpiration = DATEADD(HOUR, 24, GETUTCDATE()),
    QRCodeMaxUsage = 5,
    QRCodeUsageCount = 0
WHERE Id = @ConsentId2;

PRINT '✓ 2 Consents created (including QR code)'
GO

-- =============================================
-- VISITS
-- =============================================
INSERT INTO patient.Visits (
    PatientId, VisitType, VisitReason, VisitDate,
    ProviderName, ProviderNPI, FacilityName,
    Status, PrimaryDiagnosisCode, PrimaryDiagnosisDescription,
    ChiefComplaint, VisitSummary, FollowUpInstructions,
    CreatedBy
)
VALUES
-- John's Past Visit
('11111111-1111-1111-1111-111111111111', 'Office Visit', 'Annual Physical', '2025-09-15',
 'Dr. Johnson', '1234567890', 'St. Mary''s Hospital',
 'completed', 'Z00.00', 'Routine general health check',
 'Annual checkup', 'All vitals normal. Continue current wellness routine.',
 'Schedule follow-up in 1 year. Maintain healthy diet and exercise.',
 'system'),

-- Emma's Upcoming Visit
('33333333-3333-3333-3333-333333333333', 'Pediatric Visit', 'Asthma Follow-up', '2025-10-22',
 'Dr. Martinez', '9876543210', 'Children''s Clinic',
 'scheduled', NULL, NULL,
 'Routine asthma checkup', NULL, NULL,
 'system');

PRINT '✓ 2 Visits added (1 past, 1 upcoming)'
GO

-- =============================================
-- AUDIT LOG ENTRIES
-- =============================================
INSERT INTO patient.AuditLog (
    EventType, EventCategory, EventAction, EventOutcome,
    ActorId, ActorType, ActorName, ActorRole,
    TargetType, TargetId, PatientId,
    IPAddress, EventDescription,
    IsAuthorized, HasValidConsent
)
VALUES
('PatientView', 'DataAccess', 'View', 'Success',
 'john.smith@email.com', 'Patient', 'John Smith', 'Patient',
 'Patient', '11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111',
 '192.168.1.100', 'Patient viewed own profile',
 1, 1),

('AllergyCreate', 'ClinicalData', 'Create', 'Success',
 'john.smith@email.com', 'Patient', 'John Smith', 'Patient',
 'Allergy', NULL, '33333333-3333-3333-3333-333333333333',
 '192.168.1.100', 'Added critical peanut allergy for Emma',
 1, 1),

('QRCodeGenerate', 'Consent', 'Create', 'Success',
 'john.smith@email.com', 'Patient', 'John Smith', 'Patient',
 'Consent', @ConsentId2, '11111111-1111-1111-1111-111111111111',
 '192.168.1.100', 'Generated QR code for hospital intake',
 1, 1);

PRINT '✓ Sample audit log entries added'
GO

PRINT ''
PRINT '========================================='
PRINT 'COMPREHENSIVE TEST DATA LOADED'
PRINT '========================================='
PRINT ''
PRINT 'Patients Created:'
PRINT '  ✓ Smith Family (4 members)'
PRINT '    - John Smith (Primary, Male, 38)'
PRINT '    - Jane Smith (Spouse, Female, 36)'
PRINT '    - Emma Smith (Daughter, Female, 8)'
PRINT '    - Oliver Smith (Son, Male, 5)'
PRINT '  ✓ Maria Garcia (Independent, Female, 30)'
PRINT '  ✓ Robert Johnson (Senior, Male, 73)'
PRINT ''
PRINT 'Clinical Data:'
PRINT '  ✓ 3 Critical Allergies (Penicillin, Peanuts, Aspirin)'
PRINT '  ✓ 3 Medications (Lisinopril, Albuterol, Multivitamin)'
PRINT '  ✓ 2 Conditions (Asthma, Hypertension)'
PRINT '  ✓ 3 Immunizations (COVID-19 x2, Flu)'
PRINT ''
PRINT 'Insurance:'
PRINT '  ✓ 2 Policies (BCBS Family, Aetna Individual)'
PRINT '  ✓ 1 Claim (Annual physical - approved)'
PRINT ''
PRINT 'Consents & Audit:'
PRINT '  ✓ 2 Consents (Treatment, QR Code)'
PRINT '  ✓ 3 Audit log entries'
PRINT ''
PRINT 'Visits:'
PRINT '  ✓ 2 Visits (1 completed, 1 upcoming)'
PRINT ''
PRINT 'You can now:'
PRINT '  • View dashboard: EXEC patient.usp_Dashboard_GetOverview ''11111111-1111-1111-1111-111111111111'''
PRINT '  • Get full profile: EXEC patient.usp_Patient_GetFullProfile ''11111111-1111-1111-1111-111111111111'''
PRINT '  • Check coverage: EXEC patient.usp_Insurance_GetCoverageSummary ''11111111-1111-1111-1111-111111111111'''
PRINT '  • View audit: EXEC patient.usp_Audit_GetPatientAccessHistory ''11111111-1111-1111-1111-111111111111'''
PRINT '========================================='
GO
