CREATE TABLE [pt].[EmergencyContacts] (
    [Id]                 INT              IDENTITY (1, 1) NOT NULL,
    [PatientId]          UNIQUEIDENTIFIER NOT NULL,
    [Name]               NVARCHAR (150)   NOT NULL,
    [RelationshipTypeId] INT              NOT NULL,
    [Phone]              NVARCHAR (25)    NOT NULL,
    [AltPhone]           NVARCHAR (25)    NULL,
    [CreatedAt]          DATETIME2 (7)    DEFAULT (getutcdate()) NOT NULL,
    [UpdatedAt]          DATETIME2 (7)    NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PTEmergencyContacts_Patient] FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_EmergencyContacts_PatientId]
    ON [pt].[EmergencyContacts]([PatientId] ASC)
    INCLUDE([Name], [RelationshipTypeId], [Phone]);

