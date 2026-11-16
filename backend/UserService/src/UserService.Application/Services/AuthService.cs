using UserService.Application.Abstractions;
using UserService.Application.Abstractions.Persistence;
using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Services
{
    /// <summary>
    /// Provides authentication services for user login and registration.
    /// </summary>
    public class AuthService(IUserRepository userRepository) : IAuthService
    {
        #region Configuration
        private readonly IUserRepository _userRepository = userRepository;
        #endregion

        /// <summary>
        /// Validates user credentials by email and password.
        /// </summary>
        /// <param name="email">The user email address.</param>
        /// <param name="password">The user password.</param>
        /// <returns>The user entity if validation succeeds; otherwise, null.</returns>
        #region ValidateUserAsync
        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            if (user != null && user.Password == password)
            {
                return user;
            }
            return null;
        }
        #endregion

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <param name="user">The registration details DTO.</param>
        /// <returns>The created user entity, or null if the user already exists.</returns>
        #region RegisterUserAsync
        public async Task<User?> RegisterUserAsync(RegisterUserDto user)
        {
            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null)
            {
                return null; // User already exists
            }
            var newUser = new User
            {
                UserName = user.UserName,
                Email = user.Email,
                Password = user.Password,
                PhoneNumber = user.PhoneNumber,
                Role = "User" // Default role
            };
            var createdUser = await _userRepository.CreateUserAsync(newUser);
            return createdUser;
        }
        #endregion
    }
}