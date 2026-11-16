using MoviesService.Application.DTOs;
using MoviesService.Domain.Entities;

namespace MoviesService.Application.Abstractions
{
    /// <summary>
    /// Service interface defining business operations for movie management.
    /// </summary>
    public interface IMoviesService
    {
        /// <summary>
        /// Retrieves all available movies.
        /// </summary>
        /// <returns>A collection of movie entities.</returns>
        Task<IEnumerable<Movies>> GetMoviesAsync();

        /// <summary>
        /// Retrieves a movie by its unique identifier.
        /// </summary>
        /// <param name="id">The movie identifier.</param>
        /// <returns>The matching movie entity, or null if not found.</returns>
        Task<Movies?> GetMoviesByIdAsync(int id);

        /// <summary>
        /// Creates a new movie from the provided DTO, including file upload processing if applicable.
        /// </summary>
        /// <param name="dto">The movie creation data transfer object.</param>
        /// <returns>The newly created movie entity.</returns>
        Task<Movies> CreateMoviesAsync(MovieCreateUpdateDto dto);

        /// <summary>
        /// Updates an existing movie based on its identifier and provided DTO data.
        /// </summary>
        /// <param name="id">The identifier of the movie to update.</param>
        /// <param name="dto">The updated movie details.</param>
        /// <returns>True if the update succeeded; otherwise, false.</returns>
        Task<bool> UpdateMoviesAsync(int id, MovieCreateUpdateDto dto);

        /// <summary>
        /// Deletes a movie by its identifier after verifying no active shows or bookings depend on it.
        /// </summary>
        /// <param name="id">The identifier of the movie to delete.</param>
        /// <returns>True if deletion succeeded; otherwise, false.</returns>
        Task<bool> DeleteMoviesAsync(int id);
    }
}