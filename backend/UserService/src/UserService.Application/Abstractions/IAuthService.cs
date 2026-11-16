using UserService.Application.DTOs;
using UserService.Domain.Entities;

namespace UserService.Application.Abstractions
{
    /// <summary>
    /// Defines authentication and registration service contracts.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>Validates user login credentials.</summary>
        Task<User?> ValidateUserAsync(string email, string password);

        /// <summary>Registers a new user.</summary>
        Task<User?> RegisterUserAsync(RegisterUserDto user);
    }
}