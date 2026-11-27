using Dapper;
using Microsoft.EntityFrameworkCore;
using MoviesService.Application.Abstractions.Persistence;
using MoviesService.Domain.Entities;
using MoviesService.Infrastructure.Persistence.Context;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MoviesService.Infrastructure.Persistence.Repositories
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
        /// <inheritdoc/>
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
        /// <inheritdoc/>
        public async Task<Movies?> GetMoviesByIdAsync(Guid id)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Movies>(
                "SP_GetMovieById",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );
        }
        #endregion

        #region CreateMoviesAsync
        /// <inheritdoc/>
        public async Task<Movies> CreateMoviesAsync(Movies movie)
        {
            using var connection = _context.CreateConnection();

            var createdMovie = await connection.QuerySingleAsync<Movies>(
                "SP_CreateMovie",
                new
                {
                    movie.Id,
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
                    movie.TrailerUrl,
                    movie.Created,
                    movie.Modified,
                    movie.IsDeleted
                },
                commandType: CommandType.StoredProcedure
            );

            return createdMovie;
        }
        #endregion

        #region UpdateMoviesAsync
        /// <inheritdoc/>
        public async Task<Movies?> UpdateMoviesAsync(Movies movie)
        {
            using var connection = _context.CreateConnection();

            // 1. Execute the update command
            await connection.ExecuteAsync(
                "SP_UpdateMovie",
                new
                {
                    movie.Id,
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
                    movie.TrailerUrl,
                    movie.Modified
                },
                commandType: CommandType.StoredProcedure
            );

            // 2. Fetch and return the freshly updated record using your existing mapper
            var updatedMovie = await connection.QueryFirstOrDefaultAsync<Movies>(
                "SP_GetMovieById",
                new { Id = movie.Id },
                commandType: CommandType.StoredProcedure
            );

            return updatedMovie;
        }
        #endregion

        #region DeleteMoviesAsync
        /// <inheritdoc/>
        public async Task<bool> DeleteMoviesAsync(Guid id)
        {
            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteScalarAsync<int>(
                 "SP_DeleteMovie",
                 new { Id = id }, 
                 commandType: CommandType.StoredProcedure
             );
            return affectedRows > 0;
        }
        #endregion
    }
}

// ----- Ensure both actions happen in a single atomic database transaction, you can wrap them inside a SQL transaction in your repository like this:

//public async Task<Movies?> UpdateMoviesAsync(Movies movie)
//{
//    using var connection = _context.CreateConnection();
//    connection.Open();
//    using var transaction = connection.BeginTransaction();

//    try
//    {
//        // 1. Execute update
//        await connection.ExecuteAsync(
//            "SP_UpdateMovie",
//            new { movie.Id, movie.Title, movie.Genre, movie.ReleaseDate, movie.Director, movie.Description, movie.Actor, movie.Actress, movie.Language, movie.DurationMinutes, movie.PosterUrl, movie.TrailerUrl, movie.Modified },
//            transaction: transaction,
//            commandType: CommandType.StoredProcedure
//        );

//        // 2. Fetch updated record
//        var updatedMovie = await connection.QueryFirstOrDefaultAsync<Movies>(
//            "SP_GetMovieById",
//            new { Id = movie.Id },
//            transaction: transaction,
//            commandType: CommandType.StoredProcedure
//        );

//        transaction.Commit();
//        return updatedMovie;
//    }
//    catch
//    {
//        transaction.Rollback();
//        throw;
//    }
//}