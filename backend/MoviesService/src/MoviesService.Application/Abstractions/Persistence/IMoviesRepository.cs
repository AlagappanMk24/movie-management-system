using MoviesService.Domain.Entities;

namespace MoviesService.Application.Abstractions.Persistence
{
    /// <summary>
    /// Repository interface for data access operations related to movies.
    /// </summary>
    public interface IMoviesRepository
    {
        /// <summary>
        /// Retrieves a collection of all movies asynchronously.
        /// </summary>
        /// <returns>A collection of movie entities.</returns>
        Task<IEnumerable<Movies>> GetMoviesAsync();

        /// <summary>
        /// Retrieves a specific movie by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The movie identifier.</param>
        /// <returns>The movie entity if found; otherwise, null.</returns>
        Task<Movies?> GetMoviesByIdAsync(int id);

        /// <summary>
        /// Creates a new movie record in the database asynchronously.
        /// </summary>
        /// <param name="movie">The movie entity to create.</param>
        /// <returns>The created movie entity including its generated identifier.</returns>
        Task<Movies> CreateMoviesAsync(Movies movie);

        /// <summary>
        /// Updates an existing movie record in the database asynchronously.
        /// </summary>
        /// <param name="movie">The movie entity containing updated values.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        Task<bool> UpdateMoviesAsync(Movies movie);

        /// <summary>
        /// Deletes a movie record by its unique identifier asynchronously.
        /// </summary>
        /// <param name="id">The unique identifier of the movie to delete.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        Task<bool> DeleteMoviesAsync(int id);
    }
}