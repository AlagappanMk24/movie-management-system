-- ==========================================
-- STORED PROCEDURES (Updated for Id & IsDeleted)
-- ==========================================

-- Retrieves all users
CREATE OR ALTER PROCEDURE SP_GetUsers
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserName,
        Email,
        Password,
        PhoneNumber,
        Role,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Users;
END;
GO

-- Retrieves a single user by ID
CREATE OR ALTER PROCEDURE SP_GetUserById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserName,
        Email,
        Password,
        PhoneNumber,
        Role,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Users
    WHERE Id = @Id;
END;
GO

IF OBJECT_ID('dbo.SP_CreateUser', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_CreateUser;
GO

-- Creates a new user (Added IsDeleted)
CREATE OR ALTER PROCEDURE SP_CreateUser
    @Id UNIQUEIDENTIFIER,
    @UserName NVARCHAR(150),
    @Email NVARCHAR(256),
    @Password NVARCHAR(MAX),
    @PhoneNumber NVARCHAR(50),
    @IsDeleted BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Users (Id, UserName, Email, Password, PhoneNumber, Role, Created, Modified, IsDeleted)
    VALUES (@Id, @UserName, @Email, @Password, @PhoneNumber, 'User', GETUTCDATE(), GETUTCDATE(), @IsDeleted);
END;
GO

-- Updates an existing user
CREATE OR ALTER PROCEDURE SP_UpdateUser
    @Id UNIQUEIDENTIFIER,
    @UserName NVARCHAR(150),
    @Email NVARCHAR(256),
    @PhoneNumber NVARCHAR(50),
    @Role NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Users
    SET 
        UserName = @UserName,
        Email = @Email,
        PhoneNumber = @PhoneNumber,
        Role = @Role,
        Modified = GETUTCDATE()
    WHERE Id = @Id;

    SELECT @@ROWCOUNT;
END;
GO

-- Deletes a user by ID
CREATE OR ALTER PROCEDURE SP_DeleteUser
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Users
    WHERE Id = @Id;
END;
GO

-- Retrieves a user by email address
CREATE OR ALTER PROCEDURE SP_GetUserByEmail
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserName,
        Email,
        Password,
        PhoneNumber,
        Role,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Users
    WHERE Email = @Email;
END;
GO