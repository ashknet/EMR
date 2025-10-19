CREATE TABLE [pt].[ChronicConditions] (
    [ConditionId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [PatientId] UNIQUEIDENTIFIER NOT NULL,
    [ConditionName] NVARCHAR(200) NOT NULL,
    [DiagnosedDate] DATE NULL,
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Active',
    [Notes] NVARCHAR(MAX) NULL,
    
    -- System Fields
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NULL,
    [CreatedBy] NVARCHAR(100) NOT NULL DEFAULT SYSTEM_USER,
    [UpdatedBy] NVARCHAR(100) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT [PK_ChronicConditions] PRIMARY KEY CLUSTERED ([ConditionId] ASC),
    CONSTRAINT [FK_ChronicConditions_Patient] FOREIGN KEY ([PatientId]) 
        REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX [IX_ChronicConditions_PatientId] 
    ON [pt].[ChronicConditions] ([PatientId]) 
    INCLUDE ([ConditionName], [Status], [IsActive]);
GO

