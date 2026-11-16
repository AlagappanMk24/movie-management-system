using BookingService.Application.Abstractions.Caching;
using BookingService.Application.Abstractions.Persistence;
using BookingService.Application.Abstractions.Services;
using BookingService.Application.DTOs.Analytics;

namespace BookingService.Application.Services
{
    /// <inheritdoc />
    public class BookingAnalyticsService(
        IBookingRepository bookingRepository,
        IMicroServiceGateway gateway,
        ICacheService cache) : IBookingAnalyticsService
    {
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly IMicroServiceGateway _gateway = gateway;
        private readonly ICacheService _cache = cache;

        /// <inheritdoc />
        public async Task<List<MovieCollectionDto>> GetMovieWiseCollectionAsync()
        {
            return await _cache.GetOrSetAsync("movie_collection", async () =>
            {
                var bookings = await _bookingRepository.GetCompletedBookingsAsync();
                var result = new Dictionary<string, int>();

                foreach (var booking in bookings)
                {
                    try
                    {
                        var showDetails = await _gateway.GetShowDetailsAsync(booking.ShowId);
                        var movie = await _gateway.GetMovieAsync(showDetails.MovieId);
                        if (movie == null) continue;

                        result[movie.Title] = result.GetValueOrDefault(movie.Title) + booking.TotalAmount;
                    }
                    catch
                    {
                        continue;
                    }
                }

                return result
                    .Select(kvp => new MovieCollectionDto { Title = kvp.Key, MovieCollection = kvp.Value })
                    .OrderByDescending(x => x.MovieCollection)
                    .ToList();
            }, TimeSpan.FromMinutes(10)) ?? [];
        }
    }
}