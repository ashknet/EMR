CREATE TABLE [pt].[PatientPhones] (
    [Id]          INT              IDENTITY (1, 1) NOT NULL,
    [PatientId]   UNIQUEIDENTIFIER NOT NULL,
    [PhoneTypeId] INT              NOT NULL,
    [PhoneNumber] NVARCHAR (25)    NOT NULL,
    [CreatedAt]   DATETIME2 (7)    DEFAULT (getutcdate()) NOT NULL,
    [UpdatedAt]   DATETIME2 (7)    NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PTPatientPhones_Patient] FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_PatientPhones_PatientId]
    ON [pt].[PatientPhones]([PatientId] ASC)
    INCLUDE([PhoneTypeId], [PhoneNumber]);

