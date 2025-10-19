
CREATE PROCEDURE pt.usp_SavePatientIntake
    @PatientData NVARCHAR(MAX), -- JSON format
    @CreatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Parse JSON
        DECLARE @PatientId UNIQUEIDENTIFIER;
        DECLARE @FirstName NVARCHAR(100);
        DECLARE @LastName NVARCHAR(100);
        DECLARE @Email NVARCHAR(100);
        
        SELECT 
            @PatientId = JSON_VALUE(@PatientData, '$.patient.id'),
            @FirstName = JSON_VALUE(@PatientData, '$.patient.firstName'),
            @LastName = JSON_VALUE(@PatientData, '$.patient.lastName'),
            @Email = JSON_VALUE(@PatientData, '$.patient.email');
        
        -- Check if new or update
        IF @PatientId IS NULL OR @PatientId = '00000000-0000-0000-0000-000000000000'
        BEGIN
            SET @PatientId = NEWID();
            
            -- Insert new patient (simplified - full implementation would parse all JSON fields)
            INSERT INTO pt.Patients (
                PatientId, FirstName, LastName, Email, 
                CreatedAt, CreatedBy, IsActive, IsDeleted
            )
            VALUES (
                @PatientId, @FirstName, @LastName, @Email,
                GETUTCDATE(), @CreatedBy, 1, 0
            );
        END
        ELSE
        BEGIN
            -- Update existing patient
            UPDATE pt.Patients
            SET 
                FirstName = @FirstName,
                LastName = @LastName,
                Email = @Email,
                UpdatedAt = GETUTCDATE(),
                UpdatedBy = @CreatedBy
            WHERE PatientId = @PatientId;
        END
        
        -- Log audit
        INSERT INTO pt.AuditLogs (
            AuditLogId, PatientId, EventType, EntityType, EntityId,
            EventTimestamp, ActorId, ActorName, Action, Success, PhiAccessed
        )
        VALUES (
            NEWID(), @PatientId, 'Update', 'Patient', @PatientId,
            GETUTCDATE(), @CreatedBy, @CreatedBy, 'Comprehensive intake completed', 1, 1
        );
        
        COMMIT TRANSACTION;
        
        -- Return patient ID
        SELECT @PatientId AS PatientId, 'Success' AS Status;
        RETURN 0;
        
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- Return error
        SELECT 
            NULL AS PatientId, 
            'Error' AS Status,
            ERROR_MESSAGE() AS ErrorMessage;
        RETURN -1;
    END CATCH
END
GO
GRANT EXECUTE
    ON OBJECT::[pt].[usp_SavePatientIntake] TO PUBLIC
    AS [dbo];

