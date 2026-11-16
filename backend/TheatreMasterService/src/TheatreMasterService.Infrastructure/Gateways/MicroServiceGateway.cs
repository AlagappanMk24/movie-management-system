using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using TheatreMasterService.Application.Abstractions.Services;

namespace TheatreMasterService.Infrastructure.Gateways
{
    /// <inheritdoc />
    public class MicroServiceGateway(HttpClient httpClient, IConfiguration configuration) : IMicroServiceGateway
    {
        #region Configuration Fields
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _configuration = configuration;
        #endregion

        /// <inheritdoc />
        public async Task<bool> HasBookingByShow(int showId)
        {
            var baseUrl = _configuration["MicroServiceUrls:BookingService"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("BookingService URL not configured");

            var url = $"{baseUrl}/api/booking/GetBookingsByShow/show/{showId}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var bookings = await response.Content.ReadFromJsonAsync<List<object>>();
                return bookings != null && bookings.Count > 0;
            }

            return false;
        }

        /// <inheritdoc />
        public async Task<int> GetBookedSeatsByShowAsync(int showId)
        {
            var baseUrl = _configuration["MicroServiceUrls:BookingService"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("BookingService URL not configured");

            var response = await _httpClient.GetAsync(
                $"{baseUrl}/api/Booking/GetBookedSeatCount/show/{showId}/seatcount"
            );

            if (!response.IsSuccessStatusCode)
                return 0;

            return await response.Content.ReadFromJsonAsync<int>();
        }
    }
}