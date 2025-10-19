-- Sample Data for Healthcare Platform
-- Creates a complete family with medical records, insurance, and visits

USE HealthcarePlatform;
GO

-- Sample Patient 1: John Doe (Primary)
DECLARE @JohnDoeId UNIQUEIDENTIFIER = NEWID();
INSERT INTO pt.Patients (PatientId, FirstName, MiddleName, LastName, DateOfBirth, Gender, Email, PhoneNumber, 
    AddressLine1, City, State, ZipCode, Country, Race, Ethnicity, PreferredLanguage, MaritalStatus,
    EmergencyContactName, EmergencyContactPhone, EmergencyContactRelationship,
    CreatedAt, CreatedBy, IsActive)
VALUES (@JohnDoeId, 'John', 'Michael', 'Doe', '1980-05-15', 'Male', 'john.doe@example.com', '555-0101',
    '123 Main Street', 'Springfield', 'IL', '62701', 'USA', 'White', 'Not Hispanic or Latino', 'English', 'Married',
    'Jane Doe', '555-0102', 'Spouse',
    GETUTCDATE(), 'system', 1);

-- Sample Patient 2: Jane Doe (Spouse)
DECLARE @JaneDoeId UNIQUEIDENTIFIER = NEWID();
INSERT INTO pt.Patients (PatientId, FirstName, MiddleName, LastName, DateOfBirth, Gender, Email, PhoneNumber,
    AddressLine1, City, State, ZipCode, Country, Race, Ethnicity, PreferredLanguage, MaritalStatus,
    EmergencyContactName, EmergencyContactPhone, EmergencyContactRelationship,
    CreatedAt, CreatedBy, IsActive)
VALUES (@JaneDoeId, 'Jane', 'Elizabeth', 'Doe', '1982-08-22', 'Female', 'jane.doe@example.com', '555-0102',
    '123 Main Street', 'Springfield', 'IL', '62701', 'USA', 'White', 'Not Hispanic or Latino', 'English', 'Married',
    'John Doe', '555-0101', 'Spouse',
    GETUTCDATE(), 'system', 1);

-- Sample Patient 3: Emily Doe (Child)
DECLARE @EmilyDoeId UNIQUEIDENTIFIER = NEWID();
INSERT INTO pt.Patients (PatientId, FirstName, MiddleName, LastName, DateOfBirth, Gender, Email, PhoneNumber,
    AddressLine1, City, State, ZipCode, Country, Race, Ethnicity, PreferredLanguage, MaritalStatus,
    EmergencyContactName, EmergencyContactPhone, EmergencyContactRelationship,
    CreatedAt, CreatedBy, IsActive)
VALUES (@EmilyDoeId, 'Emily', 'Grace', 'Doe', '2015-03-10', 'Female', 'emily.doe@example.com', '555-0101',
    '123 Main Street', 'Springfield', 'IL', '62701', 'USA', 'White', 'Not Hispanic or Latino', 'English', 'Single',
    'Jane Doe', '555-0102', 'Mother',
    GETUTCDATE(), 'system', 1);

-- Family Relations
INSERT INTO pt.FamilyRelations (RelationId, PatientId, RelatedPatientId, RelationType, IsGuardian, CanViewRecords, CanManageRecords, CreatedAt, CreatedBy, IsActive)
VALUES 
    (NEWID(), @JohnDoeId, @JaneDoeId, 'Spouse', 0, 1, 1, GETUTCDATE(), 'system', 1),
    (NEWID(), @JohnDoeId, @EmilyDoeId, 'Child', 1, 1, 1, GETUTCDATE(), 'system', 1),
    (NEWID(), @JaneDoeId, @JohnDoeId, 'Spouse', 0, 1, 1, GETUTCDATE(), 'system', 1),
    (NEWID(), @JaneDoeId, @EmilyDoeId, 'Child', 1, 1, 1, GETUTCDATE(), 'system', 1);

-- Allergies for John Doe
INSERT INTO pt.Allergies (AllergyId, PatientId, AllergenName, AllergenType, AllergenCode, CodeSystem, Severity, ClinicalStatus, Reaction, OnsetDate, CreatedAt, CreatedBy, IsActive)
VALUES 
    (NEWID(), @JohnDoeId, 'Penicillin', 'Drug', '70618', 'RxNorm', 'Severe', 'Active', 'Anaphylaxis', '1995-06-10', GETUTCDATE(), 'system', 1),
    (NEWID(), @JohnDoeId, 'Peanuts', 'Food', '256349004', 'SNOMED CT', 'Moderate', 'Active', 'Hives, difficulty breathing', '1985-01-01', GETUTCDATE(), 'system', 1);

-- Allergies for Emily Doe
INSERT INTO pt.Allergies (AllergyId, PatientId, AllergenName, AllergenType, AllergenCode, CodeSystem, Severity, ClinicalStatus, Reaction, OnsetDate, CreatedAt, CreatedBy, IsActive)
VALUES 
    (NEWID(), @EmilyDoeId, 'Dust Mites', 'Environment', '260147004', 'SNOMED CT', 'Mild', 'Active', 'Sneezing, runny nose', '2018-09-01', GETUTCDATE(), 'system', 1);

-- Conditions for John Doe
INSERT INTO pt.Conditions (ConditionId, PatientId, ConditionName, ConditionCode, CodeSystem, ClinicalStatus, Severity, OnsetDate, CreatedAt, CreatedBy, IsActive)
VALUES 
    (NEWID(), @JohnDoeId, 'Hypertension', 'I10', 'ICD-10', 'Active', 'Moderate', '2018-03-15', GETUTCDATE(), 'system', 1),
    (NEWID(), @JohnDoeId, 'Type 2 Diabetes Mellitus', 'E11', 'ICD-10', 'Active', 'Moderate', '2020-07-22', GETUTCDATE(), 'system', 1);

-- Immunizations for Emily Doe
INSERT INTO pt.Immunizations (ImmunizationId, PatientId, VaccineName, VaccineCode, CodeSystem, AdministeredDate, AdministeredBy, Status, ProviderName, FacilityName, DoseNumber, SeriesDoses, CreatedAt, CreatedBy, IsActive)
VALUES 
    (NEWID(), @EmilyDoeId, 'DTaP', '20', 'CVX', '2015-05-10', 'Dr. Smith', 'Completed', 'Dr. Sarah Smith', 'Springfield Pediatrics', 1, 5, GETUTCDATE(), 'system', 1),
    (NEWID(), @EmilyDoeId, 'MMR', '03', 'CVX', '2016-03-10', 'Dr. Smith', 'Completed', 'Dr. Sarah Smith', 'Springfield Pediatrics', 1, 2, GETUTCDATE(), 'system', 1),
    (NEWID(), @EmilyDoeId, 'Varicella', '21', 'CVX', '2016-03-10', 'Dr. Smith', 'Completed', 'Dr. Sarah Smith', 'Springfield Pediatrics', 1, 2, GETUTCDATE(), 'system', 1),
    (NEWID(), @EmilyDoeId, 'COVID-19', '207', 'CVX', '2021-12-15', 'Dr. Johnson', 'Completed', 'Dr. Michael Johnson', 'Springfield Health Center', 1, 2, GETUTCDATE(), 'system', 1);

-- Insurance for John Doe (Primary)
DECLARE @JohnInsuranceId UNIQUEIDENTIFIER = NEWID();
INSERT INTO pt.Insurances (InsuranceId, PatientId, PayerName, PlanName, PlanType, MemberId, GroupNumber, GroupName,
    EffectiveDate, IsPrimary, Priority, SubscriberRelationship, SubscriberName,
    PayerPhone, PayerWebsite, CustomerServicePhone,
    Deductible, DeductibleMet, OutOfPocketMax, OutOfPocketMet, Copay, CoinsurancePercentage,
    Status, IsVerified, LastVerifiedDate, CreatedAt, CreatedBy, IsActive)
VALUES (@JohnInsuranceId, @JohnDoeId, 'Blue Cross Blue Shield', 'PPO Gold Plan', 'PPO', 'BCBS123456789', 'GRP001', 'ABC Corporation',
    '2024-01-01', 1, 1, 'Self', 'John Michael Doe',
    '1-800-555-BCBS', 'https://www.bcbs.com', '1-800-555-2227',
    2000.00, 450.00, 6000.00, 1250.00, 30.00, 20.00,
    'Active', 1, GETUTCDATE(), GETUTCDATE(), 'system', 1);

-- Insurance for Jane Doe (Covered under John's plan)
INSERT INTO pt.Insurances (InsuranceId, PatientId, PayerName, PlanName, PlanType, MemberId, GroupNumber, GroupName,
    EffectiveDate, IsPrimary, Priority, SubscriberRelationship, SubscriberName,
    PayerPhone, PayerWebsite, CustomerServicePhone,
    Deductible, DeductibleMet, OutOfPocketMax, OutOfPocketMet, Copay, CoinsurancePercentage,
    Status, IsVerified, LastVerifiedDate, CreatedAt, CreatedBy, IsActive)
VALUES (NEWID(), @JaneDoeId, 'Blue Cross Blue Shield', 'PPO Gold Plan', 'PPO', 'BCBS123456789-02', 'GRP001', 'ABC Corporation',
    '2024-01-01', 1, 1, 'Spouse', 'John Michael Doe',
    '1-800-555-BCBS', 'https://www.bcbs.com', '1-800-555-2227',
    2000.00, 650.00, 6000.00, 1800.00, 30.00, 20.00,
    'Active', 1, GETUTCDATE(), GETUTCDATE(), 'system', 1);

-- Insurance for Emily Doe (Covered under John's plan)
INSERT INTO pt.Insurances (InsuranceId, PatientId, PayerName, PlanName, PlanType, MemberId, GroupNumber, GroupName,
    EffectiveDate, IsPrimary, Priority, SubscriberRelationship, SubscriberName,
    PayerPhone, PayerWebsite, CustomerServicePhone,
    Deductible, DeductibleMet, OutOfPocketMax, OutOfPocketMet, Copay, CoinsurancePercentage,
    Status, IsVerified, LastVerifiedDate, CreatedAt, CreatedBy, IsActive)
VALUES (NEWID(), @EmilyDoeId, 'Blue Cross Blue Shield', 'PPO Gold Plan', 'PPO', 'BCBS123456789-03', 'GRP001', 'ABC Corporation',
    '2024-01-01', 1, 1, 'Child', 'John Michael Doe',
    '1-800-555-BCBS', 'https://www.bcbs.com', '1-800-555-2227',
    2000.00, 120.00, 6000.00, 320.00, 20.00, 20.00,
    'Active', 1, GETUTCDATE(), GETUTCDATE(), 'system', 1);

-- Encounters for John Doe
DECLARE @JohnEncounter1 UNIQUEIDENTIFIER = NEWID();
INSERT INTO pt.Encounters (EncounterId, PatientId, EncounterType, Status, ActualStartDate, EndDate, DurationMinutes,
    ProviderName, ProviderSpecialty, ProviderNpi, FacilityName, Department,
    ChiefComplaint, ReasonForVisit, PrimaryDiagnosisCode, PrimaryDiagnosisDescription,
    DischargeDisposition, CreatedAt, CreatedBy, IsActive)
VALUES (@JohnEncounter1, @JohnDoeId, 'Outpatient', 'Finished', '2024-09-15 10:00:00', '2024-09-15 10:45:00', 45,
    'Dr. Robert Wilson', 'Internal Medicine', '1234567890', 'Springfield Medical Center', 'Primary Care',
    'Follow-up for hypertension', 'Routine follow-up', 'I10', 'Essential (primary) hypertension',
    'Home', GETUTCDATE(), 'system', 1);

-- Upcoming encounter for Jane Doe
INSERT INTO pt.Encounters (EncounterId, PatientId, EncounterType, Status, PlannedStartDate,
    ProviderName, ProviderSpecialty, FacilityName, Department,
    ReasonForVisit, CreatedAt, CreatedBy, IsActive)
VALUES (NEWID(), @JaneDoeId, 'Outpatient', 'Planned', '2025-02-20 14:00:00',
    'Dr. Emily Chen', 'Obstetrics & Gynecology', 'Springfield Women''s Health', 'OB/GYN',
    'Annual wellness visit', GETUTCDATE(), 'system', 1);

-- Upcoming encounter for Emily Doe
INSERT INTO pt.Encounters (EncounterId, PatientId, EncounterType, Status, PlannedStartDate,
    ProviderName, ProviderSpecialty, FacilityName, Department,
    ReasonForVisit, CreatedAt, CreatedBy, IsActive)
VALUES (NEWID(), @EmilyDoeId, 'Outpatient', 'Planned', '2025-01-25 09:00:00',
    'Dr. Sarah Smith', 'Pediatrics', 'Springfield Pediatrics', 'Pediatrics',
    '10-year well-child visit', GETUTCDATE(), 'system', 1);

-- Sample Consent for John Doe
DECLARE @ConsentId UNIQUEIDENTIFIER = NEWID();
INSERT INTO pt.Consents (ConsentId, PatientId, ConsentType, Status, Scope,
    AllowDemographics, AllowAllergies, AllowConditions, AllowImmunizations, AllowMedications,
    RecipientName, RecipientOrganization, RecipientType,
    GrantedDate, ExpiryDate, Purpose, SignatureObtained, SignatureDate,
    ShareToken, TokenExpiryDate, MaxAccessCount,
    CreatedAt, CreatedBy)
VALUES (@ConsentId, @JohnDoeId, 'Share', 'Active', 'Medical',
    1, 1, 1, 1, 1,
    'Dr. Jennifer Martinez', 'Springfield Specialist Group', 'Provider',
    GETUTCDATE(), DATEADD(YEAR, 1, GETUTCDATE()), 'Referral to specialist', 1, GETUTCDATE(),
    'ABC123XYZ789DEF456', DATEADD(DAY, 30, GETUTCDATE()), 10,
    GETUTCDATE(), 'system');

-- Notifications for John Doe
INSERT INTO pt.Notifications (NotificationId, PatientId, NotificationType, Category, Title, Message, ActionUrl, ActionLabel, Priority, CreatedAt, CreatedBy)
VALUES 
    (NEWID(), @JohnDoeId, 'Reminder', 'Appointment', 'Upcoming Appointment Reminder', 'You have an upcoming appointment with Dr. Wilson on Feb 15, 2025 at 10:00 AM', '/appointments', 'View Details', 'Normal', GETUTCDATE(), 'system'),
    (NEWID(), @JohnDoeId, 'Info', 'Insurance', 'Insurance Card Updated', 'Your insurance card for Blue Cross Blue Shield has been updated', '/insurance', 'View Insurance', 'Low', GETUTCDATE(), 'system');

-- Notifications for Jane Doe
INSERT INTO pt.Notifications (NotificationId, PatientId, NotificationType, Category, Title, Message, ActionUrl, ActionLabel, Priority, CreatedAt, CreatedBy)
VALUES 
    (NEWID(), @JaneDoeId, 'Reminder', 'Appointment', 'Annual Wellness Visit Due', 'Your annual wellness visit with Dr. Chen is scheduled for Feb 20, 2025', '/appointments', 'Confirm Appointment', 'Normal', GETUTCDATE(), 'system');

-- Sample Document for John Doe
INSERT INTO pt.Documents (DocumentId, PatientId, EncounterId, FileName, DocumentType, ContentType, FileSize, StorageLocation,
    Category, ServiceDate, Status, Description, OcrProcessed, CreatedAt, CreatedBy, IsActive)
VALUES (NEWID(), @JohnDoeId, @JohnEncounter1, 'lab-results-2024-09-15.pdf', 'Lab Result', 'application/pdf', 245678, 
    'https://storage.azure.com/documents/john-doe/lab-results-2024-09-15.pdf',
    'Clinical', '2024-09-15', 'Current', 'Comprehensive metabolic panel', 1, GETUTCDATE(), 'system', 1);

-- Sample Record Transfer
DECLARE @TransferId UNIQUEIDENTIFIER = NEWID();
INSERT INTO pt.RecordTransfers (TransferId, PatientId, ConsentId, TransferType, TransferMethod,
    RecipientOrganization, RecipientName, RecipientEmail,
    Purpose, RequestedBy, RequestedDate, Status, TrackingNumber,
    IncludeDemographics, IncludeAllergies, IncludeConditions, IncludeImmunizations,
    CreatedAt, CreatedBy)
VALUES (@TransferId, @JohnDoeId, @ConsentId, 'Outbound', 'FHIR',
    'Springfield Specialist Group', 'Dr. Jennifer Martinez', 'j.martinez@springfieldspecialists.com',
    'Referral for cardiology consultation', 'john.doe@example.com', GETUTCDATE(), 'Completed', 'TRK-20250113-AB12CD34',
    1, 1, 1, 1,
    GETUTCDATE(), 'system');

-- Transfer Audit
INSERT INTO pt.TransferAudits (AuditId, TransferId, Action, ActionDetails, ActionDate, ActorId, ActorName)
VALUES 
    (NEWID(), @TransferId, 'Requested', 'Transfer requested by patient', GETUTCDATE(), 'john.doe@example.com', 'John Doe'),
    (NEWID(), @TransferId, 'Bundle Generated', 'FHIR bundle created with 8 resources', DATEADD(MINUTE, 5, GETUTCDATE()), 'system', 'System'),
    (NEWID(), @TransferId, 'Completed', 'Transfer completed successfully', DATEADD(MINUTE, 10, GETUTCDATE()), 'system', 'System');

PRINT 'Sample data created successfully';
PRINT 'Family created: John Doe (Primary), Jane Doe (Spouse), Emily Doe (Child)';
PRINT 'Includes: Allergies, Conditions, Immunizations, Insurance, Encounters, Consents, and Transfers';
GO

