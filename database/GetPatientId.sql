-- Quick script to get Patient ID for development testing
-- Run this in SQL Server Management Studio (SSMS) after running the sample data scripts

USE HealthcarePlatform;
GO

-- Get first active patient (John Doe if sample data was loaded)
SELECT TOP 1 
    PatientId,
    FirstName,
    LastName,
    Email,
    PhoneNumber,
    DateOfBirth,
    CONCAT(FirstName, ' ', LastName) AS FullName
FROM pt.Patients 
WHERE IsActive = 1 
  AND IsDeleted = 0
ORDER BY CreatedAt ASC;

-- If no patients exist, you need to run the sample data script first:
-- Run: 03_SampleData.sql

