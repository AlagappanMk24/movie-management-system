using MoviesService.Domain.Entities;

namespace MoviesService.Application.Abstractions.Persistence
{
    /// <summary>
    /// Repository interface for data access operations related to movies.
    /// </summary>
    public interface IMoviesRepository
    {
        /// <summary>
        /// Retrieves all movie records asynchronously using stored procedure execution.
        /// </summary>
        /// <returns>A collection of movie entities.</returns>
        Task<IEnumerable<Movies>> GetMoviesAsync();

        /// <summary>
        /// Retrieves a single movie by its unique identifier using stored procedure execution.
        /// </summary>
        /// <param name="id">The movie identifier.</param>
        /// <returns>The matching movie entity, or null if not found.</returns>
        Task<Movies?> GetMoviesByIdAsync(Guid id);

        /// <summary>
        /// Inserts a new movie record into the database via stored procedure and assigns the generated identifier.
        /// </summary>
        /// <param name="movie">The movie entity to create.</param>
        /// <returns>The created movie entity with its new ID.</returns>
        Task<Movies> CreateMoviesAsync(Movies movie);

        /// <summary>
        /// Updates an existing movie record in the database using a stored procedure.
        /// </summary>
        /// <param name="movie">The movie entity containing updated values.</param>
        /// <returns>True if rows were affected; otherwise, false.</returns>
        Task<Movies?> UpdateMoviesAsync(Movies movie);

        /// <summary>
        /// Deletes a movie record by its identifier using a stored procedure.
        /// </summary>
        /// <param name="id">The movie identifier.</param>
        /// <returns>True if deletion affected rows; otherwise, false.</returns>
        Task<bool> DeleteMoviesAsync(Guid id);
    }
}