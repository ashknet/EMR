
CREATE PROCEDURE pt.usp_GetAllergiesBulk
    @PatientIds NVARCHAR(MAX) -- Comma-separated GUIDs
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Parse patient IDs into table
    DECLARE @PatientIdTable TABLE (PatientId UNIQUEIDENTIFIER);
    
    INSERT INTO @PatientIdTable (PatientId)
    SELECT CAST(value AS UNIQUEIDENTIFIER)
    FROM STRING_SPLIT(@PatientIds, ',')
    WHERE value <> '';
    
    -- Get all allergies for these patients
    SELECT 
        a.PatientId,
        a.AllergyId,
        a.AllergenName,
        a.AllergenType,
        a.Reaction,
        a.Severity,
        a.OnsetDate
    FROM pt.Allergies a
    INNER JOIN @PatientIdTable p ON a.PatientId = p.PatientId
    ORDER BY a.PatientId, a.Severity DESC;
END
GO
GRANT EXECUTE
    ON OBJECT::[pt].[usp_GetAllergiesBulk] TO PUBLIC
    AS [dbo];

