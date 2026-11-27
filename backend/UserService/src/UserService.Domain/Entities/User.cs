using Shared.Contracts.Common;
using System.ComponentModel.DataAnnotations;

namespace UserService.Domain.Entities
{
    /// <summary>
    /// Represents a user entity.
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>Gets or sets the username.</summary>
        public string UserName { get; set; }

        /// <summary>Gets or sets the email address.</summary>
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        /// <summary>Gets or sets the password.</summary>
        [Required]
        public string Password { get; set; }

        /// <summary>Gets or sets the phone number.</summary>
        [Required]
        public string PhoneNumber { get; set; }

        /// <summary>Gets or sets the user role.</summary>
        public string? Role { get; set; }
    }
}