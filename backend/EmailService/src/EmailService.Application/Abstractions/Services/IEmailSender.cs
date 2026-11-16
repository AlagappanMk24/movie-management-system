namespace EmailService.Application.Abstractions.Services
{
    /// <summary>
    /// Defines contract methods for sending emails with attachments via SMTP.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>Sends an email containing HTML body and optional PDF attachments.</summary>
        Task SendAsync(string to, string subject, string body, byte[] pdfBytes);
    }
}