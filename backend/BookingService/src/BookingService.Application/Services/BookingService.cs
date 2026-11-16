using BookingService.Application.Abstractions.Caching;
using BookingService.Application.Abstractions.Messaging;
using BookingService.Application.Abstractions.Persistence;
using BookingService.Application.Abstractions.Services;
using BookingService.Application.DTOs;
using BookingService.Application.DTOs.Analytics;
using BookingService.Application.DTOs.Events;
using BookingService.Application.DTOs.Excel;
using BookingService.Application.DTOs.Requests;
using BookingService.Domain.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BookingService.Application.Services
{
    /// <inheritdoc />
    public class BookingService(
        IBookingRepository bookingRepository,
        IBookingSeatRepository seatRepository,
        IMicroServiceGateway gateway,
        IMessageBusClient messageBusClient,
        ICacheService cache,
        ILogger<BookingService> logger) : IBookingService
    {
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly IBookingSeatRepository _seatRepository = seatRepository;
        private readonly IMicroServiceGateway _gateway = gateway;
        private readonly IMessageBusClient _messageBusClient = messageBusClient;
        private readonly ICacheService _cache = cache;
        private readonly ILogger<BookingService> _logger = logger;

        /// <inheritdoc />
        public async Task<IEnumerable<Booking>> GetBookingAsync()
        {
            _logger.LogInformation("Fetching all bookings.");
            return await _cache.GetOrSetAsync(
                "all_bookings",
                () => _bookingRepository.GetBookingAsync(),
                TimeSpan.FromMinutes(10)) ?? [];
        }

        /// <inheritdoc />
        public async Task<Booking?> GetBookingByIdAsync(int bookingId)
        {
            return await _cache.GetOrSetAsync(
                $"booking_{bookingId}",
                () => _bookingRepository.GetBookingByIdAsync(bookingId),
                TimeSpan.FromMinutes(10));
        }

        /// <inheritdoc />
        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            var showDetails = await _gateway.GetShowDetailsAsync(booking.ShowId);
            var movie = await _gateway.GetMovieAsync(showDetails.MovieId);

            booking.PaymentStatus = "Completed";
            booking.TotalAmount = booking.SeatCount * showDetails.Price;
            booking.Created = DateTime.Now;
            booking.Modified = DateTime.Now;

            var createdBooking = await _bookingRepository.CreateBookingAsync(booking);

            var parsedShowTime = TimeSpan.TryParse(showDetails.ShowTime, out var ts) ? ts : TimeSpan.Zero;
            await PublishBookingEventAsync(createdBooking, movie.Title, parsedShowTime);

            return createdBooking;
        }

        /// <inheritdoc />
        public async Task<int> CreateBookingWithSeatsAsync(CreateBookingWithSeatsRequest request)
        {
            var showDetails = await _gateway.GetShowDetailsAsync(request.ShowId);

            var booking = new Booking
            {
                ShowId = request.ShowId,
                UserId = request.UserId,
                SeatCount = request.SeatNos.Count,
                TotalAmount = request.SeatNos.Count * showDetails.Price,
                PaymentStatus = "Completed",
                PaymentMethod = "UPI",
                Created = DateTime.Now,
                Modified = DateTime.Now
            };

            var createdBooking = await _bookingRepository.CreateBookingAsync(booking);
            await BulkBookSeatsAsync(createdBooking.BookingId, request.ShowId, request.SeatNos);

            var movie = await _gateway.GetMovieAsync(showDetails.MovieId);
            var parsedShowTime = TimeSpan.TryParse(showDetails.ShowTime, out var ts) ? ts : TimeSpan.Zero;
            await PublishBookingEventAsync(createdBooking, movie.Title, parsedShowTime);

            return createdBooking.BookingId;
        }

        /// <inheritdoc />
        public async Task BulkBookSeatsAsync(int bookingId, int showId, List<string> seatNos)
        {
            foreach (var seatNo in seatNos)
            {
                bool locked = await _cache.TryLockSeatAsync(showId, seatNo, TimeSpan.FromSeconds(100));
                if (!locked)
                {
                    throw new InvalidOperationException($"Seat {seatNo} is already booked or in progress.");
                }
            }

            var showDetails = await _gateway.GetShowDetailsAsync(showId);
            var seats = seatNos.Select(no => new BookingSeatTvpDto
            {
                SeatNo = no,
                Price = showDetails.Price
            }).ToList();

            await _seatRepository.BulkInsertSeatsAsync(bookingId, showId, seats);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Booking>> GetBookingByUserIdAsync(int userId) =>
            await _bookingRepository.GetBookingByUserIdAsync(userId);

        /// <inheritdoc />
        public async Task<IEnumerable<Booking>> GetBookingByShowIdAsync(int showId) =>
            await _bookingRepository.GetBookingByShowIdAsync(showId);

        /// <inheritdoc />
        public async Task<int> GetBookedSeatCountByShowAsync(int showId) =>
            await _bookingRepository.GetBookedSeatCountByShowAsync(showId);

        /// <inheritdoc />
        public async Task<List<string>> GetBookedSeatsAsync(int bookingId) =>
            await _seatRepository.GetBookedSeatsByBookingIdAsync(bookingId);

        /// <inheritdoc />
        public async Task<bool> CancelBookingAsync(int bookingId) =>
            await UpdateBookingStatusAsync(bookingId, "Cancelled");

        /// <inheritdoc />
        public async Task<bool> UpdateBookingStatusAsync(int bookingId, string status)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            if (booking == null) return false;

            booking.PaymentStatus = status;
            booking.Modified = DateTime.Now;

            await _bookingRepository.UpdateBookingAsync(booking);
            return true;
        }

        /// <inheritdoc />
        public async Task<List<MovieCollectionDto>> GetMovieWiseCollectionAsync()
        {
            const string cacheKey = "movie_collection";
            return await _cache.GetOrSetAsync(cacheKey, async () =>
            {
                var bookings = await _bookingRepository.GetCompletedBookingsAsync();
                var result = new Dictionary<string, int>();

                foreach (var booking in bookings)
                {
                    try
                    {
                        var showDetails = await _gateway.GetShowDetailsAsync(booking.ShowId);
                        var movie = await _gateway.GetMovieAsync(showDetails.MovieId);

                        if (movie == null)
                            continue;

                        if (result.ContainsKey(movie.Title))
                            result[movie.Title] += booking.TotalAmount;
                        else
                            result[movie.Title] = booking.TotalAmount;
                    }
                    catch
                    {
                        continue;
                    }
                }

                return result.Select(kvp => new MovieCollectionDto
                {
                    Title = kvp.Key,
                    MovieCollection = kvp.Value
                })
                .OrderByDescending(x => x.MovieCollection)
                .ToList();
            }, TimeSpan.FromMinutes(10)) ?? [];
        }

        private async Task PublishBookingEventAsync(Booking booking, string movieTitle, TimeSpan showTime)
        {
            var eventMessage = new BookingConfirmed
            {
                BookingId = booking.BookingId,
                ShowId = booking.ShowId,
                SeatCount = booking.SeatCount,
                Title = movieTitle,
                ShowTime = showTime,
                UserId = booking.UserId
            };

            _logger.LogInformation("Publishing booking confirmed event for BookingId: {BookingId}", eventMessage.BookingId);
            await _messageBusClient.PublishConfirmedBookingAsync(eventMessage);
        }
    }
}