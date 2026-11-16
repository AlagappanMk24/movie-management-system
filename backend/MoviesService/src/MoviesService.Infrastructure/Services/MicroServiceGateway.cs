using Microsoft.Extensions.Configuration;
using MoviesService.Application.Abstractions.Persistence;
using System.Net.Http.Json;

namespace MoviesService.Infrastructure.Services
{
    /// <summary>
    /// Gateway service for communicating with downstream microservices (TheatreMaster and Booking services) via HTTP.
    /// </summary>
    public class MicroServiceGateway(HttpClient httpClient, IConfiguration configuration) : IMicroServiceGateway
    {
        #region Configuration Keys
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _configuration = configuration;
        #endregion

        #region HasShow
        /// <summary>
        /// Checks whether the specified movie has any active shows registered in the TheatreMaster microservice.
        /// </summary>
        /// <param name="movieId">The unique identifier of the movie.</param>
        /// <returns>True if active shows exist; otherwise, false.</returns>
        /// <exception cref="InvalidOperationException">Thrown when TheatreMasterService URL is not configured.</exception>
        public async Task<bool> HasShow(int movieId)
        {
            var baseUrl = _configuration["MicroserviceUrls:TheatreMasterService"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("TheatreMasterService URL not configured");

            var url = $"{baseUrl}/api/Show/GetShowsByMovieId/movie/{movieId}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return false;

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
                return false;

            var shows = System.Text.Json.JsonSerializer.Deserialize<List<object>>(json);
            return shows != null && shows.Count > 0;
        }
        #endregion

        #region HasBooking
        /// <summary>
        /// Checks whether the specified movie has any active bookings registered in the Booking microservice.
        /// </summary>
        /// <param name="movieId">The unique identifier of the movie.</param>
        /// <returns>True if active bookings exist; otherwise, false.</returns>
        /// <exception cref="InvalidOperationException">Thrown when BookingService URL is not configured.</exception>
        public async Task<bool> HasBooking(int movieId)
        {
            var baseUrl = _configuration["MicroServiceUrls:BookingService"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("BookingService URL not configured");

            var url = $"{baseUrl}/api/booking/movie/{movieId}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var bookings = await response.Content.ReadFromJsonAsync<bool>();
                return bookings;
            }
            return false;
        }
        #endregion
    }
}