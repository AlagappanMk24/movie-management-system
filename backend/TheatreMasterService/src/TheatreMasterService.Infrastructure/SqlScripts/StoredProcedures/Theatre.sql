-- ==========================================
-- STORED PROCEDURES FOR THEATRES (Guid ID)
-- ==========================================

-- 1. Retrieves all active theatres
CREATE OR ALTER PROCEDURE SP_GetTheatres
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        TheatreName,
        City,
        Address,
        PhoneNumber,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Theatres
    WHERE IsDeleted = 0;
END;
GO

-- 2. Retrieves a single theatre by ID
CREATE OR ALTER PROCEDURE SP_GetTheatreById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        TheatreName,
        City,
        Address,
        PhoneNumber,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Theatres
    WHERE Id = @Id AND IsDeleted = 0;
END;
GO

-- 3. Creates a new theatre record and returns the inserted entity
IF OBJECT_ID('dbo.SP_CreateTheatre', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_CreateTheatre;
GO

CREATE PROCEDURE SP_CreateTheatre
    @Id UNIQUEIDENTIFIER,
    @TheatreName NVARCHAR(200),
    @City NVARCHAR(100),
    @Address NVARCHAR(MAX),
    @PhoneNumber NVARCHAR(50),
    @Created DATETIME2,
    @Modified DATETIME2,
    @IsDeleted BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Theatres (
        Id, TheatreName, City, Address, PhoneNumber, Created, Modified, IsDeleted
    )
    VALUES (
        @Id, @TheatreName, @City, @Address, @PhoneNumber, @Created, @Modified, @IsDeleted
    );

    SELECT 
        Id, TheatreName, City, Address, PhoneNumber, Created, Modified, IsDeleted
    FROM dbo.Theatres
    WHERE Id = @Id;
END;
GO

-- 4. Updates an existing theatre record
CREATE OR ALTER PROCEDURE SP_UpdateTheatre
    @Id UNIQUEIDENTIFIER,
    @TheatreName NVARCHAR(200),
    @City NVARCHAR(100),
    @Address NVARCHAR(MAX),
    @PhoneNumber NVARCHAR(50),
    @Modified DATETIME2,
    @IsDeleted BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Theatres
    SET 
        TheatreName = @TheatreName,
        City = @City,
        Address = @Address,
        PhoneNumber = @PhoneNumber,
        Modified = @Modified,
        IsDeleted = @IsDeleted
    WHERE Id = @Id;

    SELECT @@ROWCOUNT;
END;
GO

--5. Soft deletes a theatre by ID
CREATE OR ALTER PROCEDURE SP_DeleteTheatre
    @Id UNIQUEIDENTIFIER,
    @Modified DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Theatres
    SET 
        IsDeleted = 1,
        Modified = ISNULL(@Modified, GETDATE()) -- Automatically sets to current time if not supplied
    WHERE Id = @Id AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO