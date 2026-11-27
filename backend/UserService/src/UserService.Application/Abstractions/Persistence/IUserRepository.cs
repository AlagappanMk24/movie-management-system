using UserService.Domain.Entities;

namespace UserService.Application.Abstractions.Persistence
{
    /// <summary>
    /// Defines repository operations for data access related to users.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>Retrieves all users.</summary>
        Task<IEnumerable<User>> GetUserAsync();

        /// <summary>Retrieves a user by ID.</summary>
        Task<User> GetUserByIdAsync(Guid id);

        /// <summary>Creates a new user in the database.</summary>
        Task<User> CreateUserAsync(User user);

        /// <summary>Updates an existing user.</summary>
        Task<bool> UpdateUserAsync(User user);

        /// <summary>Deletes a user by ID.</summary>
        Task<bool> DeleteUserAsync(Guid id);

        /// <summary>Retrieves a user by email address.</summary>
        Task<User?> GetByEmailAsync(string email);
    }
}