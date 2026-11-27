using BookingService.Application.Abstractions.Persistence;
using BookingService.Domain.Entities;
using BookingService.Infrastructure.Persistence.Context;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BookingService.Infrastructure.Persistence.Repositories
{
    /// <inheritdoc />
    public class BookingRepository(DapperContext context) : IBookingRepository
    {
        #region Configuration
        private readonly DapperContext _context = context;
        #endregion

        /// <inheritdoc />
        public async Task<IEnumerable<Booking>> GetBookingAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Booking>(
                "SP_GetBookings",
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task<Booking?> GetBookingByIdAsync(Guid bookingId)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Booking>(
                "SP_GetBookingById",
                new { Id = bookingId },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Booking>> GetBookingByUserIdAsync(Guid userId)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Booking>(
                "SP_GetBookingsByUserId",
                new { UserId = userId },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Booking>> GetBookingByShowIdAsync(Guid showId)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Booking>(
                "SP_GetBookingsByShowId",
                new { ShowId = showId },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Booking>> GetBookingByMovieIdAsync(Guid movieId)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Booking>(
                "SP_GetBookingsByMovieId",
                new { MovieId = movieId },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task<int> GetBookedSeatCountByShowAsync(Guid showId)
        {
            using var connection = _context.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(
                "SP_GetBookedSeatCountByShow",
                new { ShowId = showId },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            using var connection = _context.CreateConnection();

            var createdBooking = await connection.QuerySingleAsync<Booking>(
                "SP_CreateBooking",
                new
                {
                    booking.Id,
                    booking.UserId,
                    booking.ShowId,
                    booking.SeatCount,
                    booking.PaymentMethod,
                    booking.PaymentStatus,
                    booking.TotalAmount,
                    booking.Created,
                    booking.Modified,
                    booking.IsDeleted
                },
                commandType: CommandType.StoredProcedure
            );

            return createdBooking;
        }

        public async Task<Booking> CreateBookingWithSeatsAsync(Booking booking, List<BookingSeat> seats)
        {
            using var conn = _context.CreateConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                // Insert booking
                var created = await conn.QuerySingleAsync<Booking>(
                    "SP_CreateBooking",
                    new
                    {
                        booking.Id,
                        booking.UserId,
                        booking.ShowId,
                        booking.SeatCount,
                        booking.PaymentMethod,
                        booking.PaymentStatus,
                        booking.TotalAmount,
                        booking.Created,
                        booking.Modified,
                        booking.IsDeleted
                    },
                    transaction: tx,
                    commandType: CommandType.StoredProcedure);

                // Insert each seat in the same transaction
                foreach (var seat in seats)
                {
                    await conn.ExecuteAsync(
                        "SP_InsertBookingSeat",
                        new
                        {
                            Id = Guid.NewGuid(),
                            BookingId = created.Id,
                            seat.ShowId,
                            seat.SeatNo,
                            seat.Price,
                            seat.Created,
                            seat.Modified
                        },
                        transaction: tx,
                        commandType: CommandType.StoredProcedure);
                }

                tx.Commit();
                return created;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> UpdateBookingAsync(Booking booking)
        {
            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteScalarAsync<int>(
                "SP_UpdateBooking",
                new
                {
                    booking.Id,
                    booking.UserId,
                    booking.ShowId,
                    booking.SeatCount,
                    booking.PaymentMethod,
                    booking.PaymentStatus,
                    booking.TotalAmount,
                    booking.Modified,
                    booking.IsDeleted
                },
                commandType: CommandType.StoredProcedure
            );
            return affectedRows > 0;
        }

        /// <inheritdoc />
        public async Task<List<Booking>> GetCompletedBookingsAsync()
        {
            using var connection = _context.CreateConnection();
            var bookings = await connection.QueryAsync<Booking>(
                "SP_GetCompletedBookings",
                commandType: CommandType.StoredProcedure
            );
            return bookings.ToList();
        }
    }
}