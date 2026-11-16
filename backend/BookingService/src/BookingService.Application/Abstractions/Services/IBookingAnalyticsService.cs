using BookingService.Application.DTOs.Analytics;

namespace BookingService.Application.Abstractions.Services
{
    /// <summary>
    /// Provides aggregated financial and collection reports for bookings.
    /// </summary>
    public interface IBookingAnalyticsService
    {
        /// <summary>
        /// Calculates total box-office revenue grouped by movie title.
        /// </summary>
        Task<List<MovieCollectionDto>> GetMovieWiseCollectionAsync();
    }
}