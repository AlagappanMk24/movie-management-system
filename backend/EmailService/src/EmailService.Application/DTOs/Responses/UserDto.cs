namespace EmailService.Application.DTOs.Responses
{
    /// <summary>
    /// Data transfer object representing user profile details.
    /// </summary>
    public class UserDto
    {
        /// <summary>Gets or sets the user's email address.</summary>
        public string Email { get; set; } = string.Empty;
    }
}