using BookingService.Application.Abstractions.Services;
using BookingService.Application.DTOs.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Shared.Contracts.Common;
using System.Net.Http.Json;
using System.Text.Json;

namespace BookingService.Infrastructure.Services
{
    /// <summary>
    /// Gateway service for communicating with downstream microservices via HTTP.
    /// </summary>
    public class MicroServiceGateway(HttpClient httpClient, IConfiguration configuration) : IMicroServiceGateway
    {
        #region Configuration Keys
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _configuration = configuration;
        #endregion

        #region ShowExists
        /// <inheritdoc />
        public async Task<bool> GetShow(Guid showId)
        {
            try
            {
                await GetShowDetailsAsync(showId);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region UserExists
        /// <inheritdoc />
        public async Task<bool> UserExists(Guid userId)
        {
            var baseUrl = _configuration["MicroServiceUrls:UserService"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("UserService URL not configured");

            var url = $"{baseUrl}/api/user/{userId}";

            var response = await _httpClient.GetAsync(url);

            // Most common pattern: 200 = exists, 404 = does not exist
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            if (response.IsSuccessStatusCode)
                return true;

            // Optional: log the unexpected status
            return false;
        }
        #endregion

        #region GetShowDetails
        /// <inheritdoc />
        public async Task<BookingShow> GetShowDetailsAsync(Guid showId)
        {
            var baseUrl = _configuration["MicroServiceUrls:TheatreMasterService"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("TheatreMasterService URL not configured");

            var url = $"{baseUrl}/api/show/{showId}";

            using var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to fetch show details");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<BookingShow>>(options);

            if (apiResponse is not { Success: true, Data: not null })
                throw new Exception(apiResponse?.Message is { Length: > 0 } msg
                    ? msg
                    : "Show details payload was empty or unsuccessful");

            return apiResponse.Data;
        }
        #endregion

        #region GetShowsByMovieIdAsync
        /// <inheritdoc />
        public async Task<List<BookingShow>> GetShowsByMovieIdAsync(Guid movieId)
        {
            var baseUrl = _configuration["MicroServiceUrls:TheatreMasterService"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("TheatreMasterService URL not configured");

            var url = $"{baseUrl}/api/show/movie/{movieId}";
            using var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to fetch shows for movie");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<BookingShow>>>(options);

            return apiResponse is { Success: true, Data: not null }
                ? apiResponse.Data
                : [];
        }
        #endregion

        #region GetMovieAsync
        /// <inheritdoc />
        public async Task<MovieDto> GetMovieAsync(Guid movieId)
        {
            var baseUrl = _configuration["MicroServiceUrls:Movieservice"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("MovieService URL not configured");

            var url = $"{baseUrl}/api/movies/{movieId}";
            using var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to fetch movie");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<MovieDto>>(options);

            if (apiResponse is not { Success: true, Data: not null })
                throw new Exception(apiResponse?.Message is { Length: > 0 } msg
                    ? msg
                    : "Movie details payload was empty or unsuccessful");

            return apiResponse.Data;
        }
        #endregion

        #region GetuserAsync
        /// <inheritdoc />
        public async Task<UserDto> GetUserAsync(Guid userId)
        {
            var baseUrl = _configuration["MicroServiceUrls:UserService"];
            var url = $"{baseUrl}/api/user/{userId}";

            using var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to fetch user");

            return await response.Content.ReadFromJsonAsync<UserDto>()!;
        }
        #endregion

        #region GetTheatreAsync
        /// <inheritdoc />
        public async Task<TheatreDto> GetTheatreAsync(Guid theatreId)
        {
            var baseUrl = _configuration["MicroServiceUrls:TheatreMasterService"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("TheatreMasterService URL not configured");

            var url = $"{baseUrl}/api/theatre/{theatreId}";

            using var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to fetch theatre details");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<TheatreDto>>(options);

            if (apiResponse is not { Success: true, Data: not null })
                throw new Exception(apiResponse?.Message is { Length: > 0 } msg
                    ? msg
                    : "Theatre details payload was empty or unsuccessful");

            return apiResponse.Data;
        }
        #endregion

        #region GetScreenAsync
        /// <inheritdoc />
        public async Task<ScreenDto> GetScreenAsync(Guid screenId)
        {
            var baseUrl = _configuration["MicroServiceUrls:TheatreMasterService"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("TheatreMasterService URL not configured");

            var url = $"{baseUrl}/api/screen/{screenId}";
            using var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to fetch screen details");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ScreenDto>>(options);

            if (apiResponse is not { Success: true, Data: not null })
                throw new Exception(apiResponse?.Message is { Length: > 0 } msg
                    ? msg
                    : "Screen details payload was empty or unsuccessful");

            return apiResponse.Data;
        }
        #endregion

        #region DownloadImageAsync
        /// <inheritdoc />
        public async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            using var response = await _httpClient.GetAsync(imageUrl);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to download image");

            return await response.Content.ReadAsByteArrayAsync();
        }
        #endregion

        public async Task<ScreenLayoutDto> GetScreenLayoutAsync(Guid showId)
        {
            var baseUrl = _configuration["MicroServiceUrls:TheatreMasterService"]
                ?? throw new InvalidOperationException("TheatreMasterService URL not configured.");

            using var response = await _httpClient.GetAsync($"{baseUrl}/api/show/{showId}/screen-layout");
            response.EnsureSuccessStatusCode();

            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var api = await response.Content.ReadFromJsonAsync<ApiResponse<ScreenLayoutDto>>(opts);
            return api is { Success: true, Data: not null }
                ? api.Data
                : throw new InvalidOperationException("Screen layout unavailable.");
        }
    }
}