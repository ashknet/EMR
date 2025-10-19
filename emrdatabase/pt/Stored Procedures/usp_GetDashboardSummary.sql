
CREATE PROCEDURE pt.usp_GetDashboardSummary
    @PatientId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Patient basic info
    SELECT 
        p.PatientId,
        p.FirstName + ' ' + p.LastName AS PatientName,
        p.Email,
        p.PhoneNumber
    FROM pt.Patients p
    WHERE p.PatientId = @PatientId AND p.IsDeleted = 0;
    
    -- Primary insurance
    SELECT TOP 1
        i.InsuranceId,
        i.PayerName,
        i.PlanName,
        i.MemberId,
        i.EffectiveDate,
        i.ExpirationDate
    FROM pt.Insurances i
    WHERE i.PatientId = @PatientId 
      AND i.IsActive = 1
      AND i.Priority = 1
    ORDER BY i.EffectiveDate DESC;
    
    -- Counts
    SELECT 
        (SELECT COUNT(*) FROM pt.FamilyRelations WHERE PatientId = @PatientId) AS FamilyMemberCount,
        (SELECT COUNT(*) FROM pt.Allergies WHERE PatientId = @PatientId) AS AllergyCount,
        (SELECT COUNT(*) FROM pt.Medications WHERE PatientId = @PatientId AND IsActive = 1) AS MedicationCount,
        (SELECT COUNT(*) FROM pt.Documents WHERE PatientId = @PatientId) AS DocumentCount;
END
GO
GRANT EXECUTE
    ON OBJECT::[pt].[usp_GetDashboardSummary] TO PUBLIC
    AS [dbo];

