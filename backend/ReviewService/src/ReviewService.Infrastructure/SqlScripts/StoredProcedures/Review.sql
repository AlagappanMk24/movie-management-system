-- ==========================================
-- STORED PROCEDURES FOR REVIEWS (Guid ID)
-- ==========================================

-- 1. Retrieves all active reviews ordered by creation date descending
CREATE OR ALTER PROCEDURE SP_GetReviews
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        MovieId,
        UserId,
        Category,
        Description,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Reviews
    WHERE IsDeleted = 0
    ORDER BY Created DESC;
END;
GO

-- 2. Retrieves a single review by ID
CREATE OR ALTER PROCEDURE SP_GetReviewById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        MovieId,
        UserId,
        Category,
        Description,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Reviews
    WHERE Id = @Id AND IsDeleted = 0;
END;
GO

-- 3. Retrieves reviews filtered by Movie ID ordered by creation date descending
CREATE OR ALTER PROCEDURE SP_GetReviewsByMovieId
    @MovieId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        MovieId,
        UserId,
        Category,
        Description,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Reviews
    WHERE MovieId = @MovieId AND IsDeleted = 0
    ORDER BY Created DESC;
END;
GO

-- 4. Retrieves a review filtered by both Movie ID and User ID
CREATE OR ALTER PROCEDURE SP_GetReviewByMovieAndUser
    @MovieId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 1
        Id,
        MovieId,
        UserId,
        Category,
        Description,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Reviews
    WHERE MovieId = @MovieId AND UserId = @UserId AND IsDeleted = 0;
END;
GO

-- 5. Inserts a new review record and returns the inserted entity
IF OBJECT_ID('dbo.SP_CreateReview', 'P') IS NOT NULL
    DROP PROCEDURE dbo.SP_CreateReview;
GO

CREATE PROCEDURE SP_CreateReview
    @Id UNIQUEIDENTIFIER,
    @MovieId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @Category INT,
    @Description NVARCHAR(1000),
    @Created DATETIME2,
    @Modified DATETIME2,
    @IsDeleted BIT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Reviews (
        Id, MovieId, UserId, Category, Description, Created, Modified, IsDeleted
    )
    VALUES (
        @Id, @MovieId, @UserId, @Category, @Description, @Created, @Modified, @IsDeleted
    );

    SELECT 
        Id,
        MovieId,
        UserId,
        Category,
        Description,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Reviews
    WHERE Id = @Id;
END;
GO

-- 6. Updates an existing review record
CREATE OR ALTER PROCEDURE SP_UpdateReview
    @Id UNIQUEIDENTIFIER,
    @Category INT,
    @Description NVARCHAR(1000),
    @Modified DATETIME2,
    @IsDeleted BIT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Reviews
    SET 
        Category = @Category,
        Description = @Description,
        Modified = @Modified,
        IsDeleted = @IsDeleted
    WHERE Id = @Id;

    SELECT @@ROWCOUNT;
END;
GO