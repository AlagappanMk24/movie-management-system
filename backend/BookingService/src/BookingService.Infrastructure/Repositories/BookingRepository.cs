using BookingService.Application.Abstractions.Persistence;
using BookingService.Domain.Entities;
using BookingService.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Infrastructure.Repositories
{
    /// <inheritdoc />
    public class BookingRepository(BookingServiceDbContext context) : IBookingRepository
    {
        #region Configuration
        private readonly BookingServiceDbContext _context = context;
        #endregion

        /// <inheritdoc />
        public async Task<IEnumerable<Booking>> GetBookingAsync()
        {
            return await _context.Bookings.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Booking>> GetBookingByShowIdAsync(int showid)
        {
            var bookings = await _context.Bookings
                .Where(b => b.ShowId == showid)
                .ToListAsync();
            return bookings;
        }

        /// <inheritdoc />
        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings.FindAsync(bookingId);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Booking>> GetBookingByUserIdAsync(int userId)
        {
            var bookings = await _context.Bookings
                .Where(b => b.UserId == userId)
                .ToListAsync();
            return bookings;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateBookingAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <inheritdoc />
        public async Task<int> GetBookedSeatCountByShowAsync(int showId)
        {
            return await _context.Bookings
                .Where(b => b.ShowId == showId && b.PaymentStatus == "Completed")
                .SumAsync(b => b.SeatCount);
        }

        /// <inheritdoc />
        public async Task<List<Booking>> GetCompletedBookingsAsync()
        {
            return await _context.Bookings
                .Where(b => b.PaymentStatus == "Completed" || b.PaymentStatus == "Paid")
                .ToListAsync();
        }
    }
}