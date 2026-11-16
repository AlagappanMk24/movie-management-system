using Microsoft.Extensions.Configuration;
using System.Data;
using Dapper;
using MoviesService.Application.Abstractions.Persistence;
using MoviesService.Domain.Entities;
using MoviesService.Infrastructure.Persistence.Context;

namespace MoviesService.Infrastructure.Repositories
{
    /// <summary>
    /// Implements <see cref="IMoviesRepository"/> using Dapper and stored procedures for high-performance data access.
    /// </summary>
    public class MoviesRepository(DapperContext context) : IMoviesRepository
    {
        #region Configuration
        private readonly DapperContext _context = context;
        #endregion

        #region GetMovies
        /// <summary>
        /// Retrieves all movie records asynchronously using stored procedure execution.
        /// </summary>
        /// <returns>A collection of movie entities.</returns>
        public async Task<IEnumerable<Movies>> GetMoviesAsync()
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Movies>(
                "SP_GetMovie",
                commandType: CommandType.StoredProcedure
               );
        }
        #endregion

        #region GetMoviesByIdAsync
        /// <summary>
        /// Retrieves a single movie by its unique identifier using stored procedure execution.
        /// </summary>
        /// <param name="id">The movie identifier.</param>
        /// <returns>The matching movie entity, or null if not found.</returns>
        public async Task<Movies?> GetMoviesByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Movies>(
                "SP_GetMovieById",
                new { MovieId = id },
                commandType: CommandType.StoredProcedure
               );
        }
        #endregion

        #region CreateMoviesAsync
        /// <summary>
        /// Inserts a new movie record into the database via stored procedure and assigns the generated identifier.
        /// </summary>
        /// <param name="movie">The movie entity to create.</param>
        /// <returns>The created movie entity with its new ID.</returns>
        public async Task<Movies> CreateMoviesAsync(Movies movie)
        {
            using var connection = _context.CreateConnection();
            var movieId = await connection.QuerySingleAsync<int>(
                "SP_CreateMovie",
                new
                {
                    movie.Title,
                    movie.Genre,
                    movie.ReleaseDate,
                    movie.Director,
                    movie.Description,
                    movie.Actor,
                    movie.Actress,
                    movie.Language,
                    movie.DurationMinutes,
                    movie.PosterUrl,
                    movie.TrailerUrl
                },
                commandType: CommandType.StoredProcedure
               );
            movie.MovieId = movieId;
            return movie;
        }
        #endregion

        #region UpdateMoviesAsync
        /// <summary>
        /// Updates an existing movie record in the database using a stored procedure.
        /// </summary>
        /// <param name="movie">The movie entity containing updated values.</param>
        /// <returns>True if rows were affected; otherwise, false.</returns>
        public async Task<bool> UpdateMoviesAsync(Movies movie)
        {
            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(
                "SP_UpdateMovie",
                new
                {
                    movie.MovieId,
                    movie.Title,
                    movie.Genre,
                    movie.ReleaseDate,
                    movie.Director,
                    movie.Description,
                    movie.Actor,
                    movie.Actress,
                    movie.Language,
                    movie.DurationMinutes,
                    movie.PosterUrl,
                    movie.TrailerUrl
                },
                commandType: CommandType.StoredProcedure
               );
            return affectedRows > 0;
        }
        #endregion

        #region DeleteMoviesAsync
        /// <summary>
        /// Deletes a movie record by its identifier using a stored procedure.
        /// </summary>
        /// <param name="id">The movie identifier.</param>
        /// <returns>True if deletion affected rows; otherwise, false.</returns>
        public async Task<bool> DeleteMoviesAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(
                "SP_DeleteMovie",
                new { MovieId = id },
                commandType: CommandType.StoredProcedure
               );
            return affectedRows > 0;
        }
        #endregion
    }
}