using Microsoft.EntityFrameworkCore;
using System.Data;
using TheatreMasterService.Application.Abstractions.Persistence;
using TheatreMasterService.Domain.Entities;
using TheatreMasterService.Infrastructure.Persistence.Context;
using Dapper;

namespace TheatreMasterService.Infrastructure.Persistence.Repositories
{
    /// <inheritdoc />
    public class TheatreRepository(DapperContext context) : ITheatreRepository
    {
        #region Configuration
        private readonly DapperContext _context = context;
        #endregion

        /// <inheritdoc />
        public async Task<Theatre> CreateTheatreAsync(Theatre theatre)
        {
            using var connection = _context.CreateConnection();

            var createdTheatre = await connection.QuerySingleAsync<Theatre>(
                "SP_CreateTheatre",
                new
                {
                    theatre.Id,
                    theatre.TheatreName,
                    theatre.City,
                    theatre.Address,
                    theatre.PhoneNumber,
                    theatre.Created,
                    theatre.Modified,
                    theatre.IsDeleted
                },
                commandType: CommandType.StoredProcedure
            );

            return createdTheatre;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Theatre>> GetTheatresAsync()
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Theatre>(
                "SP_GetTheatres",
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task<Theatre?> GetTheatreByIdAsync(Guid id)
        {
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Theatre>(
                "SP_GetTheatreById",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task<Theatre?> UpdateTheatreAsync(Theatre theatre)
        {
            using var connection = _context.CreateConnection();

            // 1. Execute update
            await connection.ExecuteAsync(
                "SP_UpdateTheatre",
                new
                {
                    theatre.Id,
                    theatre.TheatreName,
                    theatre.City,
                    theatre.Address,
                    theatre.PhoneNumber,
                    theatre.Modified,
                    theatre.IsDeleted
                },
                commandType: CommandType.StoredProcedure
            );

            // 2. Fetch and return the fresh record
            return await connection.QueryFirstOrDefaultAsync<Theatre>(
                "SP_GetTheatreById",
                new { Id = theatre.Id },
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task<bool> DeleteTheatreAsync(Guid id)
        {
            using var connection = _context.CreateConnection();

            var affectedRows = await connection.ExecuteScalarAsync<int>(
                "SP_DeleteTheatre",
                 new { Id = id },
                 commandType: CommandType.StoredProcedure
             );

            return affectedRows > 0;
        }
    }
}