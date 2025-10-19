-- EMR Master Database Script
-- Electronic Medical Record Platform

USE master;
GO

-- Create EMR database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'EMR')
BEGIN
    CREATE DATABASE EMR;
END
GO

USE EMR;
GO

-- Create Gender lookup table
CREATE TABLE [dbo].[Gender](
    [GenderId] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
    [Name] [nvarchar](50) NOT NULL,
    [Code] [nvarchar](10) NOT NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedDate] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedDate] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] [nvarchar](100) NULL,
    [ModifiedBy] [nvarchar](100) NULL,
    CONSTRAINT [PK_Gender] PRIMARY KEY CLUSTERED ([GenderId] ASC)
);
GO

-- Create Hospital table
CREATE TABLE [dbo].[Hospital](
    [HospitalId] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
    [Name] [nvarchar](200) NOT NULL,
    [Address] [nvarchar](500) NULL,
    [City] [nvarchar](100) NULL,
    [State] [nvarchar](50) NULL,
    [ZipCode] [nvarchar](20) NULL,
    [Phone] [nvarchar](20) NULL,
    [Email] [nvarchar](100) NULL,
    [LicenseNumber] [nvarchar](50) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [CreatedDate] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedDate] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] [nvarchar](100) NULL,
    [ModifiedBy] [nvarchar](100) NULL,
    CONSTRAINT [PK_Hospital] PRIMARY KEY CLUSTERED ([HospitalId] ASC)
);
GO

-- Create Patient table
CREATE TABLE [dbo].[Patient](
    [PatientId] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
    [MRN] [nvarchar](50) NOT NULL,
    [FirstName] [nvarchar](100) NOT NULL,
    [LastName] [nvarchar](100) NOT NULL,
    [MiddleName] [nvarchar](100) NULL,
    [DateOfBirth] [date] NOT NULL,
    [GenderId] [uniqueidentifier] NOT NULL,
    [SSN] [nvarchar](20) NULL,
    [Address] [nvarchar](500) NULL,
    [City] [nvarchar](100) NULL,
    [State] [nvarchar](50) NULL,
    [ZipCode] [nvarchar](20) NULL,
    [Phone] [nvarchar](20) NULL,
    [Email] [nvarchar](100) NULL,
    [EmergencyContactName] [nvarchar](200) NULL,
    [EmergencyContactPhone] [nvarchar](20) NULL,
    [EmergencyContactRelationship] [nvarchar](100) NULL,
    [IsActive] [bit] NOT NULL DEFAULT 1,
    [IsDeleted] [bit] NOT NULL DEFAULT 0,
    [CreatedDate] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedDate] [datetime2](7) NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] [nvarchar](100) NULL,
    [ModifiedBy] [nvarchar](100) NULL,
    CONSTRAINT [PK_Patient] PRIMARY KEY CLUSTERED ([PatientId] ASC),
    CONSTRAINT [FK_Patient_Gender] FOREIGN KEY([GenderId]) REFERENCES [dbo].[Gender] ([GenderId])
);
GO

-- Create indexes for performance
CREATE UNIQUE NONCLUSTERED INDEX [IX_Patient_MRN] ON [dbo].[Patient] ([MRN] ASC);
CREATE NONCLUSTERED INDEX [IX_Patient_Name] ON [dbo].[Patient] ([LastName] ASC, [FirstName] ASC);
GO

-- Insert seed data
INSERT INTO [dbo].[Gender] ([Name], [Code]) VALUES 
('Male', 'M'),
('Female', 'F'),
('Other', 'O'),
('Unknown', 'U');
GO

INSERT INTO [dbo].[Hospital] ([Name], [Address], [City], [State], [ZipCode], [Phone], [Email], [LicenseNumber]) VALUES 
('General Hospital', '123 Main St', 'Anytown', 'CA', '12345', '(555) 123-4567', 'info@generalhospital.com', 'HL001'),
('City Medical Center', '456 Oak Ave', 'Anytown', 'CA', '12346', '(555) 234-5678', 'info@citymedical.com', 'HL002');
GO

-- Create sample patients
DECLARE @MaleGenderId uniqueidentifier = (SELECT GenderId FROM [dbo].[Gender] WHERE Code = 'M');
DECLARE @FemaleGenderId uniqueidentifier = (SELECT GenderId FROM [dbo].[Gender] WHERE Code = 'F');

INSERT INTO [dbo].[Patient] ([MRN], [FirstName], [LastName], [DateOfBirth], [GenderId], [Phone], [Email]) VALUES 
('MRN001', 'John', 'Doe', '1980-01-15', @MaleGenderId, '(555) 111-2222', 'john.doe@email.com'),
('MRN002', 'Jane', 'Smith', '1985-05-20', @FemaleGenderId, '(555) 333-4444', 'jane.smith@email.com'),
('MRN003', 'Bob', 'Johnson', '1975-12-10', @MaleGenderId, '(555) 555-6666', 'bob.johnson@email.com');
GO

PRINT 'EMR Database schema created successfully!';
PRINT 'Seed data inserted successfully!';
GO