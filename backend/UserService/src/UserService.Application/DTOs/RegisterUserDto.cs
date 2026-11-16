namespace UserService.Application.DTOs
{
    /// <summary>
    /// Data transfer object for user registration requests.
    /// </summary>
    public class RegisterUserDto
    {
        /// <summary>Gets or sets the username.</summary>
        public string UserName { get; set; } = null!;

        /// <summary>Gets or sets the email address.</summary>
        public string Email { get; set; } = null!;

        /// <summary>Gets or sets the password.</summary>
        public string Password { get; set; } = null!;

        /// <summary>Gets or sets the phone number.</summary>
        public string PhoneNumber { get; set; } = null!;
    }
}