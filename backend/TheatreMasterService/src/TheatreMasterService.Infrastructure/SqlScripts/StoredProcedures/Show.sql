-- ==========================================
-- STORED PROCEDURES FOR SHOWS (Guid ID)
-- ==========================================

-- 1. Retrieves all active shows
CREATE OR ALTER PROCEDURE SP_GetShows
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        MovieId,
        ScreenId,
        ShowDate,
        ShowTime,
        Price,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Shows
    WHERE IsDeleted = 0;
END;
GO

-- 2. Retrieves a single show by ID
CREATE OR ALTER PROCEDURE SP_GetShowById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        MovieId,
        ScreenId,
        ShowDate,
        ShowTime,
        Price,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Shows
    WHERE Id = @Id AND IsDeleted = 0;
END;
GO

-- 3. Retrieves shows filtered by Movie ID
CREATE OR ALTER PROCEDURE SP_GetShowsByMovieId
    @MovieId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        MovieId,
        ScreenId,
        ShowDate,
        ShowTime,
        Price,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Shows
    WHERE MovieId = @MovieId AND IsDeleted = 0;
END;
GO

-- 4. Creates a new show record and returns the inserted entity
IF OBJECT_ID('dbo.SP_CreateShow', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_CreateShow;
GO

CREATE PROCEDURE SP_CreateShow
    @Id UNIQUEIDENTIFIER,
    @MovieId UNIQUEIDENTIFIER,
    @ScreenId UNIQUEIDENTIFIER,
    @ShowDate DATETIME2,
    @ShowTime TIME,
    @Price INT,
    @Created DATETIME2,
    @Modified DATETIME2,
    @IsDeleted BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Shows (
        Id, MovieId, ScreenId, ShowDate, ShowTime, Price, Created, Modified, IsDeleted
    )
    VALUES (
        @Id, @MovieId, @ScreenId, @ShowDate, @ShowTime, @Price, @Created, @Modified, @IsDeleted
    );

    SELECT 
        Id,
        MovieId,
        ScreenId,
        ShowDate,
        ShowTime,
        Price,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Shows
    WHERE Id = @Id;
END;
GO

-- 5. Updates an existing show record
CREATE OR ALTER PROCEDURE SP_UpdateShow
    @Id UNIQUEIDENTIFIER,
    @MovieId UNIQUEIDENTIFIER,
    @ScreenId UNIQUEIDENTIFIER,
    @ShowDate DATETIME2,
    @ShowTime TIME,
    @Price INT,
    @Modified DATETIME2,
    @IsDeleted BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Shows
    SET 
        MovieId = @MovieId,
        ScreenId = @ScreenId,
        ShowDate = @ShowDate,
        ShowTime = @ShowTime,
        Price = @Price,
        Modified = @Modified,
        IsDeleted = @IsDeleted
    WHERE Id = @Id;

    SELECT @@ROWCOUNT;
END;
GO

-- 5. Soft deletes an active show by ID
CREATE OR ALTER PROCEDURE SP_DeleteShow
    @Id UNIQUEIDENTIFIER,
    @Modified DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Shows
    SET 
        IsDeleted = 1,
        Modified = ISNULL(@Modified, SYSUTCDATETIME())
    WHERE Id = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO