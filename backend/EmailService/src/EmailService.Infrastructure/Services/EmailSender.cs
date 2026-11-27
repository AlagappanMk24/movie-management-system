using EmailService.Application.Abstractions.Services;
using EmailService.Application.Models;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailService.Infrastructure.Services
{
    /// <inheritdoc />
    public class EmailSender(IOptions<EmailSettings> settings) : IEmailSender
    {
        private readonly EmailSettings _settings = settings.Value;

        /// <inheritdoc />
        public async Task SendAsync(string to, string subject, string body, byte[] pdfBytes)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_settings.From));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            builder.Attachments.Add("Ticket.pdf", pdfBytes, MimeKit.ContentType.Parse("application/pdf"));
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
            await smtp.SendAsync(email, CancellationToken.None);
            await smtp.DisconnectAsync(true);
        }
    }
}