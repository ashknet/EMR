
CREATE PROCEDURE pt.usp_SearchPatients
    @SearchTerm NVARCHAR(100) = NULL,
    @Email NVARCHAR(100) = NULL,
    @PhoneNumber NVARCHAR(25) = NULL,
    @DateOfBirth DATE = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    -- Get total count
    DECLARE @TotalCount INT;
    
    SELECT @TotalCount = COUNT(*)
    FROM pt.Patients p
    WHERE p.IsDeleted = 0
      AND (@SearchTerm IS NULL OR 
           p.FirstName LIKE '%' + @SearchTerm + '%' OR 
           p.LastName LIKE '%' + @SearchTerm + '%')
      AND (@Email IS NULL OR p.Email = @Email)
      AND (@PhoneNumber IS NULL OR p.PhoneNumber = @PhoneNumber)
      AND (@DateOfBirth IS NULL OR CAST(p.DateOfBirth AS DATE) = @DateOfBirth);
    
    -- Get paginated results
    SELECT 
        p.PatientId,
        p.FirstName,
        p.MiddleName,
        p.LastName,
        p.DateOfBirth,
        p.Gender,
        p.Email,
        p.PhoneNumber,
        p.City,
        p.State,
        @TotalCount AS TotalCount,
        @PageNumber AS CurrentPage,
        @PageSize AS PageSize
    FROM pt.Patients p
    WHERE p.IsDeleted = 0
      AND (@SearchTerm IS NULL OR 
           p.FirstName LIKE '%' + @SearchTerm + '%' OR 
           p.LastName LIKE '%' + @SearchTerm + '%')
      AND (@Email IS NULL OR p.Email = @Email)
      AND (@PhoneNumber IS NULL OR p.PhoneNumber = @PhoneNumber)
      AND (@DateOfBirth IS NULL OR CAST(p.DateOfBirth AS DATE) = @DateOfBirth)
    ORDER BY p.LastName, p.FirstName
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
GRANT EXECUTE
    ON OBJECT::[pt].[usp_SearchPatients] TO PUBLIC
    AS [dbo];

