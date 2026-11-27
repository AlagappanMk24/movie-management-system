using BookingService.Application.DTOs.Responses;

namespace BookingService.Application.Abstractions.Services
{
    /// <summary>
    /// Defines contract methods for communicating with downstream microservices via HTTP.
    /// </summary>
    public interface IMicroServiceGateway
    {
        /// <summary>
        /// Checks whether a show exists by its unique identifier.
        /// </summary>
        /// <param name="showId">The unique identifier of the show.</param>
        /// <returns>True if the show exists; otherwise, false.</returns>
        Task<bool> GetShow(Guid showId);

        /// <summary>
        /// Checks whether a user exists by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>True if the user exists; otherwise, false.</returns>
        Task<bool> UserExists(Guid userId);

        /// <summary>
        /// Retrieves detailed information about a show.
        /// </summary>
        /// <param name="showId">The unique identifier of the show.</param>
        /// <returns>A <see cref="BookingShow"/> object containing show details.</returns>
        Task<BookingShow> GetShowDetailsAsync(Guid showId);

        /// <summary>
        /// Retrieves all shows scheduled for a specific movie identifier.
        /// </summary>
        /// <param name="movieId">The unique identifier of the movie.</param>
        /// <returns>A list of <see cref="BookingShow"/> objects associated with the movie.</returns>
        Task<List<BookingShow>> GetShowsByMovieIdAsync(Guid movieId);

        /// <summary>
        /// Retrieves movie details by its identifier.
        /// </summary>
        /// <param name="movieId">The unique identifier of the movie.</param>
        /// <returns>A <see cref="MovieDto"/> object containing movie details.</returns>
        Task<MovieDto> GetMovieAsync(Guid movieId);

        /// <summary>
        /// Retrieves user details by their identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A <see cref="UserDto"/> object containing user details.</returns>
        Task<UserDto> GetUserAsync(Guid userId);

        /// <summary>
        /// Retrieves theatre details by its identifier.
        /// </summary>
        /// <param name="theatreId">The unique identifier of the theatre.</param>
        /// <returns>A <see cref="TheatreDto"/> object containing theatre details.</returns>
        Task<TheatreDto> GetTheatreAsync(Guid theatreId);

        /// <summary>
        /// Retrieves screen details by its identifier.
        /// </summary>
        /// <param name="screenId">The unique identifier of the screen.</param>
        /// <returns>A <see cref="ScreenDto"/> object containing screen details.</returns>
        Task<ScreenDto> GetScreenAsync(Guid screenId);

        /// <summary>
        /// Downloads an image as a byte array from the given URL.
        /// </summary>
        /// <param name="imageUrl">The absolute URL of the image.</param>
        /// <returns>A byte array representing the image file contents.</returns>
        Task<byte[]> DownloadImageAsync(string imageUrl);

        Task<ScreenLayoutDto> GetScreenLayoutAsync(Guid showId);
    }
}