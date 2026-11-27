-- ==========================================
-- STORED PROCEDURES FOR MOVIES (Soft Delete Pattern)
-- ==========================================

-- Retrieves all active (non-deleted) movies
CREATE OR ALTER PROCEDURE SP_GetMovie
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        Title,
        Genre,
        ReleaseDate,
        Director,
        Description,
        Actor,
        Actress,
        Language,
        DurationMinutes,
        PosterUrl,
        TrailerUrl,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Movies
    WHERE IsDeleted = 0;
END;
GO

-- Retrieves a single active movie by ID
CREATE OR ALTER PROCEDURE SP_GetMovieById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        Title,
        Genre,
        ReleaseDate,
        Director,
        Description,
        Actor,
        Actress,
        Language,
        DurationMinutes,
        PosterUrl,
        TrailerUrl,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Movies
    WHERE Id = @Id AND IsDeleted = 0;
END;
GO

IF OBJECT_ID('dbo.SP_CreateMovie', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_CreateMovie;
GO

-- Creates a new movie and returns the inserted record
CREATE OR ALTER PROCEDURE SP_CreateMovie
    @Id UNIQUEIDENTIFIER,
    @Title NVARCHAR(200),
    @Genre NVARCHAR(100),
    @ReleaseDate DATETIME2,
    @Director NVARCHAR(150),
    @Description NVARCHAR(MAX),
    @Actor NVARCHAR(150),
    @Actress NVARCHAR(150),
    @Language NVARCHAR(50),
    @DurationMinutes INT,
    @PosterUrl NVARCHAR(MAX),
    @TrailerUrl NVARCHAR(MAX),
    @Created DATETIME2,
    @Modified DATETIME2,
    @IsDeleted BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Movies (
        Id, Title, Genre, ReleaseDate, Director, Description, 
        Actor, Actress, Language, DurationMinutes, PosterUrl, TrailerUrl, Created, Modified, IsDeleted
    )
    VALUES (
        @Id, @Title, @Genre, @ReleaseDate, @Director, @Description, 
        @Actor, @Actress, @Language, @DurationMinutes, @PosterUrl, @TrailerUrl, @Created, @Modified, @IsDeleted
    );

    SELECT 
        Id, Title, Genre, ReleaseDate, Director, Description, 
        Actor, Actress, Language, DurationMinutes, PosterUrl, TrailerUrl, Created, Modified, IsDeleted
    FROM dbo.Movies
    WHERE Id = @Id;
END;
GO

-- Update Movie command (no select needed)
CREATE OR ALTER PROCEDURE SP_UpdateMovie
    @Id UNIQUEIDENTIFIER,
    @Title NVARCHAR(200),
    @Genre NVARCHAR(100),
    @ReleaseDate DATETIME2,
    @Director NVARCHAR(150),
    @Description NVARCHAR(MAX),
    @Actor NVARCHAR(150),
    @Actress NVARCHAR(150),
    @Language NVARCHAR(50),
    @DurationMinutes INT,
    @PosterUrl NVARCHAR(MAX),
    @TrailerUrl NVARCHAR(MAX),
    @Modified DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Movies
    SET 
        Title = @Title,
        Genre = @Genre,
        ReleaseDate = @ReleaseDate,
        Director = @Director,
        Description = @Description,
        Actor = @Actor,
        Actress = @Actress,
        Language = @Language,
        DurationMinutes = @DurationMinutes,
        PosterUrl = ISNULL(@PosterUrl, PosterUrl),
        TrailerUrl = @TrailerUrl,
        Modified = @Modified
    WHERE Id = @Id AND IsDeleted = 0;
END;
GO

-- Soft deletes a movie by ID
CREATE OR ALTER PROCEDURE SP_DeleteMovie
    @Id UNIQUEIDENTIFIER,
    @Modified DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Movies
    SET 
        IsDeleted = 1,
        Modified = ISNULL(@Modified, GETDATE()) -- Automatically sets to current time if not supplied
    WHERE Id = @Id AND IsDeleted = 0;

    SELECT @@ROWCOUNT;
END;
GO