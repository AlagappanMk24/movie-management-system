using EmailService.Application.Abstractions.Messaging;
using EmailService.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;

namespace EmailService.Application.Services
{
    /// <inheritdoc />
    public class EmailProcessingService(
        IMicroServiceGateway gateway,
        IEmailSender emailSender,
        ILogger<EmailProcessingService> logger) : IEmailProcessingService
    {
        private readonly IMicroServiceGateway _gateway = gateway;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly ILogger<EmailProcessingService> _logger = logger;

        /// <inheritdoc />
        public async Task ProcessBookingConfirmationAsync(BookingConfirmed bookingEvent)
        {
            _logger.LogInformation("Processing email confirmation for Booking ID: {BookingId}", bookingEvent.BookingId);

            // 1. Fetch user email
            var email = await _gateway.GetUserEmailAsync(bookingEvent.UserId);
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Email not found for User ID {UserId}", bookingEvent.UserId);
                return;
            }

            // 2. Download Ticket PDF from Booking Service
            var pdfBytes = await _gateway.DownloadTicketAsync(bookingEvent.BookingId);

            // 3. Compose Email Body
            var subject = $"Booking Confirmed: {bookingEvent.Title}";
            var body = $"<h1>Your booking for {bookingEvent.Title} is confirmed!</h1>" +
                       $"<p>Show Time: {bookingEvent.ShowTime}</p>" +
                       $"<p>Seats: {bookingEvent.SeatCount}</p>";

            // 4. Send Email
            await _emailSender.SendAsync(email, subject, body, pdfBytes);
            _logger.LogInformation("Successfully sent confirmation email to {Email}", email);
        }
    }
}
