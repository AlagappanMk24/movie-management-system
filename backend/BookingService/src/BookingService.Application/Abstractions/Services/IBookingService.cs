using BookingService.Application.DTOs.Analytics;
using BookingService.Application.DTOs.Requests;
using BookingService.Application.DTOs.Responses;
using BookingService.Domain.Entities;

namespace BookingService.Application.Abstractions.Services
{
    /// <summary>
    /// Defines business use cases and operations for booking management.
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Retrieves all bookings.
        /// </summary>
        Task<IEnumerable<BookingResponse>> GetBookingsAsync();

        /// <summary>
        /// Retrieves a booking by its unique identifier.
        /// </summary>
        Task<BookingResponse?> GetBookingByIdAsync(Guid bookingId);

        /// <summary>
        /// Retrieves bookings for a specific user.
        /// </summary>
        Task<IEnumerable<BookingResponse>> GetBookingsByUserIdAsync(Guid userId);

        /// <summary>
        /// Retrieves bookings for a specific show.
        /// </summary>
        Task<IEnumerable<BookingResponse>> GetBookingsByShowIdAsync(Guid showId);

        /// <summary>
        /// Retrieves all bookings associated with a specific movie.
        /// </summary>
        Task<IEnumerable<BookingResponse>> GetBookingsByMovieIdAsync(Guid movieId);

        /// <summary>
        /// Gets the total count of booked seats for a show.
        /// </summary>
        Task<int> GetBookedSeatCountByShowAsync(Guid showId);

        /// <summary>
        /// Gets all seat numbers assigned to a booking.
        /// </summary>
        Task<List<string>> GetBookedSeatsAsync(Guid bookingId);

        /// <summary>
        /// Creates a basic booking record.
        /// </summary>
        Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request);

        /// <summary>
        /// Creates a complete booking with seat allocations transactionally.
        /// </summary>
        Task<Guid> CreateBookingWithSeatsAsync(CreateBookingWithSeatsRequest request);

        /// <summary>
        /// Bulk books and locks multiple seats for a show.
        /// </summary>
        Task BulkBookSeatsAsync(Guid bookingId, Guid showId, List<string> seatNos);

        /// <summary>
        /// Cancels an existing booking by identifier.
        /// </summary>
        Task<bool> CancelBookingAsync(Guid bookingId);

        /// <summary>
        /// Updates the status of a booking.
        /// </summary>
        Task<bool> UpdateBookingStatusAsync(Guid bookingId, string status);

        /// <summary>
        /// Calculates box-office collection revenue grouped by movie title.
        /// </summary>
        Task<List<MovieCollectionDto>> GetMovieWiseCollectionAsync();
    }
}