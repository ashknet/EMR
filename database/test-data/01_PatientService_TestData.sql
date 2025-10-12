-- =============================================
-- Healthcare Platform - Patient Service Test Data
-- Sample patient records for development and testing
-- NOTE: All sensitive data is sample/fictional for testing only
-- =============================================

USE [HealthcarePlatform_Patient]
GO

-- Clear existing test data
DELETE FROM patient.PatientAccessLog
DELETE FROM patient.PatientNotes
DELETE FROM patient.PatientDocuments
DELETE FROM patient.Patients
GO

-- =============================================
-- Test Patient 1: Primary Account Holder - John Smith (Adult Male)
-- =============================================
DECLARE @JohnSmithId UNIQUEIDENTIFIER = NEWID()

INSERT INTO patient.Patients (
    Id, FirstName, MiddleName, LastName, DateOfBirth, Gender, BiologicalSex,
    Email, PhoneNumber, AlternatePhoneNumber,
    AddressLine1, AddressLine2, City, State, ZipCode, Country,
    SSNHash, MedicalRecordNumber,
    Race, Ethnicity, PreferredLanguage, MaritalStatus,
    EmergencyContactName, EmergencyContactRelationship, EmergencyContactPhone,
    PrimaryAccountHolderId, IsPrimaryAccountHolder, ProfileType,
    IsActive, IsVerified, VerifiedAt, IsMinor,
    HasConsentedToDataSharing, ConsentDate, ConsentVersion,
    CreatedBy, CreatedAt
)
VALUES (
    @JohnSmithId, 'John', 'Michael', 'Smith', '1985-06-15', 'Male', 'Male',
    'john.smith@email.com', '+1-555-0101', '+1-555-0102',
    '123 Main Street', 'Apt 4B', 'New York', 'NY', '10001', 'USA',
    HASHBYTES('SHA2_256', '123-45-6789'), 'MRN-001-2023',
    'White', 'Not Hispanic or Latino', 'English', 'Married',
    'Jane Smith', 'Spouse', '+1-555-0103',
    NULL, 1, 'Self',
    1, 1, GETUTCDATE(), 0,
    1, GETUTCDATE(), '1.0',
    'system', GETUTCDATE()
)

-- =============================================
-- Test Patient 2: Spouse - Jane Smith (Adult Female)
-- =============================================
DECLARE @JaneSmithId UNIQUEIDENTIFIER = NEWID()

INSERT INTO patient.Patients (
    Id, FirstName, MiddleName, LastName, PreferredName, DateOfBirth, Gender, BiologicalSex,
    Email, PhoneNumber,
    AddressLine1, City, State, ZipCode, Country,
    SSNHash, MedicalRecordNumber,
    Race, Ethnicity, PreferredLanguage, MaritalStatus,
    EmergencyContactName, EmergencyContactRelationship, EmergencyContactPhone,
    PrimaryAccountHolderId, IsPrimaryAccountHolder, ProfileType, RelationshipToPrimary,
    IsActive, IsVerified, VerifiedAt, IsMinor,
    HasConsentedToDataSharing, ConsentDate, ConsentVersion,
    CreatedBy, CreatedAt
)
VALUES (
    @JaneSmithId, 'Jane', 'Elizabeth', 'Smith', 'Jane', '1987-03-22', 'Female', 'Female',
    'jane.smith@email.com', '+1-555-0103',
    '123 Main Street', 'New York', 'NY', '10001', 'USA',
    HASHBYTES('SHA2_256', '987-65-4321'), 'MRN-002-2023',
    'White', 'Not Hispanic or Latino', 'English', 'Married',
    'John Smith', 'Spouse', '+1-555-0101',
    @JohnSmithId, 0, 'Spouse', 'Spouse',
    1, 1, GETUTCDATE(), 0,
    1, GETUTCDATE(), '1.0',
    'system', GETUTCDATE()
)

-- =============================================
-- Test Patient 3: Child - Emma Smith (Minor Female)
-- =============================================
DECLARE @EmmaSmithId UNIQUEIDENTIFIER = NEWID()

INSERT INTO patient.Patients (
    Id, FirstName, LastName, DateOfBirth, Gender, BiologicalSex,
    Email, PhoneNumber,
    AddressLine1, City, State, ZipCode, Country,
    MedicalRecordNumber,
    Race, Ethnicity, PreferredLanguage,
    EmergencyContactName, EmergencyContactRelationship, EmergencyContactPhone,
    PrimaryAccountHolderId, IsPrimaryAccountHolder, ProfileType, RelationshipToPrimary,
    IsActive, IsVerified, VerifiedAt, IsMinor,
    HasConsentedToDataSharing, ConsentDate, ConsentVersion,
    CreatedBy, CreatedAt
)
VALUES (
    @EmmaSmithId, 'Emma', 'Smith', '2015-09-10', 'Female', 'Female',
    NULL, NULL,
    '123 Main Street', 'New York', 'NY', '10001', 'USA',
    'MRN-003-2023',
    'White', 'Not Hispanic or Latino', 'English',
    'John Smith', 'Father', '+1-555-0101',
    @JohnSmithId, 0, 'Child', 'Daughter',
    1, 1, GETUTCDATE(), 1,
    0, NULL, NULL,
    'system', GETUTCDATE()
)

-- =============================================
-- Test Patient 4: Child - Oliver Smith (Minor Male)
-- =============================================
DECLARE @OliverSmithId UNIQUEIDENTIFIER = NEWID()

INSERT INTO patient.Patients (
    Id, FirstName, LastName, DateOfBirth, Gender, BiologicalSex,
    AddressLine1, City, State, ZipCode, Country,
    MedicalRecordNumber,
    Race, Ethnicity, PreferredLanguage,
    EmergencyContactName, EmergencyContactRelationship, EmergencyContactPhone,
    PrimaryAccountHolderId, IsPrimaryAccountHolder, ProfileType, RelationshipToPrimary,
    IsActive, IsVerified, VerifiedAt, IsMinor,
    HasConsentedToDataSharing, ConsentDate, ConsentVersion,
    CreatedBy, CreatedAt
)
VALUES (
    @OliverSmithId, 'Oliver', 'Smith', '2018-11-25', 'Male', 'Male',
    '123 Main Street', 'New York', 'NY', '10001', 'USA',
    'MRN-004-2023',
    'White', 'Not Hispanic or Latino', 'English',
    'John Smith', 'Father', '+1-555-0101',
    @JohnSmithId, 0, 'Child', 'Son',
    1, 1, GETUTCDATE(), 1,
    0, NULL, NULL,
    'system', GETUTCDATE()
)

-- =============================================
-- Test Patient 5: Independent Adult - Maria Garcia
-- =============================================
DECLARE @MariaGarciaId UNIQUEIDENTIFIER = NEWID()

INSERT INTO patient.Patients (
    Id, FirstName, MiddleName, LastName, DateOfBirth, Gender, BiologicalSex,
    Email, PhoneNumber,
    AddressLine1, City, State, ZipCode, Country,
    SSNHash, MedicalRecordNumber,
    Race, Ethnicity, PreferredLanguage, MaritalStatus,
    EmergencyContactName, EmergencyContactRelationship, EmergencyContactPhone,
    PrimaryAccountHolderId, IsPrimaryAccountHolder, ProfileType,
    IsActive, IsVerified, VerifiedAt, IsMinor,
    HasConsentedToDataSharing, ConsentDate, ConsentVersion,
    CreatedBy, CreatedAt
)
VALUES (
    @MariaGarciaId, 'Maria', 'Elena', 'Garcia', '1992-12-08', 'Female', 'Female',
    'maria.garcia@email.com', '+1-555-0201',
    '456 Oak Avenue', 'Los Angeles', 'CA', '90001', 'USA',
    HASHBYTES('SHA2_256', '456-78-9012'), 'MRN-005-2023',
    'Other', 'Hispanic or Latino', 'Spanish', 'Single',
    'Carlos Garcia', 'Father', '+1-555-0202',
    NULL, 1, 'Self',
    1, 1, GETUTCDATE(), 0,
    1, GETUTCDATE(), '1.0',
    'system', GETUTCDATE()
)

-- =============================================
-- Test Patient 6: Senior - Robert Johnson
-- =============================================
DECLARE @RobertJohnsonId UNIQUEIDENTIFIER = NEWID()

INSERT INTO patient.Patients (
    Id, FirstName, MiddleName, LastName, DateOfBirth, Gender, BiologicalSex,
    Email, PhoneNumber,
    AddressLine1, City, State, ZipCode, Country,
    SSNHash, MedicalRecordNumber, DriverLicenseNumber,
    Race, Ethnicity, PreferredLanguage, MaritalStatus,
    EmergencyContactName, EmergencyContactRelationship, EmergencyContactPhone,
    PrimaryAccountHolderId, IsPrimaryAccountHolder, ProfileType,
    IsActive, IsVerified, VerifiedAt, IsMinor,
    HasConsentedToDataSharing, ConsentDate, ConsentVersion,
    CreatedBy, CreatedAt
)
VALUES (
    @RobertJohnsonId, 'Robert', 'Lee', 'Johnson', '1950-04-20', 'Male', 'Male',
    'robert.johnson@email.com', '+1-555-0301',
    '789 Elm Street', 'Chicago', 'IL', '60601', 'USA',
    HASHBYTES('SHA2_256', '234-56-7890'), 'MRN-006-2023', 'D123-4567-8901',
    'Black or African American', 'Not Hispanic or Latino', 'English', 'Widowed',
    'Sarah Johnson', 'Daughter', '+1-555-0302',
    NULL, 1, 'Self',
    1, 1, GETUTCDATE(), 0,
    1, GETUTCDATE(), '1.0',
    'system', GETUTCDATE()
)

-- =============================================
-- Sample Patient Documents
-- =============================================
INSERT INTO patient.PatientDocuments (
    Id, PatientId, DocumentType, DocumentName, Description,
    BlobStorageUrl, FileType, FileSizeBytes,
    IsEncrypted, OCRProcessed,
    UploadedBy, UploadedAt, CreatedBy, CreatedAt
)
VALUES
-- John Smith's documents
(NEWID(), @JohnSmithId, 'Insurance', 'Health Insurance Card - Front.jpg', 'Blue Cross Blue Shield insurance card',
 'https://storage.blob.core.windows.net/documents/john-insurance-front.jpg', 'JPG', 524288,
 1, 1, 'john.smith@email.com', GETUTCDATE(), 'john.smith@email.com', GETUTCDATE()),

(NEWID(), @JohnSmithId, 'Lab', 'Blood Work Results - Annual Physical 2023.pdf', 'Complete blood count and metabolic panel',
 'https://storage.blob.core.windows.net/documents/john-bloodwork-2023.pdf', 'PDF', 1048576,
 1, 1, 'john.smith@email.com', GETUTCDATE(), 'john.smith@email.com', GETUTCDATE()),

-- Jane Smith's documents
(NEWID(), @JaneSmithId, 'Insurance', 'Health Insurance Card.pdf', 'Dependent insurance card',
 'https://storage.blob.core.windows.net/documents/jane-insurance.pdf', 'PDF', 327680,
 1, 1, 'jane.smith@email.com', GETUTCDATE(), 'jane.smith@email.com', GETUTCDATE()),

-- Emma Smith's documents
(NEWID(), @EmmaSmithId, 'Medical', 'Vaccination Record.pdf', 'Complete vaccination history',
 'https://storage.blob.core.windows.net/documents/emma-vaccinations.pdf', 'PDF', 450560,
 1, 1, 'john.smith@email.com', GETUTCDATE(), 'john.smith@email.com', GETUTCDATE()),

-- Maria Garcia's documents
(NEWID(), @MariaGarciaId, 'Imaging', 'Chest X-Ray 2023-08-15.dcm', 'Chest X-ray for routine checkup',
 'https://storage.blob.core.windows.net/documents/maria-xray-2023.dcm', 'DICOM', 5242880,
 1, 0, 'maria.garcia@email.com', GETUTCDATE(), 'maria.garcia@email.com', GETUTCDATE())

-- =============================================
-- Sample Patient Notes
-- =============================================
INSERT INTO patient.PatientNotes (
    Id, PatientId, NoteType, Title, Content,
    IsImportant, IsPrivate, Tags,
    ReminderDate, IsReminderSent,
    CreatedBy, CreatedAt
)
VALUES
-- John Smith's notes
(NEWID(), @JohnSmithId, 'Medical', 'Allergy to Penicillin', 
 'Severe allergic reaction to penicillin antibiotics. Use alternatives like azithromycin or cephalosporins with caution.',
 1, 1, 'allergy,medication,urgent',
 NULL, 0, 'john.smith@email.com', GETUTCDATE()),

(NEWID(), @JohnSmithId, 'Personal', 'Annual Physical Reminder', 
 'Schedule annual physical exam for September 2024. Last exam was September 2023.',
 0, 1, 'reminder,checkup',
 DATEADD(MONTH, 2, GETUTCDATE()), 0, 'john.smith@email.com', GETUTCDATE()),

-- Jane Smith's notes
(NEWID(), @JaneSmithId, 'Medical', 'Lactose Intolerance', 
 'Mild lactose intolerance. Avoid dairy or use lactase supplements.',
 0, 1, 'allergy,dietary',
 NULL, 0, 'jane.smith@email.com', GETUTCDATE()),

-- Emma Smith's notes
(NEWID(), @EmmaSmithId, 'Medical', 'Asthma Management', 
 'Mild asthma. Uses albuterol inhaler as needed. Keep rescue inhaler at school.',
 1, 1, 'chronic,medication,school',
 NULL, 0, 'john.smith@email.com', GETUTCDATE()),

-- Robert Johnson's notes
(NEWID(), @RobertJohnsonId, 'Medical', 'Hypertension Management', 
 'Taking lisinopril 10mg daily for blood pressure. Monitor BP weekly.',
 1, 1, 'chronic,medication,cardiovascular',
 NULL, 0, 'robert.johnson@email.com', GETUTCDATE())

-- =============================================
-- Sample Access Log Entries
-- =============================================
INSERT INTO patient.PatientAccessLog (
    Id, PatientId, AccessType, AccessedBy, AccessedByRole,
    IPAddress, HasValidConsent, IsAuthorized, AccessedAt
)
VALUES
(NEWID(), @JohnSmithId, 'View', 'john.smith@email.com', 'Patient', '192.168.1.100', 1, 1, GETUTCDATE()),
(NEWID(), @JohnSmithId, 'Update', 'john.smith@email.com', 'Patient', '192.168.1.100', 1, 1, DATEADD(HOUR, -2, GETUTCDATE())),
(NEWID(), @JaneSmithId, 'View', 'jane.smith@email.com', 'Patient', '192.168.1.101', 1, 1, DATEADD(DAY, -1, GETUTCDATE())),
(NEWID(), @EmmaSmithId, 'View', 'john.smith@email.com', 'Guardian', '192.168.1.100', 1, 1, DATEADD(DAY, -2, GETUTCDATE())),
(NEWID(), @MariaGarciaId, 'Create', 'maria.garcia@email.com', 'Patient', '192.168.1.102', 1, 1, DATEADD(DAY, -5, GETUTCDATE()))

PRINT '✓ Sample patient data inserted successfully'
PRINT '✓ Patients created: 6 (3 adults, 2 minors, 1 senior)'
PRINT '✓ Family group: Smith family (John, Jane, Emma, Oliver)'
PRINT '✓ Independent accounts: Maria Garcia, Robert Johnson'
PRINT '✓ Sample documents: 5 documents (insurance, lab, imaging, vaccination)'
PRINT '✓ Sample notes: 5 notes (allergies, reminders, conditions)'
PRINT '✓ Access log entries: 5 audit records'
GO
