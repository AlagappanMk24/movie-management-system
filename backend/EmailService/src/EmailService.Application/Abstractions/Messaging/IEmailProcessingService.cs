using EmailService.Application.DTOs.Events;

namespace EmailService.Application.Abstractions.Messaging
{
    /// <summary>Defines workflow orchestration for processing booking confirmation emails.</summary>
    public interface IEmailProcessingService
    {
        /// <summary>Processes a confirmed booking event by downloading its ticket and emailing the user.</summary>
        Task ProcessBookingConfirmationAsync(BookingConfirmed bookingEvent);
    }
}