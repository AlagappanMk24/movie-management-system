using UserService.Domain.Entities;

namespace UserService.Application.Abstractions
{
    /// <summary>
    /// Defines user management service contracts.
    /// </summary>
    public interface IUserService
    {
        /// <summary>Retrieves all users.</summary>
        Task<IEnumerable<User>> GetUserAsync();

        /// <summary>Retrieves a user by ID.</summary>
        Task<User> GetUserByIdAsync(Guid id);

        /// <summary>Creates a new user.</summary>
        Task<User> CreateUserAsync(User user);

        /// <summary>Updates an existing user.</summary>
        Task<bool> UpdateUserAsync(Guid id, User user);

        /// <summary>Deletes a user by ID.</summary>
        Task<bool> DeleteUserAsync(Guid id);
    }
}