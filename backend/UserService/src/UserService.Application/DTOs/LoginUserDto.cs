namespace UserService.Application.DTOs
{
    /// <summary>
    /// Data transfer object for user login requests.
    /// </summary>
    public class LoginUserDto
    {
        /// <summary>Gets or sets the user email.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the user password.</summary>
        public string Password { get; set; } = string.Empty;
    }
}