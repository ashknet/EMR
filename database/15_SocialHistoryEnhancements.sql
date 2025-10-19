-- =============================================
-- Social History Enhancements
-- Created: October 19, 2025
-- Purpose: Add additional lifestyle fields to Social History
-- =============================================

USE EMRMaster;
GO

-- Add new columns to SocialHistory table if they don't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('pt.SocialHistory') AND name = 'ExerciseFrequency')
BEGIN
    ALTER TABLE pt.SocialHistory ADD ExerciseFrequency NVARCHAR(50) NULL;
    PRINT 'Added ExerciseFrequency column to pt.SocialHistory';
END
ELSE
BEGIN
    PRINT 'ExerciseFrequency column already exists in pt.SocialHistory';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('pt.SocialHistory') AND name = 'Diet')
BEGIN
    ALTER TABLE pt.SocialHistory ADD Diet NVARCHAR(100) NULL;
    PRINT 'Added Diet column to pt.SocialHistory';
END
ELSE
BEGIN
    PRINT 'Diet column already exists in pt.SocialHistory';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('pt.SocialHistory') AND name = 'StressLevel')
BEGIN
    ALTER TABLE pt.SocialHistory ADD StressLevel NVARCHAR(50) NULL;
    PRINT 'Added StressLevel column to pt.SocialHistory';
END
ELSE
BEGIN
    PRINT 'StressLevel column already exists in pt.SocialHistory';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('pt.SocialHistory') AND name = 'SleepHours')
BEGIN
    ALTER TABLE pt.SocialHistory ADD SleepHours INT NULL;
    PRINT 'Added SleepHours column to pt.SocialHistory';
END
ELSE
BEGIN
    PRINT 'SleepHours column already exists in pt.SocialHistory';
END
GO

-- Verification
PRINT '';
PRINT '=== Verification ===';
PRINT 'Social History table structure:';
EXEC sp_help 'pt.SocialHistory';
GO

PRINT '';
PRINT '✓ Social History enhancements complete';
GO

