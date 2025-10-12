-- =============================================
-- Healthcare Platform - Complete Database Setup Script
-- Run this script to create all databases and load test data
-- =============================================

PRINT '🏥 Healthcare Platform - Database Setup'
PRINT '========================================='
PRINT ''

-- =============================================
-- Create all databases
-- =============================================
PRINT 'Step 1: Creating databases...'
PRINT ''

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'HealthcarePlatform_Patient')
BEGIN
    CREATE DATABASE HealthcarePlatform_Patient
    PRINT '✓ Created HealthcarePlatform_Patient'
END
ELSE
    PRINT '  HealthcarePlatform_Patient already exists'

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'HealthcarePlatform_Family')
BEGIN
    CREATE DATABASE HealthcarePlatform_Family
    PRINT '✓ Created HealthcarePlatform_Family'
END
ELSE
    PRINT '  HealthcarePlatform_Family already exists'

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'HealthcarePlatform_HealthHistory')
BEGIN
    CREATE DATABASE HealthcarePlatform_HealthHistory
    PRINT '✓ Created HealthcarePlatform_HealthHistory'
END
ELSE
    PRINT '  HealthcarePlatform_HealthHistory already exists'

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'HealthcarePlatform_Insurance')
BEGIN
    CREATE DATABASE HealthcarePlatform_Insurance
    PRINT '✓ Created HealthcarePlatform_Insurance'
END
ELSE
    PRINT '  HealthcarePlatform_Insurance already exists'

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'HealthcarePlatform_Consent')
BEGIN
    CREATE DATABASE HealthcarePlatform_Consent
    PRINT '✓ Created HealthcarePlatform_Consent'
END
ELSE
    PRINT '  HealthcarePlatform_Consent already exists'

PRINT ''
PRINT '✓ All databases created successfully!'
PRINT ''
PRINT '========================================='
PRINT 'Next Steps:'
PRINT '========================================='
PRINT ''
PRINT '1. Apply schemas:'
PRINT '   sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_Patient -i database/schemas/01_PatientService_Schema.sql'
PRINT '   sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_Family -i database/schemas/02_FamilyService_Schema.sql'
PRINT '   sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_HealthHistory -i database/schemas/03_HealthHistoryService_Schema.sql'
PRINT '   sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_Insurance -i database/schemas/04_InsuranceService_Schema.sql'
PRINT '   sqlcmd -S "(localdb)\mssqllocaldb" -d HealthcarePlatform_Consent -i database/schemas/05_ConsentAuditService_Schema.sql'
PRINT ''
PRINT '2. Load test data:'
PRINT '   sqlcmd -S "(localdb)\mssqllocaldb" -i database/test-data/01_PatientService_TestData.sql'
PRINT '   sqlcmd -S "(localdb)\mssqllocaldb" -i database/test-data/02_AllServices_TestData.sql'
PRINT ''
PRINT '3. Run services:'
PRINT '   .\run-all-services.ps1   (Windows)'
PRINT '   ./run-all-services.sh    (Linux/Mac)'
PRINT ''
PRINT 'Or use Docker Compose:'
PRINT '   docker-compose up -d'
PRINT ''
GO
