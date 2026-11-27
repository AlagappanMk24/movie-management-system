-- ==========================================
-- STORED PROCEDURES FOR BOOKINGS (Guid ID)
-- ==========================================

-- 1. Retrieves all active bookings
CREATE OR ALTER PROCEDURE SP_GetBookings
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserId,
        ShowId,
        SeatCount,
        PaymentMethod,
        PaymentStatus,
        TotalAmount,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Bookings
    WHERE IsDeleted = 0;
END;
GO

-- 2. Retrieves a single booking by ID
CREATE OR ALTER PROCEDURE SP_GetBookingById
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserId,
        ShowId,
        SeatCount,
        PaymentMethod,
        PaymentStatus,
        TotalAmount,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Bookings
    WHERE Id = @Id AND IsDeleted = 0;
END;
GO

-- 3. Retrieves bookings filtered by Show ID
CREATE OR ALTER PROCEDURE SP_GetBookingsByShowId
    @ShowId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserId,
        ShowId,
        SeatCount,
        PaymentMethod,
        PaymentStatus,
        TotalAmount,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Bookings
    WHERE ShowId = @ShowId AND IsDeleted = 0;
END;
GO

-- 4. Retrieves bookings filtered by User ID
CREATE OR ALTER PROCEDURE SP_GetBookingsByUserId
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserId,
        ShowId,
        SeatCount,
        PaymentMethod,
        PaymentStatus,
        TotalAmount,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Bookings
    WHERE UserId = @UserId AND IsDeleted = 0;
END;
GO

-- 5. Creates a new booking record and returns the inserted row
CREATE OR ALTER PROCEDURE SP_CreateBooking
    @Id UNIQUEIDENTIFIER = NULL,
    @UserId UNIQUEIDENTIFIER,
    @ShowId UNIQUEIDENTIFIER,
    @SeatCount INT,
    @PaymentMethod NVARCHAR(100),
    @PaymentStatus NVARCHAR(100),
    @TotalAmount INT,
    @Created DATETIME2,
    @Modified DATETIME2,
    @IsDeleted BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        SET @Id = COALESCE(NULLIF(@Id, '00000000-0000-0000-0000-000000000000'), NEWID());
        SET @IsDeleted = ISNULL(@IsDeleted, 0);

        INSERT INTO dbo.Bookings (
            Id, UserId, ShowId, SeatCount, PaymentMethod, PaymentStatus, TotalAmount, Created, Modified, IsDeleted
        )
        VALUES (
            @Id, @UserId, @ShowId, @SeatCount, @PaymentMethod, @PaymentStatus, @TotalAmount, @Created, @Modified, @IsDeleted
        );

        SELECT 
            Id, UserId, ShowId, SeatCount, PaymentMethod, PaymentStatus, TotalAmount, Created, Modified, IsDeleted
        FROM dbo.Bookings
        WHERE Id = @Id;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- 6. Updates an existing active booking record
CREATE OR ALTER PROCEDURE SP_UpdateBooking
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @ShowId UNIQUEIDENTIFIER,
    @SeatCount INT,
    @PaymentMethod NVARCHAR(100),
    @PaymentStatus NVARCHAR(100),
    @TotalAmount INT,
    @Modified DATETIME2,
    @IsDeleted BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        UPDATE dbo.Bookings
        SET 
            UserId = @UserId,
            ShowId = @ShowId,
            SeatCount = @SeatCount,
            PaymentMethod = @PaymentMethod,
            PaymentStatus = @PaymentStatus,
            TotalAmount = @TotalAmount,
            Modified = @Modified,
            IsDeleted = ISNULL(@IsDeleted, 0)
        WHERE Id = @Id
          AND IsDeleted = 0;

        SELECT @@ROWCOUNT;
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- 7. Gets the total booked seat count for a specific show where payment is completed
CREATE OR ALTER PROCEDURE SP_GetBookedSeatCountByShow
    @ShowId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT ISNULL(SUM(SeatCount), 0) AS TotalBookedSeats
    FROM dbo.Bookings
    WHERE ShowId = @ShowId 
      AND (PaymentStatus = 'Completed' OR PaymentStatus = 'Paid')
      AND IsDeleted = 0;
END;
GO

-- 8. Retrieves all completed/paid bookings
CREATE OR ALTER PROCEDURE SP_GetCompletedBookings
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        Id,
        UserId,
        ShowId,
        SeatCount,
        PaymentMethod,
        PaymentStatus,
        TotalAmount,
        Created,
        Modified,
        IsDeleted
    FROM dbo.Bookings
    WHERE (PaymentStatus = 'Completed' OR PaymentStatus = 'Paid') 
      AND IsDeleted = 0;
END;
GO


-- 9. Retrieves bookings filtered by Movie ID (via Shows join)
CREATE OR ALTER PROCEDURE SP_GetBookingsByMovieId
    @MovieId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        b.Id AS Id,
        b.UserId AS UserId,
        b.ShowId AS ShowId,
        b.SeatCount AS SeatCount,
        b.PaymentMethod AS PaymentMethod,
        b.PaymentStatus AS PaymentStatus,
        b.TotalAmount AS TotalAmount,
        b.Created AS Created,
        b.Modified AS Modified,
        b.IsDeleted AS IsDeleted
    FROM dbo.Bookings b
    INNER JOIN dbo.Shows s ON b.ShowId = s.Id
    WHERE s.MovieId = @MovieId 
      AND b.IsDeleted = 0 
      AND s.IsDeleted = 0;
END;
GO

-- Seat assignments table for bookings
IF OBJECT_ID(N'dbo.BookingSeats', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.BookingSeats
    (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        BookingId UNIQUEIDENTIFIER NOT NULL,
        ShowId UNIQUEIDENTIFIER NOT NULL,
        SeatNo NVARCHAR(20) NOT NULL,
        Price INT NOT NULL,
        Created DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_BookingSeats_Bookings FOREIGN KEY (BookingId)
            REFERENCES dbo.Bookings (Id)
    );

    CREATE INDEX IX_BookingSeats_BookingId ON dbo.BookingSeats (BookingId);
    CREATE INDEX IX_BookingSeats_ShowId_SeatNo ON dbo.BookingSeats (ShowId, SeatNo);
END;
GO

-- Table-valued parameter type for bulk seat inserts (if not already created)
IF TYPE_ID(N'dbo.BookingSeatType') IS NULL
BEGIN
    CREATE TYPE dbo.BookingSeatType AS TABLE
    (
        SeatNo NVARCHAR(20) NOT NULL,
        Price INT NOT NULL
    );
END;
GO

-- Retrieves seat numbers assigned to a specific booking
CREATE OR ALTER PROCEDURE SP_GetBookedSeatsByBookingId
    @BookingId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        SELECT SeatNo,
        Price
        FROM dbo.BookingSeats
        WHERE BookingId = @BookingId 
            AND IsDeleted = 0
    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- Bulk inserts seats for a booking/show via TVP
CREATE OR ALTER PROCEDURE SP_BulkInsertBookingSeats
    @BookingId UNIQUEIDENTIFIER,
    @ShowId UNIQUEIDENTIFIER,
    @Seats dbo.BookingSeatType READONLY
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

       -- Optional: guard against empty TVP
        IF NOT EXISTS (SELECT 1 FROM @Seats)
            RETURN;

       INSERT INTO dbo.BookingSeats
        (
            Id,
            BookingId,
            ShowId,
            SeatNo,
            Price,
            Created,    
            Modified,   
            IsDeleted
        )
        SELECT
            NEWID(),          -- generate a new Id for every seat
            @BookingId,
            @ShowId,
            s.SeatNo,
            s.Price,
            SYSUTCDATETIME(),   -- Created
            SYSUTCDATETIME(),   -- Modified
            0                   -- IsDeleted
        FROM @Seats AS s;

    END TRY
    BEGIN CATCH
        THROW;
    END CATCH
END;
GO

-- Returns every seat number already taken for the show (both completed + pending)
CREATE OR ALTER PROCEDURE SP_GetBookedSeatNosByShow
    @ShowId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT bs.SeatNo
    FROM dbo.BookingSeats bs
    INNER JOIN dbo.Bookings b ON b.Id = bs.BookingId
    WHERE bs.ShowId = @ShowId
      AND b.IsDeleted = 0
      AND b.PaymentStatus IN ('Completed', 'Paid', 'Pending');
END;
GO

CREATE OR ALTER PROCEDURE SP_InsertBookingSeat
    @Id UNIQUEIDENTIFIER,
    @BookingId UNIQUEIDENTIFIER,
    @ShowId UNIQUEIDENTIFIER,
    @SeatNo NVARCHAR(20),
    @Price INT,
    @Created DATETIME2,
    @Modified DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.BookingSeats (Id, BookingId, ShowId, SeatNo, Price, Created, Modified, IsDeleted)
    VALUES (@Id, @BookingId, @ShowId, @SeatNo, @Price, @Created, @Modified, 0);
END;
GO