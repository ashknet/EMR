-- ================================================================
-- 09_SampleData.sql
-- Author: AI Assistant (o3-pro)
-- Description: Comprehensive sample data for all metadata tables
--              and patient records to support testing and development
-- ================================================================

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- Clear existing data (in reverse dependency order)
DELETE FROM dbo.PatientAllergies;
DELETE FROM dbo.PatientMedications;
DELETE FROM dbo.PatientChronicConditions;
DELETE FROM dbo.PatientSurgeries;
DELETE FROM dbo.PatientHospitalizations;
DELETE FROM dbo.PatientImmunizations;
DELETE FROM dbo.PatientTestResults;
DELETE FROM dbo.FamilyMedicalHistory;
DELETE FROM dbo.SocialHistory;
DELETE FROM dbo.LegalConsents;
DELETE FROM dbo.EmergencyContacts;
DELETE FROM dbo.InsurancePolicies;
DELETE FROM dbo.PatientPhones;
DELETE FROM dbo.PatientAddresses;
DELETE FROM dbo.Patients;

-- Seed additional metadata values
INSERT INTO dbo.Race (Name, SortOrder) VALUES
('American Indian or Alaska Native', 1),
('Asian', 2),
('Black or African American', 3),
('Native Hawaiian or Other Pacific Islander', 4),
('White', 5),
('Other', 6),
('Prefer not to answer', 7);

INSERT INTO dbo.[Language] (Name, SortOrder) VALUES
('English', 1),
('Spanish', 2),
('French', 3),
('German', 4),
('Italian', 5),
('Portuguese', 6),
('Chinese', 7),
('Japanese', 8),
('Korean', 9),
('Arabic', 10),
('Russian', 11),
('Other', 12);

INSERT INTO dbo.InsuranceProvider (Name, Phone, SortOrder) VALUES
('Blue Cross Blue Shield', '1-800-555-0123', 1),
('Aetna', '1-800-555-0124', 2),
('Cigna', '1-800-555-0125', 3),
('UnitedHealthcare', '1-800-555-0126', 4),
('Humana', '1-800-555-0127', 5),
('Kaiser Permanente', '1-800-555-0128', 6),
('Medicare', '1-800-555-0129', 7),
('Medicaid', '1-800-555-0130', 8),
('Tricare', '1-800-555-0131', 9),
('Other', 10);

INSERT INTO dbo.AllergyType (Name, SortOrder) VALUES
('Drug', 1),
('Food', 2),
('Environmental', 3),
('Latex', 4),
('Insect Sting', 5),
('Other', 6);

INSERT INTO dbo.Medication (Name, SortOrder) VALUES
('Lisinopril', 1),
('Metformin', 2),
('Atorvastatin', 3),
('Amlodipine', 4),
('Metoprolol', 5),
('Omeprazole', 6),
('Losartan', 7),
('Albuterol', 8),
('Gabapentin', 9),
('Hydrochlorothiazide', 10),
('Other', 11);

INSERT INTO dbo.ChronicCondition (Name, SortOrder) VALUES
('Hypertension', 1),
('Diabetes Type 1', 2),
('Diabetes Type 2', 3),
('Asthma', 4),
('COPD', 5),
('Heart Disease', 6),
('High Cholesterol', 7),
('Depression', 8),
('Anxiety', 9),
('Arthritis', 10),
('Osteoporosis', 11),
('Cancer', 12),
('Other', 13);

INSERT INTO dbo.SurgeryType (Name, SortOrder) VALUES
('Appendectomy', 1),
('Cholecystectomy', 2),
('Cesarean Section', 3),
('Heart Bypass', 4),
('Knee Replacement', 5),
('Hip Replacement', 6),
('Cataract Surgery', 7),
('Gallbladder Removal', 8),
('Hernia Repair', 9),
('Tonsillectomy', 10),
('Other', 11);

INSERT INTO dbo.ImmunizationType (Name, SortOrder) VALUES
('Tetanus', 1),
('Diphtheria', 2),
('Pertussis', 3),
('MMR', 4),
('Varicella', 5),
('Hepatitis A', 6),
('Hepatitis B', 7),
('HPV', 8),
('Meningococcal', 9),
('Pneumococcal', 10),
('Influenza', 11),
('COVID-19', 12),
('Other', 13);

INSERT INTO dbo.Condition (Name, SortOrder) VALUES
('Heart Disease', 1),
('Stroke', 2),
('Cancer', 3),
('Diabetes', 4),
('High Blood Pressure', 5),
('High Cholesterol', 6),
('Alzheimer''s Disease', 7),
('Parkinson''s Disease', 8),
('Multiple Sclerosis', 9),
('Epilepsy', 10),
('Mental Illness', 11),
('Substance Abuse', 12),
('Other', 13);

-- Sample Patient Records
INSERT INTO dbo.Patients (FirstName, LastName, DateOfBirth, GenderId, MaritalStatusId, RaceId, PrimaryLanguageId, Email, CreatedUtc) VALUES
('John', 'Smith', '1985-03-15', 1, 2, 5, 1, 'john.smith@email.com', SYSUTCDATETIME()),
('Sarah', 'Johnson', '1990-07-22', 2, 1, 5, 1, 'sarah.johnson@email.com', SYSUTCDATETIME()),
('Michael', 'Brown', '1978-11-08', 1, 2, 3, 1, 'michael.brown@email.com', SYSUTCDATETIME()),
('Emily', 'Davis', '1995-01-30', 2, 1, 2, 1, 'emily.davis@email.com', SYSUTCDATETIME()),
('David', 'Wilson', '1982-09-14', 1, 3, 5, 1, 'david.wilson@email.com', SYSUTCDATETIME());

-- Get patient IDs for foreign key relationships
DECLARE @Patient1 INT = (SELECT PatientId FROM dbo.Patients WHERE FirstName = 'John' AND LastName = 'Smith');
DECLARE @Patient2 INT = (SELECT PatientId FROM dbo.Patients WHERE FirstName = 'Sarah' AND LastName = 'Johnson');
DECLARE @Patient3 INT = (SELECT PatientId FROM dbo.Patients WHERE FirstName = 'Michael' AND LastName = 'Brown');

-- Sample Addresses
INSERT INTO dbo.PatientAddresses (PatientId, AddressTypeId, Line1, City, State, PostalCode, Country) VALUES
(@Patient1, 1, '123 Main St', 'Anytown', 'CA', '90210', 'USA'),
(@Patient1, 2, '456 Oak Ave', 'Anytown', 'CA', '90210', 'USA'),
(@Patient2, 1, '789 Pine Rd', 'Springfield', 'IL', '62701', 'USA'),
(@Patient3, 1, '321 Elm St', 'Portland', 'OR', '97201', 'USA');

-- Sample Phones
INSERT INTO dbo.PatientPhones (PatientId, PhoneTypeId, PhoneNumber) VALUES
(@Patient1, 1, '555-123-4567'),
(@Patient1, 2, '555-987-6543'),
(@Patient2, 1, '555-456-7890'),
(@Patient3, 1, '555-789-0123');

-- Sample Emergency Contacts
INSERT INTO dbo.EmergencyContacts (PatientId, RelationshipTypeId, Name, Phone) VALUES
(@Patient1, 2, 'Jane Smith', '555-111-2222'),
(@Patient2, 1, 'Tom Johnson', '555-333-4444'),
(@Patient3, 2, 'Mary Brown', '555-555-6666');

-- Sample Insurance Policies
INSERT INTO dbo.InsurancePolicies (PatientId, ProviderId, GroupNumber, PolicyNumber, PolicyHolderName) VALUES
(@Patient1, 1, 'GRP001', 'POL123456', 'John Smith'),
(@Patient2, 2, 'GRP002', 'POL789012', 'Sarah Johnson'),
(@Patient3, 3, 'GRP003', 'POL345678', 'Michael Brown');

-- Sample Social History
INSERT INTO dbo.SocialHistory (PatientId, SmokingStatusId, AlcoholUseId, Occupation, LivingSituationId) VALUES
(@Patient1, 1, 2, 'Software Engineer', 2),
(@Patient2, 1, 1, 'Teacher', 2),
(@Patient3, 2, 3, 'Construction Manager', 2);

-- Sample Legal Consents
INSERT INTO dbo.LegalConsents (PatientId, HipaaAgreed, ConsentToTreat, AssignmentOfBenefits, FinancialResponsibility, SignedOnUtc) VALUES
(@Patient1, 1, 1, 1, 1, SYSUTCDATETIME()),
(@Patient2, 1, 1, 1, 1, SYSUTCDATETIME()),
(@Patient3, 1, 1, 1, 1, SYSUTCDATETIME());

-- Sample Allergies
INSERT INTO dbo.PatientAllergies (PatientId, AllergyTypeId, CustomName, Notes) VALUES
(@Patient1, 1, 'Penicillin', 'Causes rash'),
(@Patient2, 2, 'Peanuts', 'Severe reaction'),
(@Patient3, 1, 'Sulfa', 'Nausea and vomiting');

-- Sample Medications
INSERT INTO dbo.PatientMedications (PatientId, MedicationId, Dosage, Frequency, Prescriber) VALUES
(@Patient1, 1, '10mg', 'Daily', 'Dr. Smith'),
(@Patient2, 2, '500mg', 'Twice daily', 'Dr. Johnson'),
(@Patient3, 3, '20mg', 'Daily', 'Dr. Brown');

-- Sample Chronic Conditions
INSERT INTO dbo.PatientChronicConditions (PatientId, ChronicConditionId) VALUES
(@Patient1, 1), -- Hypertension
(@Patient2, 2), -- Diabetes Type 1
(@Patient3, 1); -- Hypertension

PRINT 'Sample data inserted successfully';
GO
