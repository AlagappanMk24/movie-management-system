using BookingService.Application.DTOs;

namespace BookingService.Application.Abstractions.Documents
{
    /// <summary>
    /// Handles assembling ticket data and rendering downloadable PDF documents.
    /// </summary>
    public interface ITicketPdfService
    {
        /// <summary>
        /// Aggregates booking, show, movie, theatre, and QR code data into a ticket model.
        /// </summary>
        Task<TicketPdfModel> BuildTicketPdfModelAsync(Guid bookingId);

        /// <summary>
        /// Renders a <see cref="TicketPdfModel"/> into a binary PDF file.
        /// </summary>
        byte[] GenerateTicketPdf(TicketPdfModel ticket);
    }
}