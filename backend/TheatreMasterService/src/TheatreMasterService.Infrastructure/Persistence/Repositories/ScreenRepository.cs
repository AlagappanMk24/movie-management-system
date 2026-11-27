using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using TheatreMasterService.Application.Abstractions.Persistence;
using TheatreMasterService.Domain.Entities;
using TheatreMasterService.Infrastructure.Persistence.Context;

namespace TheatreMasterService.Infrastructure.Persistence.Repositories
{
    /// <inheritdoc />
    public class ScreenRepository(DapperContext context) : IScreenRepository
    {
        #region Configuration
        private readonly DapperContext _context = context;
        #endregion

        /// <inheritdoc />
        public async Task<Screen> CreateScreenAsync(Screen screen)
        {
            using var connection = _context.CreateConnection();
            var created = await connection.QuerySingleAsync<Screen>(
                 "SP_CreateScreen",
                 new
                 {
                     screen.Id,
                     screen.TheatreId,
                     screen.ScreenName,
                     screen.ScreenType,
                     screen.SeatCapacity,
                     screen.Rows,             
                     screen.SeatsPerRow,     
                     screen.Created,
                     screen.Modified,
                     screen.IsDeleted
                 },
                 commandType: CommandType.StoredProcedure);
            return created;
        }

        /// <inheritdoc />
        public async Task<Screen?> GetScreenByIdAsync(Guid id)
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<Screen>(
                "SP_GetScreenById",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Screen>> GetScreensAsync()
        {
            using var connection = _context.CreateConnection();

            return await connection.QueryAsync<Screen>(
                "SP_GetScreens",
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task<Screen?> UpdateScreenAsync(Screen screen)
        {
            using var connection = _context.CreateConnection();

            await connection.ExecuteAsync(
                            "SP_UpdateScreen",
                            new
                            {
                                screen.Id,
                                screen.TheatreId,
                                screen.ScreenName,
                                screen.ScreenType,
                                screen.SeatCapacity,
                                screen.Rows,            
                                screen.SeatsPerRow,     
                                screen.Modified,
                                screen.IsDeleted
                            },
                            commandType: CommandType.StoredProcedure
                        );

            return await connection.QueryFirstOrDefaultAsync<Screen>(
                "SP_GetScreenById",
                new { Id = screen.Id },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task<bool> DeleteScreenAsync(Guid id)
        {
            using var connection = _context.CreateConnection();

            var affectedRows = await connection.ExecuteScalarAsync<int>(
                "SP_DeleteScreen",
                 new { Id = id },
                 commandType: CommandType.StoredProcedure
             );

            return affectedRows > 0;
        }
    }
}