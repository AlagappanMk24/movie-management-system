using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using UserService.Application.Abstractions.Persistence;
using UserService.Domain.Entities;
using UserService.Infrastructure.Persistence.Context;
using Dapper;

namespace UserService.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implements repository operations for user data access using Dapper and stored procedures.
    /// </summary>
    public class UserRepository(DapperContext context, IMemoryCache cache) : IUserRepository
    {
        #region Configuration
        private readonly DapperContext _context = context;
        private readonly IMemoryCache _cache = cache;
        #endregion

        /// <summary>
        /// Retrieves all users from the database.
        /// </summary>
        #region GetUsersAsync
        public async Task<IEnumerable<User>> GetUserAsync()
        {
            using var connection = _context.CreateConnection();

            var users = await connection.QueryAsync<User>(
                "SP_GetUsers",
                commandType: CommandType.StoredProcedure
            );

            return users;
        }
        #endregion

        /// <summary>
        /// Retrieves a user by their unique identifier, using caching.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        #region GetUserByIdAsync
        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            string cacheKey = $"user_{userId}";

            if (_cache.TryGetValue(cacheKey, out User cachedUser))
            {
                return cachedUser;
            }

            using var connection = _context.CreateConnection();

            var user = await connection.QueryFirstOrDefaultAsync<User>(
                "SP_GetUserById",
                new { Id = userId },
                commandType: CommandType.StoredProcedure
            );

            if (user != null)
            {
                _cache.Set(cacheKey, user, TimeSpan.FromMinutes(5));
            }

            return user;
        }
        #endregion

        /// <summary>
        /// Creates a new user in the database.
        /// </summary>
        /// <param name="user">The user entity.</param>
        #region CreateUserAsync
        public async Task<User> CreateUserAsync(User user)
        {
            using var connection = _context.CreateConnection();

            // Use ExecuteAsync since SP_CreateUser performs an INSERT and returns no result set
            await connection.ExecuteAsync(
                "SP_CreateUser",
                new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.Password,
                    user.PhoneNumber,
                    user.IsDeleted
                },
                commandType: CommandType.StoredProcedure
            );

            return user;
        }
        #endregion

        /// <summary>
        /// Updates an existing user in the database.
        /// </summary>
        /// <param name="user">The user entity with updated values.</param>
        #region UpdateUserAsync
        public async Task<bool> UpdateUserAsync(User user)
        {
            using var connection = _context.CreateConnection();

            var rowsAffected = await connection.ExecuteScalarAsync<int>(
                "SP_UpdateUser",
                new
                {
                    Id = user.Id, 
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role
                },
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }
        #endregion

        /// <summary>
        /// Deletes a user by their identifier.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        #region DeleteUserAsync
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            using var connection = _context.CreateConnection();

            var affectedRows = await connection.ExecuteAsync
                ("SP_DeleteUser",
                new { Id = id },
                commandType: CommandType.StoredProcedure);

            return affectedRows > 0;
        }
        #endregion

        /// <summary>
        /// Retrieves a user by email address, using caching.
        /// </summary>
        /// <param name="email">The user email address.</param>
        #region GetByEmailAsync
        public async Task<User?> GetByEmailAsync(string email)
        {
            string cacheKey = $"user_email_{email}";

            if (_cache.TryGetValue(cacheKey, out User cachedUser))
            {
                return cachedUser;
            }

            using var connection = _context.CreateConnection();

            var user = await connection.QueryFirstOrDefaultAsync<User>(
                "SP_GetUserByEmail",
                new { Email = email },
                commandType: CommandType.StoredProcedure
            );

            if (user != null)
            {
                _cache.Set(cacheKey, user, TimeSpan.FromMinutes(5));
            }

            return user;
        }
        #endregion
    }
}