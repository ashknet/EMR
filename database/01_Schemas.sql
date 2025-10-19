-- Healthcare Platform Database Setup
-- Creates schemas for Patient Service (pt) and Agent Service (ag)

USE HealthcarePlatform;
GO

-- Create Patient Service Schema
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'pt')
BEGIN
    EXEC('CREATE SCHEMA pt');
END
GO

-- Create Agent Service Schema
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'ag')
BEGIN
    EXEC('CREATE SCHEMA ag');
END
GO

PRINT 'Schemas created successfully';

