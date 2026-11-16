using BookingService.Application.DTOs.Events;

namespace BookingService.Application.Abstractions.Messaging
{
    /// <summary>
    /// Defines contract methods for publishing asynchronous integration events to the message broker.
    /// </summary>
    public interface IMessageBusClient
    {
        /// <summary>
        /// Publishes a confirmed booking event to the message bus exchange.
        /// </summary>
        /// <param name="bookingConfirmed">The booking confirmation event data transfer object.</param>
        /// <returns>A task representing the asynchronous publish operation.</returns>
        Task PublishConfirmedBookingAsync(BookingConfirmed bookingConfirmed);
    }
}