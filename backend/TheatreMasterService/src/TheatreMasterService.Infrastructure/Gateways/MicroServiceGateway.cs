using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using TheatreMasterService.Application.Abstractions.Services;
using TheatreMasterService.Application.DTOs.Shows.Responses;

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
        public async Task<bool> HasBookingByShow(Guid showId)
        {
            var baseUrl = _configuration["MicroServiceUrls:BookingService"];

            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("BookingService URL not configured");

            var url = $"{baseUrl}/api/booking/show/{showId}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return false;

            var payload = await response.Content.ReadFromJsonAsync<BookingsEnvelope>();
            return payload?.Data is { Count: > 0 };
        }

        /// <inheritdoc />
        public async Task<int> GetBookedSeatsByShowAsync(Guid showId)
        {
            var baseUrl = _configuration["MicroServiceUrls:BookingService"];

            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("BookingService URL not configured");

            var response = await _httpClient.GetAsync(
                $"{baseUrl}/api/booking/show/{showId}/seatcount"
            );

            if (!response.IsSuccessStatusCode)
                return 0;

            var payload = await response.Content.ReadFromJsonAsync<SeatCountEnvelope>();
            return payload?.Data?.BookedSeatCount ?? 0;
        }
    }
}