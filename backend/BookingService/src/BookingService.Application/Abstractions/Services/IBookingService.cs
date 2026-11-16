using BookingService.Application.DTOs.Analytics;
using BookingService.Application.DTOs.Requests;
using BookingService.Domain.Entities;

namespace BookingService.Application.Abstractions.Services
{
    /// <summary>
    /// Defines business use cases and operations for booking management.
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Bulk books and locks multiple seats for a show.
        /// </summary>
        Task BulkBookSeatsAsync(int bookingId, int showId, List<string> seatNos);

        /// <summary>
        /// Cancels an existing booking by identifier.
        /// </summary>
        Task<bool> CancelBookingAsync(int bookingId);

        /// <summary>
        /// Creates a basic booking record.
        /// </summary>
        Task<Booking> CreateBookingAsync(Booking booking);

        /// <summary>
        /// Creates a complete booking with seat allocations transactionally.
        /// </summary>
        Task<int> CreateBookingWithSeatsAsync(CreateBookingWithSeatsRequest request);

        /// <summary>
        /// Gets the count of booked seats for a show.
        /// </summary>
        Task<int> GetBookedSeatCountByShowAsync(int showId);

        /// <summary>
        /// Gets all seat numbers assigned to a booking.
        /// </summary>
        Task<List<string>> GetBookedSeatsAsync(int bookingId);

        /// <summary>
        /// Retrieves all bookings.
        /// </summary>
        Task<IEnumerable<Booking>> GetBookingAsync();

        /// <summary>
        /// Retrieves a booking by its identifier.
        /// </summary>
        Task<Booking?> GetBookingByIdAsync(int bookingId);

        /// <summary>
        /// Retrieves bookings for a specific show.
        /// </summary>
        Task<IEnumerable<Booking>> GetBookingByShowIdAsync(int showId);

        /// <summary>
        /// Retrieves bookings for a specific user.
        /// </summary>
        Task<IEnumerable<Booking>> GetBookingByUserIdAsync(int userId);

        /// <summary>
        /// Updates the status of a booking.
        /// </summary>
        Task<bool> UpdateBookingStatusAsync(int bookingId, string status);

        /// <summary>
        /// Calculates box-office collection revenue grouped by movie title.
        /// </summary>
        Task<List<MovieCollectionDto>> GetMovieWiseCollectionAsync();
    }
}