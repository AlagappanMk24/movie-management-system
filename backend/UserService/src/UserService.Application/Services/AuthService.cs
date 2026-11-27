using Microsoft.AspNetCore.Identity;
using UserService.Application.Abstractions;
using UserService.Application.Abstractions.Persistence;
using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Services
{
    /// <summary>
    /// Provides authentication services for user login and registration.
    /// </summary>
    public class AuthService(IUserRepository userRepository, IUserService userService) : IAuthService
    {
        #region Configuration
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUserService _userService = userService;
        private readonly PasswordHasher<User> _passwordHasher = new();
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

            if (user == null)
            {
                return null;
            }

            // Verify the entered password against the stored secure hash
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);

            if (verificationResult == PasswordVerificationResult.Success)
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
        public async Task<User?> RegisterUserAsync(RegisterUserDto request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return null; // User already exists
            }

            var newUser = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                Password = request.Password, // Will be hashed securely inside UserService
                PhoneNumber = request.PhoneNumber,
                Role = "User"
            };

            // Delegate creation and hashing logic to UserService
            var createdUser = await _userService.CreateUserAsync(newUser);
            return createdUser;
        }
        #endregion
    }
}