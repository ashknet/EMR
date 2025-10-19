CREATE TABLE [pt].[ProviderTypes] (
    [ProviderTypeId]  INT            IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (100) NOT NULL,
    [Description]     NVARCHAR (500) NULL,
    [DisplayOrder]    INT            NOT NULL DEFAULT 0,
    [IsActive]        BIT            NOT NULL DEFAULT 1,
    [CreatedAt]       DATETIME2 (7)  NOT NULL DEFAULT (getutcdate()),
    [UpdatedAt]       DATETIME2 (7)  NULL,
    
    CONSTRAINT [PK_ProviderTypes] PRIMARY KEY CLUSTERED ([ProviderTypeId] ASC),
    CONSTRAINT [UQ_ProviderTypes_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);
GO

CREATE NONCLUSTERED INDEX [IX_ProviderTypes_DisplayOrder]
    ON [pt].[ProviderTypes]([DisplayOrder] ASC, [IsActive] ASC);
GO

