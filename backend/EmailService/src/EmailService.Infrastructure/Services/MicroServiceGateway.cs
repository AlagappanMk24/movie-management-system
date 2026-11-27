using EmailService.Application.Abstractions.Services;
using EmailService.Application.DTOs.Responses;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace EmailService.Infrastructure.Services
{
    /// <inheritdoc />
    public class MicroServiceGateway(HttpClient httpClient, IConfiguration configuration) : IMicroServiceGateway
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _configuration = configuration;

        /// <inheritdoc />
        public async Task<byte[]> DownloadTicketAsync(Guid bookingId)
        {
            var baseUrl = _configuration["MicroServiceUrls:BookingService"];
            var response = await _httpClient.GetAsync($"{baseUrl}/api/booking/download/{bookingId}");

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        /// <inheritdoc />
        public async Task<string> GetUserEmailAsync(Guid userId)
        {
            var baseUrl = _configuration["MicroServiceUrls:UserService"];
            var url = $"{baseUrl}/api/user/{userId}";

            var user = await _httpClient.GetFromJsonAsync<UserDto>(url);
            return user?.Email ?? throw new InvalidOperationException($"User email not found for ID {userId}");
        }
    }
}