
CREATE PROCEDURE pt.usp_UpdatePersonalInfo
    @PatientId UNIQUEIDENTIFIER,
    @FirstName NVARCHAR(100) = NULL,
    @MiddleName NVARCHAR(100) = NULL,
    @LastName NVARCHAR(100) = NULL,
    @Suffix NVARCHAR(20) = NULL,
    @DateOfBirth DATETIME2 = NULL,
    @Gender NVARCHAR(50) = NULL,
    @MaritalStatus NVARCHAR(50) = NULL,
    @Race NVARCHAR(100) = NULL,
    @Ethnicity NVARCHAR(100) = NULL,
    @PreferredLanguage NVARCHAR(50) = NULL,
    @UpdatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if patient exists
    IF NOT EXISTS (SELECT 1 FROM pt.Patients WHERE PatientId = @PatientId AND IsDeleted = 0)
    BEGIN
        RAISERROR('Patient not found', 16, 1);
        RETURN -1;
    END
    
    -- Update only provided fields
    UPDATE pt.Patients
    SET 
        FirstName = ISNULL(@FirstName, FirstName),
        MiddleName = CASE WHEN @MiddleName IS NOT NULL THEN @MiddleName ELSE MiddleName END,
        LastName = ISNULL(@LastName, LastName),
        Suffix = CASE WHEN @Suffix IS NOT NULL THEN @Suffix ELSE Suffix END,
        DateOfBirth = ISNULL(@DateOfBirth, DateOfBirth),
        Gender = ISNULL(@Gender, Gender),
        MaritalStatus = CASE WHEN @MaritalStatus IS NOT NULL THEN @MaritalStatus ELSE MaritalStatus END,
        Race = CASE WHEN @Race IS NOT NULL THEN @Race ELSE Race END,
        Ethnicity = CASE WHEN @Ethnicity IS NOT NULL THEN @Ethnicity ELSE Ethnicity END,
        PreferredLanguage = CASE WHEN @PreferredLanguage IS NOT NULL THEN @PreferredLanguage ELSE PreferredLanguage END,
        UpdatedAt = GETUTCDATE(),
        UpdatedBy = @UpdatedBy
    WHERE PatientId = @PatientId;
    
    -- Log audit
    INSERT INTO pt.AuditLogs (
        AuditLogId, PatientId, EventType, EntityType, EntityId, 
        EventTimestamp, ActorId, ActorName, Action, Success, PhiAccessed
    )
    VALUES (
        NEWID(), @PatientId, 'Update', 'Patient', @PatientId,
        GETUTCDATE(), @UpdatedBy, @UpdatedBy, 'Personal information updated', 1, 1
    );
    
    RETURN 0;
END
GO
GRANT EXECUTE
    ON OBJECT::[pt].[usp_UpdatePersonalInfo] TO PUBLIC
    AS [dbo];

