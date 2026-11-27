-- ==========================================
-- TABLE SCHEMA (Users)
-- ==========================================
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
    DROP TABLE dbo.Users;
GO

CREATE TABLE dbo.Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserName NVARCHAR(150) NOT NULL,
    Email NVARCHAR(256) NOT NULL UNIQUE,
    Password NVARCHAR(MAX) NOT NULL,
    PhoneNumber NVARCHAR(50) NOT NULL,
    Role NVARCHAR(50) NULL,
    Created DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    Modified DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);
GO