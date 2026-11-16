using BookingService.Application.Abstractions.Documents;
using BookingService.Application.Abstractions.Persistence;
using BookingService.Application.Abstractions.Services;
using BookingService.Application.DTOs;
using BookingService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using QuestPDF.Fluent;

namespace BookingService.Infrastructure.Documents
{
    /// <inheritdoc />
    public class TicketPdfService(
        IBookingRepository bookingRepository,
        IBookingSeatRepository seatRepository,
        IMicroServiceGateway gateway,
        IConfiguration configuration) : ITicketPdfService
    {
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly IBookingSeatRepository _seatRepository = seatRepository;
        private readonly IMicroServiceGateway _gateway = gateway;
        private readonly IConfiguration _configuration = configuration;

        /// <inheritdoc />
        public async Task<TicketPdfModel> BuildTicketPdfModelAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId)
                ?? throw new KeyNotFoundException($"Booking {bookingId} not found.");

            var showDetails = await _gateway.GetShowDetailsAsync(booking.ShowId);
            var movie = await _gateway.GetMovieAsync(showDetails.MovieId);
            var user = await _gateway.GetUserAsync(booking.UserId);
            var screen = await _gateway.GetScreenAsync(showDetails.ScreenId);
            var theatre = await _gateway.GetTheatreAsync(screen.TheatreId);

            var movieBaseUrl = _configuration["MicroServiceUrls:Movieservice"];
            byte[] posterBytes = await _gateway.DownloadImageAsync($"{movieBaseUrl}{movie.PosterUrl}");
            var seatList = await _seatRepository.GetBookedSeatsByBookingIdAsync(bookingId);

            var qrText = $"BOOKING:{booking.BookingId}\nMOVIE:{movie.Title}\nDATE:{showDetails.ShowDate:dd-MM-yyyy}\nTIME:{showDetails.ShowTime}\nSCREEN:{screen.ScreenName}";
            
            var qrBytes = QrCodeHelper.GenerateQrCode(qrText);

            return new TicketPdfModel
            {
                BookingId = booking.BookingId,
                MovieName = movie.Title,
                TheatreName = theatre.TheatreName,
                Screen = screen.ScreenName,
                Address = theatre.Address,
                ShowTime = showDetails.ShowTime,
                ShowDate = showDetails.ShowDate,
                Amount = booking.TotalAmount,
                UserName = user.UserName,
                MoviePoster = posterBytes,
                QrCodeImage = qrBytes,
                SeatCount = booking.SeatCount,
                SeatNos = seatList
            };
        }

        /// <inheritdoc />
        public byte[] GenerateTicketPdf(TicketPdfModel ticket)
        {
            var document = new TicketPdfDocument(ticket);
            return document.GeneratePdf();
        }
    }
}