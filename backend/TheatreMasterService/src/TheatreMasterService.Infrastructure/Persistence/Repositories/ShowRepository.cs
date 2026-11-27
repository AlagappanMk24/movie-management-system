using Microsoft.EntityFrameworkCore;
using System.Data;
using TheatreMasterService.Application.Abstractions.Persistence;
using TheatreMasterService.Domain.Entities;
using TheatreMasterService.Infrastructure.Persistence.Context;
using Dapper;

namespace TheatreMasterService.Infrastructure.Persistence.Repositories
{
    /// <inheritdoc />
    public class ShowRepository(DapperContext context) : IShowRepository
    {
        #region Configuration
        private readonly DapperContext _context = context;
        #endregion

        /// <inheritdoc />
        public async Task<IEnumerable<Show>> GetShowsAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Show>(
                "SP_GetShows",
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task<Show?> GetShowByIdAsync(Guid id)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Show>(
                "SP_GetShowById",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task<Show> CreateShowAsync(Show show)
        {
            using var connection = _context.CreateConnection();

            show.Id = show.Id == Guid.Empty ? Guid.NewGuid() : show.Id;
            show.Created = DateTime.UtcNow;
            show.Modified = DateTime.UtcNow;
            show.IsDeleted = false;

            var createdShow = await connection.QuerySingleAsync<Show>(
                "SP_CreateShow",
                new
                {
                    show.Id,
                    show.MovieId,
                    show.ScreenId,
                    show.ShowDate,
                    show.ShowTime,
                    show.Price,
                    show.Created,
                    show.Modified,
                    show.IsDeleted
                },
                commandType: CommandType.StoredProcedure
            );

            return createdShow;
        }

        /// <inheritdoc />
        public async Task<bool> UpdateShowAsync(Show show)
        {
            using var connection = _context.CreateConnection();

            var affectedRows = await connection.ExecuteScalarAsync<int>(
                "SP_UpdateShow",
                new
                {
                    Id = show.Id,
                    show.MovieId,
                    show.ScreenId,
                    show.ShowDate,
                    show.ShowTime,
                    show.Price,
                    Modified = DateTime.UtcNow,
                    show.IsDeleted
                },
                commandType: CommandType.StoredProcedure
            );

            return affectedRows > 0;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteShowAsync(Guid id)
        {
            using var connection = _context.CreateConnection();

            var affectedRows = await connection.ExecuteScalarAsync<int>(
               "SP_DeleteShow",
               new { Id = id },
               commandType: CommandType.StoredProcedure
           );

            return affectedRows > 0;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Show>> GetShowsByMovieId(Guid movieId)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Show>(
                "SP_GetShowsByMovieId",
                new { MovieId = movieId },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}