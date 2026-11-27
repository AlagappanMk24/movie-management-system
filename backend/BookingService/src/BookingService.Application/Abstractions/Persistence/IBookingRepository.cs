using BookingService.Domain.Entities;

namespace BookingService.Application.Abstractions.Persistence
{
    /// <summary>
    /// Defines repository operations for booking entity data persistence.
    /// </summary>
    public interface IBookingRepository
    {
        /// <summary>
        /// Retrieves all bookings in the system.
        /// </summary>
        /// <returns>A collection of booking entities.</returns>
        Task<IEnumerable<Booking>> GetBookingAsync();

        /// <summary>
        /// Retrieves a specific booking by its unique identifier.
        /// </summary>
        /// <param name="bookingId">The unique booking identifier.</param>
        /// <returns>The matching booking entity, or null if not found.</returns>
        Task<Booking?> GetBookingByIdAsync(Guid bookingId);

        /// <summary>
        /// Retrieves all bookings associated with a specific user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>A collection of bookings for the user.</returns>
        Task<IEnumerable<Booking>> GetBookingByUserIdAsync(Guid userId);

        /// <summary>
        /// Retrieves all bookings associated with a specific show identifier.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <returns>A collection of bookings for the show.</returns>
        Task<IEnumerable<Booking>> GetBookingByShowIdAsync(Guid showId);

        /// <summary>
        /// Retrieves all bookings associated with a specific movie identifier.
        /// </summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>A collection of bookings for the movie.</returns>
        Task<IEnumerable<Booking>> GetBookingByMovieIdAsync(Guid movieId);

        /// <summary>
        /// Creates a new booking record in the database.
        /// </summary>
        /// <param name="booking">The booking entity to create.</param>
        /// <returns>The created booking entity.</returns>
        Task<Booking> CreateBookingAsync(Booking booking);

        /// <summary>
        /// Updates an existing booking record.
        /// </summary>
        /// <param name="booking">The booking entity containing updated properties.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        Task<bool> UpdateBookingAsync(Booking booking);

        /// <summary>
        /// Gets the total count of successfully booked seats for a specific show.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <returns>The total number of booked seats.</returns>
        Task<int> GetBookedSeatCountByShowAsync(Guid showId);

        /// <summary>
        /// Retrieves all completed or paid bookings for financial analysis.
        /// </summary>
        /// <returns>A list of completed bookings.</returns>
        Task<List<Booking>> GetCompletedBookingsAsync();

        Task<Booking> CreateBookingWithSeatsAsync(Booking booking, List<BookingSeat> seats);
    }
}