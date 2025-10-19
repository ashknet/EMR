CREATE TABLE [pt].[SocialHistory] (
    [PatientId]         UNIQUEIDENTIFIER NOT NULL,
    [SmokingStatusId]   INT              NULL,
    [AlcoholUseId]      INT              NULL,
    [DrugUseId]         INT              NULL,
    [Occupation]        NVARCHAR (150)   NULL,
    [LivingSituationId] INT              NULL,
    [CreatedAt]         DATETIME2 (7)    DEFAULT (getutcdate()) NOT NULL,
    [UpdatedAt]         DATETIME2 (7)    NULL,
    [ExerciseFrequency] NVARCHAR (50)    NULL,
    [Diet]              NVARCHAR (100)   NULL,
    [StressLevel]       NVARCHAR (50)    NULL,
    [SleepHours]        INT              NULL,
    PRIMARY KEY CLUSTERED ([PatientId] ASC),
    CONSTRAINT [FK_PTSocialHistory_Patient] FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);




GO
CREATE NONCLUSTERED INDEX [IX_SocialHistory_PatientId]
    ON [pt].[SocialHistory]([PatientId] ASC);

