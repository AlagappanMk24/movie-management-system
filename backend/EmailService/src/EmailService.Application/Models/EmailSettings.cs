namespace EmailService.Application.Models
{
    /// <summary>
    /// Represents SMTP server configuration settings for email dispatch.
    /// </summary>
    public class EmailSettings
    {
        /// <summary>Gets or sets the sender's email address.</summary>
        public string From { get; set; } = string.Empty;

        /// <summary>Gets or sets the SMTP server hostname or IP address.</summary>
        public string SmtpServer { get; set; } = string.Empty;

        /// <summary>Gets or sets the SMTP server port number.</summary>
        public int Port { get; set; }

        /// <summary>Gets or sets the SMTP authentication username.</summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>Gets or sets the SMTP authentication password.</summary>
        public string Password { get; set; } = string.Empty;
    }
}