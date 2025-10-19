
CREATE PROCEDURE pt.usp_GetProfileCompleteness
    @PatientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @HasBasicInfo BIT = 0;
    DECLARE @HasContactInfo BIT = 0;
    DECLARE @HasEmergencyContact BIT = 0;
    DECLARE @HasInsurance BIT = 0;
    DECLARE @HasAllergies BIT = 0;
    DECLARE @IsComplete BIT = 0;
    
    -- Check if patient exists
    IF NOT EXISTS (SELECT 1 FROM pt.Patients WHERE PatientId = @PatientId AND IsDeleted = 0)
    BEGIN
        -- Return error result
        SELECT NULL AS PatientId, 0 AS IsComplete, 'Patient not found' AS ErrorMessage;
        RETURN;
    END
    
    -- Check basic info
    SELECT @HasBasicInfo = CASE 
        WHEN FirstName IS NOT NULL AND FirstName <> ''
         AND LastName IS NOT NULL AND LastName <> ''
         AND Email IS NOT NULL AND Email <> ''
        THEN 1 ELSE 0 END
    FROM pt.Patients
    WHERE PatientId = @PatientId;
    
    -- Check contact info
    SELECT @HasContactInfo = CASE 
        WHEN PhoneNumber IS NOT NULL AND PhoneNumber <> ''
         AND AddressLine1 IS NOT NULL AND AddressLine1 <> ''
        THEN 1 ELSE 0 END
    FROM pt.Patients
    WHERE PatientId = @PatientId;
    
    -- Check emergency contact
    SELECT @HasEmergencyContact = CASE 
        WHEN EmergencyContactName IS NOT NULL AND EmergencyContactName <> ''
         AND EmergencyContactPhone IS NOT NULL AND EmergencyContactPhone <> ''
        THEN 1 ELSE 0 END
    FROM pt.Patients
    WHERE PatientId = @PatientId;
    
    -- Check insurance (optimized with index)
    IF EXISTS (SELECT 1 FROM pt.Insurances WHERE PatientId = @PatientId AND IsActive = 1)
        SET @HasInsurance = 1;
    
    -- Check allergies (optimized with index)
    IF EXISTS (SELECT 1 FROM pt.Allergies WHERE PatientId = @PatientId)
        SET @HasAllergies = 1;
    
    -- Calculate overall completeness
    SET @IsComplete = CASE 
        WHEN @HasBasicInfo = 1 AND @HasContactInfo = 1 
         AND @HasEmergencyContact = 1 AND @HasInsurance = 1 
         AND @HasAllergies = 1
        THEN 1 ELSE 0 END;
    
    -- Return result
    SELECT 
        @PatientId AS PatientId,
        @IsComplete AS IsComplete,
        @HasBasicInfo AS HasBasicInfo,
        @HasContactInfo AS HasContactInfo,
        @HasEmergencyContact AS HasEmergencyContact,
        @HasInsurance AS HasInsurance,
        @HasAllergies AS HasAllergies;
END
GO
GRANT EXECUTE
    ON OBJECT::[pt].[usp_GetProfileCompleteness] TO PUBLIC
    AS [dbo];

