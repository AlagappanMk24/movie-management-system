using Microsoft.AspNetCore.Identity;
using UserService.Application.Abstractions;
using UserService.Application.Abstractions.Persistence;
using UserService.Domain.Entities;

namespace UserService.Application.Services
{
    /// <summary>
    /// Provides business logic operations for user management.
    /// </summary>
    public class UserService(IUserRepository userRepository, IMicroServiceGateway gateway) : IUserService
    {
        #region Configuration
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMicroServiceGateway _gateway = gateway;
        private readonly PasswordHasher<User> _passwordHasher = new();
        #endregion

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>A collection of user entities.</returns>
        #region GetAllUsers
        public async Task<IEnumerable<User>> GetUserAsync()
        {
            return await _userRepository.GetUserAsync();
        }
        #endregion

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>The user entity.</returns>
        #region GetUserById
        public async Task<User> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found");
            return user;
        }
        #endregion

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">The user entity to create.</param>
        /// <returns>The created user entity.</returns>
        #region CreateUser
        public async Task<User> CreateUserAsync(User user)
        {
            if (user.Id == Guid.Empty)
            {
                user.Id = Guid.NewGuid();
            }
            user.Password = _passwordHasher.HashPassword(user, user.Password);

            user.Role = string.IsNullOrEmpty(user.Role) ? "User" : user.Role;
            user.Created = DateTime.UtcNow;
            user.Modified = DateTime.UtcNow;
            user.IsDeleted = false;

            return await _userRepository.CreateUserAsync(user);
        }
        #endregion

        /// <summary>
        /// Updates an existing user's details.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <param name="user">The updated user data.</param>
        /// <returns>True if the update is successful.</returns>
        #region UpdateUser
        public async Task<bool> UpdateUserAsync(Guid id, User user)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null)
                throw new KeyNotFoundException("User not found");

            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;

            // Update password only if a new one is provided
            if (!string.IsNullOrEmpty(user.Password))
            {
                existingUser.Password = _passwordHasher.HashPassword(existingUser, user.Password);
            }

            existingUser.Modified = DateTime.UtcNow;
            return await _userRepository.UpdateUserAsync(existingUser);
        }
        #endregion

        /// <summary>
        /// Deletes a user by their identifier if they have no active bookings.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>True if deletion is successful.</returns>
        #region DeleteUser
        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(id);
            if (existingUser == null)
                throw new KeyNotFoundException("User not found");

            var hasBookings = await _gateway.HasBooking(id);
            if (hasBookings)
                throw new InvalidOperationException("Cannot delete user with existing bookings");

            return await _userRepository.DeleteUserAsync(id);
        }
        #endregion
    }
}