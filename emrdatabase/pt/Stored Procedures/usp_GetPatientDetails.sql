
CREATE PROCEDURE pt.usp_GetPatientDetails
    @PatientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.PatientId,
        p.FirstName,
        p.MiddleName,
        p.LastName,
        p.Suffix,
        p.DateOfBirth,
        p.Gender,
        p.Email,
        p.PhoneNumber,
        p.AlternatePhone,
        p.AddressLine1,
        p.AddressLine2,
        p.City,
        p.State,
        p.ZipCode,
        p.Country,
        p.Race,
        p.Ethnicity,
        p.PreferredLanguage,
        p.MaritalStatus,
        p.EmergencyContactName,
        p.EmergencyContactPhone,
        p.EmergencyContactRelationship,
        p.InterpreterRequired,
        p.MobilityAssistance,
        p.ReligiousConsiderations,
        p.PrimaryCarePhysician,
        p.PCPPhoneNumber,
        p.ReferringPhysician,
        p.PreferredPharmacyName,
        p.PreferredPharmacyLocation,
        CASE WHEN p.SsnEncrypted IS NOT NULL AND p.SsnEncrypted <> '' THEN 1 ELSE 0 END AS HasSsn,
        p.CreatedAt,
        p.UpdatedAt
    FROM pt.Patients p
    WHERE p.PatientId = @PatientId 
      AND p.IsDeleted = 0;
END
GO
GRANT EXECUTE
    ON OBJECT::[pt].[usp_GetPatientDetails] TO PUBLIC
    AS [dbo];

