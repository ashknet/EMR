-- ================================================================
-- 11_PerformanceIndexes.sql
-- Description: Performance indexes for quick access patterns
-- Supports user-friendly navigation with section-specific updates
-- ================================================================

USE HealthcarePlatform;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

--------------------------------------------------------------------
-- 1. Patient table indexes for quick lookups
--------------------------------------------------------------------

-- Index for email lookup (login/authentication)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Patients_Email' AND object_id = OBJECT_ID('pt.Patients'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Patients_Email 
    ON pt.Patients(Email) 
    WHERE IsDeleted = 0;
    PRINT 'Created index IX_Patients_Email';
END
GO

-- Index for phone number lookup
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Patients_PhoneNumber' AND object_id = OBJECT_ID('pt.Patients'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Patients_PhoneNumber 
    ON pt.Patients(PhoneNumber) 
    WHERE IsDeleted = 0;
    PRINT 'Created index IX_Patients_PhoneNumber';
END
GO

-- Index for name search
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Patients_Name' AND object_id = OBJECT_ID('pt.Patients'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Patients_Name 
    ON pt.Patients(LastName, FirstName) 
    INCLUDE (MiddleName, DateOfBirth)
    WHERE IsDeleted = 0;
    PRINT 'Created index IX_Patients_Name';
END
GO

-- Index for active patients
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Patients_Active' AND object_id = OBJECT_ID('pt.Patients'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Patients_Active 
    ON pt.Patients(IsActive, IsDeleted) 
    INCLUDE (PatientId, FirstName, LastName, Email);
    PRINT 'Created index IX_Patients_Active';
END
GO

--------------------------------------------------------------------
-- 2. Allergy indexes for profile completeness check
--------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Allergies_PatientId_Active' AND object_id = OBJECT_ID('pt.Allergies'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Allergies_PatientId_Active 
    ON pt.Allergies(PatientId) 
    INCLUDE (AllergenName, Severity, ClinicalStatus);
    PRINT 'Created index IX_Allergies_PatientId_Active';
END
GO

--------------------------------------------------------------------
-- 3. Insurance indexes for profile completeness check
--------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Insurances_PatientId_Active' AND object_id = OBJECT_ID('pt.Insurances'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Insurances_PatientId_Active 
    ON pt.Insurances(PatientId, IsActive) 
    INCLUDE (PayerName, MemberId, Priority);
    PRINT 'Created index IX_Insurances_PatientId_Active';
END
GO

--------------------------------------------------------------------
-- 4. Medication indexes for medical history page
--------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Medications_PatientId_Active' AND object_id = OBJECT_ID('pt.Medications'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Medications_PatientId_Active 
    ON pt.Medications(PatientId, IsActive) 
    INCLUDE (MedicationName, Dosage, Frequency);
    PRINT 'Created index IX_Medications_PatientId_Active';
END
GO

--------------------------------------------------------------------
-- 5. PatientSurgeries index
--------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PatientSurgeries_PatientId' AND object_id = OBJECT_ID('pt.PatientSurgeries'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_PatientSurgeries_PatientId 
    ON pt.PatientSurgeries(PatientId) 
    INCLUDE (SurgeryType, SurgeryDate, Notes);
    PRINT 'Created index IX_PatientSurgeries_PatientId';
END
GO

--------------------------------------------------------------------
-- 6. PatientHospitalizations index
--------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PatientHospitalizations_PatientId' AND object_id = OBJECT_ID('pt.PatientHospitalizations'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_PatientHospitalizations_PatientId 
    ON pt.PatientHospitalizations(PatientId) 
    INCLUDE (HospitalName, Reason, AdmissionDate, DischargeDate);
    PRINT 'Created index IX_PatientHospitalizations_PatientId';
END
GO

--------------------------------------------------------------------
-- 7. FamilyMedicalHistory index
--------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_FamilyMedicalHistory_PatientId' AND object_id = OBJECT_ID('pt.FamilyMedicalHistory'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_FamilyMedicalHistory_PatientId 
    ON pt.FamilyMedicalHistory(PatientId) 
    INCLUDE (Relative, Condition, Notes);
    PRINT 'Created index IX_FamilyMedicalHistory_PatientId';
END
GO

--------------------------------------------------------------------
-- 8. PatientChronicConditions index
--------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PatientChronicConditions_PatientId' AND object_id = OBJECT_ID('pt.PatientChronicConditions'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_PatientChronicConditions_PatientId 
    ON pt.PatientChronicConditions(PatientId) 
    INCLUDE (ConditionId, DiagnosedDate);
    PRINT 'Created index IX_PatientChronicConditions_PatientId';
END
GO

--------------------------------------------------------------------
-- 9. AuditLog indexes for tracking
--------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AuditLogs_PatientId_Timestamp' AND object_id = OBJECT_ID('pt.AuditLogs'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_AuditLogs_PatientId_Timestamp 
    ON pt.AuditLogs(PatientId, EventTimestamp DESC) 
    INCLUDE (EventType, Action, ActorName);
    PRINT 'Created index IX_AuditLogs_PatientId_Timestamp';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AuditLogs_EventType' AND object_id = OBJECT_ID('pt.AuditLogs'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_AuditLogs_EventType 
    ON pt.AuditLogs(EventType, EventTimestamp DESC);
    PRINT 'Created index IX_AuditLogs_EventType';
END
GO

--------------------------------------------------------------------
-- 10. FamilyRelations index for family page
--------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_FamilyRelations_PatientId' AND object_id = OBJECT_ID('pt.FamilyRelations'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_FamilyRelations_PatientId 
    ON pt.FamilyRelations(PatientId) 
    INCLUDE (RelatedPatientId, RelationshipType, IsGuardian, CanViewRecords);
    PRINT 'Created index IX_FamilyRelations_PatientId';
END
GO

--------------------------------------------------------------------
-- 11. PatientAddresses index
--------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PatientAddresses_PatientId' AND object_id = OBJECT_ID('pt.PatientAddresses'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_PatientAddresses_PatientId 
    ON pt.PatientAddresses(PatientId) 
    INCLUDE (AddressTypeId, Line1, City, State, PostalCode);
    PRINT 'Created index IX_PatientAddresses_PatientId';
END
GO

--------------------------------------------------------------------
-- 12. PatientPhones index
--------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PatientPhones_PatientId' AND object_id = OBJECT_ID('pt.PatientPhones'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_PatientPhones_PatientId 
    ON pt.PatientPhones(PatientId) 
    INCLUDE (PhoneTypeId, PhoneNumber);
    PRINT 'Created index IX_PatientPhones_PatientId';
END
GO

--------------------------------------------------------------------
-- 13. EmergencyContacts index
--------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_EmergencyContacts_PatientId' AND object_id = OBJECT_ID('pt.EmergencyContacts'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_EmergencyContacts_PatientId 
    ON pt.EmergencyContacts(PatientId) 
    INCLUDE (Name, RelationshipTypeId, Phone);
    PRINT 'Created index IX_EmergencyContacts_PatientId';
END
GO

--------------------------------------------------------------------
-- 14. InsurancePolicies index
--------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_InsurancePolicies_PatientId' AND object_id = OBJECT_ID('pt.InsurancePolicies'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_InsurancePolicies_PatientId 
    ON pt.InsurancePolicies(PatientId) 
    INCLUDE (ProviderId, PolicyNumber, GroupNumber);
    PRINT 'Created index IX_InsurancePolicies_PatientId';
END
GO

--------------------------------------------------------------------
-- 15. SocialHistory index
--------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SocialHistory_PatientId' AND object_id = OBJECT_ID('pt.SocialHistory'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_SocialHistory_PatientId 
    ON pt.SocialHistory(PatientId);
    PRINT 'Created index IX_SocialHistory_PatientId';
END
GO

--------------------------------------------------------------------
-- 16. LegalConsents index
--------------------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LegalConsents_PatientId' AND object_id = OBJECT_ID('pt.LegalConsents'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_LegalConsents_PatientId 
    ON pt.LegalConsents(PatientId);
    PRINT 'Created index IX_LegalConsents_PatientId';
END
GO

--------------------------------------------------------------------
-- Update statistics for optimal query plans
--------------------------------------------------------------------

UPDATE STATISTICS pt.Patients WITH FULLSCAN;
UPDATE STATISTICS pt.Allergies WITH FULLSCAN;
UPDATE STATISTICS pt.Insurances WITH FULLSCAN;
UPDATE STATISTICS pt.Medications WITH FULLSCAN;
UPDATE STATISTICS pt.PatientSurgeries WITH FULLSCAN;
UPDATE STATISTICS pt.PatientHospitalizations WITH FULLSCAN;
UPDATE STATISTICS pt.FamilyMedicalHistory WITH FULLSCAN;
UPDATE STATISTICS pt.PatientChronicConditions WITH FULLSCAN;

PRINT 'Statistics updated for all tables';
GO

PRINT 'Performance indexes created successfully';
GO

