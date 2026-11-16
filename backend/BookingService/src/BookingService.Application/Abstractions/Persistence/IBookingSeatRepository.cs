using BookingService.Application.DTOs.Excel;

namespace BookingService.Application.Abstractions.Persistence
{
    /// <summary>
    /// Defines repository operations for seat reservations and bulk insertions.
    /// </summary>
    public interface IBookingSeatRepository
    {
        /// <summary>
        /// Bulk inserts booked seats using SQL Table-Valued Parameters.
        /// </summary>
        /// <param name="bookingId">The booking identifier.</param>
        /// <param name="showId">The show identifier.</param>
        /// <param name="seats">The list of seat TVP DTO records.</param>
        Task BulkInsertSeatsAsync(int bookingId, int showId, List<BookingSeatTvpDto> seats);

        /// <summary>
        /// Retrieves a list of booked seat numbers associated with a specific booking identifier.
        /// </summary>
        /// <param name="bookingId">The booking identifier.</param>
        /// <returns>A list of seat numbers as strings.</returns>
        Task<List<string>> GetBookedSeatsByBookingIdAsync(int bookingId);
    }
}