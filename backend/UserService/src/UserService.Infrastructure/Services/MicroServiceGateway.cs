using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using UserService.Application.Abstractions;

namespace UserService.Infrastructure.Services
{
    /// <summary>
    /// Handles communication with external microservices via HTTP.
    /// </summary>
    public class MicroServiceGateway(HttpClient httpClient, IConfiguration config) : IMicroServiceGateway
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _config = config;

        /// <summary>
        /// Checks if a user has any existing bookings by calling the BookingService.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>True if the user has bookings; otherwise, false.</returns>
        public async Task<bool> HasBooking(Guid userId)
        {
            var baseUrl = _config["MicroserviceUrls:BookingService"];

            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("BookingService URL is not configured.");

            var url = $"{baseUrl}/api/booking/user/{userId}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return false;

            var bookings = await response.Content.ReadFromJsonAsync<List<object>>();
            return bookings != null && bookings.Count > 0;
        }
    }
}