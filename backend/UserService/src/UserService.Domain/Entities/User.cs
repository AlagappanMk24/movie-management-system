using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserService.Domain.Entities
{
    /// <summary>
    /// Represents a user entity.
    /// </summary>
    public class User
    {
        /// <summary>Gets or sets the unique user identifier.</summary>
        public int UserId { get; set; }

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

        /// <summary>Gets or sets the creation timestamp.</summary>
        public DateTime Created { get; set; }

        /// <summary>Gets or sets the last modification timestamp.</summary>
        public DateTime Modified { get; set; }
    }
}