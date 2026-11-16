using BookingService.Application.Abstractions.Services;
using BookingService.Application.DTOs.Responses;
using Microsoft.Extensions.Configuration;
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
        public async Task<bool> GetShow(int showId)
        {
            var baseUrl = _configuration["MicroServiceUrls:TheatreMasterService"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("TheatreMasterService URL not configured");
            var url = $"{baseUrl}/api/show/GetShowById/{showId}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var exists = await response.Content.ReadFromJsonAsync<bool>();
                return exists;
            }
            return false;
        }
        #endregion

        #region UserExists
        /// <inheritdoc />
        public async Task<bool> UserExists(int userId)
        {
            var baseUrl = _configuration["MicroServiceUrls:UserService"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("UserService URL not configured");
            var url = $"{baseUrl}/api/user/GetUserById/{userId}";
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var exists = await response.Content.ReadFromJsonAsync<bool>();
                return exists;
            }
            return false;
        }
        #endregion

        #region GetShowDetails
        /// <inheritdoc />
        public async Task<BookingShow> GetShowDetailsAsync(int showId)
        {
            var baseUrl = _configuration["MicroServiceUrls:TheatreMasterService"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("TheatreMasterService URL not configured");

            var url = $"{baseUrl}/api/Show/GetShowById/{showId}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to fetch show details");

            var showDetails =
             await response.Content.ReadFromJsonAsync<BookingShow>(
                 new JsonSerializerOptions
                 {
                     PropertyNameCaseInsensitive = true
                 }
            );
            return showDetails!;
        }
        #endregion

        #region GetMovieAsync
        /// <inheritdoc />
        public async Task<MovieDto> GetMovieAsync(int movieId)
        {
            var baseUrl = _configuration["MicroServiceUrls:Movieservice"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("MovieService URL not configured");

            var url = $"{baseUrl}/api/Movies/GetMovieById/{movieId}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to fetch movie");

            var movie = await response.Content.ReadFromJsonAsync<MovieDto>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return movie!;
        }
        #endregion

        #region GetuserAsync
        /// <inheritdoc />
        public async Task<UserDto> GetUserAsync(int userId)
        {
            var baseUrl = _configuration["MicroServiceUrls:UserService"];
            var url = $"{baseUrl}/api/user/GetUserById/{userId}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to fetch user");

            return await response.Content.ReadFromJsonAsync<UserDto>()!;
        }
        #endregion

        #region GetTheatreAsync
        /// <inheritdoc />
        public async Task<TheatreDto> GetTheatreAsync(int theatreId)
        {
            var baseUrl = _configuration["MicroServiceUrls:TheatreMasterService"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("TheatreMasterService URL not configured");
            var url = $"{baseUrl}/api/Theatre/GetTheatreById/{theatreId}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to fetch theatre details");
            var theatreDetails =
             await response.Content.ReadFromJsonAsync<TheatreDto>(
                 new JsonSerializerOptions
                 {
                     PropertyNameCaseInsensitive = true
                 }
            );
            return theatreDetails!;
        }
        #endregion

        #region GetScreenAsync
        /// <inheritdoc />
        public async Task<ScreenDto> GetScreenAsync(int screenId)
        {
            var baseUrl = _configuration["MicroServiceUrls:TheatreMasterService"];
            if (string.IsNullOrEmpty(baseUrl))
                throw new InvalidOperationException("TheatreMasterService URL not configured");

            var url = $"{baseUrl}/api/Screen/GetScreenById/{screenId}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to fetch screen details");

            var screen = await response.Content.ReadFromJsonAsync<ScreenDto>(
                     new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                 );

            return screen!;
        }
        #endregion

        #region DownloadImageAsync
        /// <inheritdoc />
        public async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            var response = await _httpClient.GetAsync(imageUrl);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to download image");

            return await response.Content.ReadAsByteArrayAsync();
        }
        #endregion
    }
}