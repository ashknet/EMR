CREATE TABLE [pt].[PatientAddresses] (
    [Id]            INT              IDENTITY (1, 1) NOT NULL,
    [PatientId]     UNIQUEIDENTIFIER NOT NULL,
    [AddressTypeId] INT              NOT NULL,
    [Line1]         NVARCHAR (200)   NOT NULL,
    [Line2]         NVARCHAR (200)   NULL,
    [City]          NVARCHAR (100)   NULL,
    [State]         NVARCHAR (100)   NULL,
    [PostalCode]    NVARCHAR (20)    NULL,
    [Country]       NVARCHAR (100)   DEFAULT ('USA') NULL,
    [CreatedAt]     DATETIME2 (7)    DEFAULT (getutcdate()) NOT NULL,
    [UpdatedAt]     DATETIME2 (7)    NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PTPatientAddresses_Patient] FOREIGN KEY ([PatientId]) REFERENCES [pt].[Patients] ([PatientId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_PatientAddresses_PatientId]
    ON [pt].[PatientAddresses]([PatientId] ASC)
    INCLUDE([AddressTypeId], [Line1], [City], [State], [PostalCode]);

