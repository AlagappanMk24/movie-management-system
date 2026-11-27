-- ==========================================
-- STORED PROCEDURES FOR SCREENS (Guid ID)
-- ==========================================

-- 1. Retrieves all active screens belonging to active theatres
CREATE OR ALTER PROCEDURE SP_GetScreens
AS
BEGIN
    SET NOCOUNT ON;

    SELECT s.Id, s.TheatreId, s.ScreenName, s.ScreenType,
           s.SeatCapacity, s.Rows, s.SeatsPerRow,
           s.Created, s.Modified, s.IsDeleted
    FROM dbo.Screens s
    INNER JOIN dbo.Theatres t ON s.TheatreId = t.Id
    WHERE s.IsDeleted = 0 AND t.IsDeleted = 0;
END;
GO

-- 2. Retrieves a single active screen by ID belonging to an active theatre
CREATE OR ALTER PROCEDURE SP_GetScreenById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT s.Id, s.TheatreId, s.ScreenName, s.ScreenType,
           s.SeatCapacity, s.Rows, s.SeatsPerRow,
           s.Created, s.Modified, s.IsDeleted
    FROM dbo.Screens s
    INNER JOIN dbo.Theatres t ON s.TheatreId = t.Id
    WHERE s.Id = @Id AND s.IsDeleted = 0 AND t.IsDeleted = 0;
END;
GO

-- 3. Creates a new screen record only if the parent theatre is active
CREATE OR ALTER PROCEDURE SP_CreateScreen
    @Id            UNIQUEIDENTIFIER,
    @TheatreId     UNIQUEIDENTIFIER,
    @ScreenName    NVARCHAR(100),
    @ScreenType    NVARCHAR(50),
    @SeatCapacity  INT,
    @Rows          INT,
    @SeatsPerRow   INT,
    @Created       DATETIME2,
    @Modified      DATETIME2,
    @IsDeleted     BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
     SET XACT_ABORT ON;

     -- 1. Parent theatre must be alive
    IF NOT EXISTS (SELECT 1 FROM dbo.Theatres
                   WHERE Id = @TheatreId AND IsDeleted = 0)
        THROW 51000, 'Cannot create screen because the specified theatre does not exist or has been deleted.', 1;

    -- 2. Layout invariant
    IF (@Rows <= 0 OR @SeatsPerRow <= 0)
        THROW 51001, 'Rows and SeatsPerRow must both be greater than zero.', 1;

    IF (@SeatCapacity <> @Rows * @SeatsPerRow)
        THROW 51002, 'SeatCapacity must equal Rows * SeatsPerRow.', 1;

    -- 3. Insert
    INSERT INTO dbo.Screens
        (Id, TheatreId, ScreenName, ScreenType, SeatCapacity, Rows, SeatsPerRow,
         Created, Modified, IsDeleted)
    VALUES
        (@Id, @TheatreId, @ScreenName, @ScreenType, @SeatCapacity, @Rows, @SeatsPerRow,
         @Created, @Modified, @IsDeleted);

   -- 4. Return the row (Dapper maps this to Screen)
    SELECT Id, TheatreId, ScreenName, ScreenType, SeatCapacity,
           Rows, SeatsPerRow, Created, Modified, IsDeleted
    FROM dbo.Screens
    WHERE Id = @Id;
END;
GO

-- 4. Updates an existing active screen record only if the target theatre is active
CREATE OR ALTER PROCEDURE SP_UpdateScreen
    @Id           UNIQUEIDENTIFIER,
    @TheatreId    UNIQUEIDENTIFIER,
    @ScreenName   NVARCHAR(100),
    @ScreenType   NVARCHAR(50),
    @SeatCapacity INT,
    @Rows         INT,
    @SeatsPerRow  INT,
    @Modified     DATETIME2,
    @IsDeleted    BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF NOT EXISTS (SELECT 1 FROM dbo.Theatres
                   WHERE Id = @TheatreId AND IsDeleted = 0)
        THROW 51000, 'Cannot update screen because the specified theatre does not exist or has been deleted.', 1;

    IF (@SeatCapacity <> @Rows * @SeatsPerRow)
        THROW 51002, 'SeatCapacity must equal Rows * SeatsPerRow.', 1;

    UPDATE dbo.Screens
    SET TheatreId    = @TheatreId,
        ScreenName   = @ScreenName,
        ScreenType   = @ScreenType,
        SeatCapacity = @SeatCapacity,
        Rows         = @Rows,
        SeatsPerRow  = @SeatsPerRow,
        Modified     = @Modified,
        IsDeleted    = @IsDeleted
    WHERE Id = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO

-- 5. Soft deletes an active screen by ID
CREATE OR ALTER PROCEDURE SP_DeleteScreen
    @Id UNIQUEIDENTIFIER,
    @Modified DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Screens
    SET 
        IsDeleted = 1,
        Modified = ISNULL(@Modified, SYSUTCDATETIME())
    WHERE Id = @Id
      AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO