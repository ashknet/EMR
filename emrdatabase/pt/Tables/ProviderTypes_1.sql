CREATE TABLE [pt].[ProviderTypes] (
    [ProviderTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (100) NOT NULL,
    [Description]    NVARCHAR (500) NULL,
    [DisplayOrder]   INT            DEFAULT ((0)) NOT NULL,
    [IsActive]       BIT            DEFAULT ((1)) NOT NULL,
    [CreatedAt]      DATETIME2 (7)  DEFAULT (getutcdate()) NOT NULL,
    [UpdatedAt]      DATETIME2 (7)  NULL,
    CONSTRAINT [PK_ProviderTypes] PRIMARY KEY CLUSTERED ([ProviderTypeId] ASC),
    CONSTRAINT [UQ_ProviderTypes_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);

