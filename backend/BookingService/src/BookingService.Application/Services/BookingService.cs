using BookingService.Application.Abstractions.Caching;
using BookingService.Application.Abstractions.Messaging;
using BookingService.Application.Abstractions.Persistence;
using BookingService.Application.Abstractions.Services;
using BookingService.Application.DTOs.Analytics;
using BookingService.Application.DTOs.Excel;
using BookingService.Application.DTOs.Requests;
using BookingService.Application.DTOs.Responses;
using BookingService.Domain.Entities;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;
using System.Data;

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
        public async Task<IEnumerable<BookingResponse>> GetBookingsAsync()
        {
            _logger.LogInformation("Fetching all bookings.");
            var bookings = await _cache.GetOrSetAsync(
                "all_bookings",
                () => _bookingRepository.GetBookingAsync(),
                TimeSpan.FromMinutes(10)) ?? [];

            return bookings.ToResponses();
        }

        /// <inheritdoc />
        public async Task<BookingResponse?> GetBookingByIdAsync(Guid bookingId)
        {
            var booking = await _cache.GetOrSetAsync(
                $"booking_{bookingId}",
                () => _bookingRepository.GetBookingByIdAsync(bookingId),
                TimeSpan.FromMinutes(10));

            return booking?.ToResponse();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<BookingResponse>> GetBookingsByUserIdAsync(Guid userId) =>
            (await _bookingRepository.GetBookingByUserIdAsync(userId)).ToResponses();

        /// <inheritdoc />
        public async Task<IEnumerable<BookingResponse>> GetBookingsByShowIdAsync(Guid showId) =>
         (await _bookingRepository.GetBookingByShowIdAsync(showId)).ToResponses();

        /// <inheritdoc />
        public async Task<IEnumerable<BookingResponse>> GetBookingsByMovieIdAsync(Guid movieId)
        {
            var shows = await _gateway.GetShowsByMovieIdAsync(movieId);
            if (shows.Count == 0)
                return [];

            var bookings = new List<Booking>();
            foreach (var show in shows)
            {
                var showBookings = await _bookingRepository.GetBookingByShowIdAsync(show.ShowId);
                bookings.AddRange(showBookings);
            }

            return bookings.ToResponses();
        }

        /// <inheritdoc />
        public async Task<int> GetBookedSeatCountByShowAsync(Guid showId) =>
            await _bookingRepository.GetBookedSeatCountByShowAsync(showId);

        /// <inheritdoc />
        public async Task<List<string>> GetBookedSeatsAsync(Guid bookingId) =>
            await _seatRepository.GetBookedSeatsByBookingIdAsync(bookingId);

        /// <inheritdoc />
        public async Task<BookingResponse> CreateBookingAsync(CreateBookingRequest request)
        {
            // 1. Validate
            if (!await _gateway.UserExists(request.UserId))
                throw new InvalidOperationException($"User {request.UserId} does not exist.");
            if (!await _gateway.GetShow(request.ShowId))
                throw new InvalidOperationException($"Show {request.ShowId} does not exist.");

            // 2. Fetch metadata
            var showDetails = await _gateway.GetShowDetailsAsync(request.ShowId);
            var movie = await _gateway.GetMovieAsync(showDetails.MovieId);
            var layout = await _gateway.GetScreenLayoutAsync(request.ShowId);

            // 3. Figure out which seats to allocate
            var occupied = await _seatRepository.GetOccupiedSeatNosAsync(request.ShowId);
            var seats = AllocateSeats(layout, occupied, request.SeatCount);
            if (seats.Count < request.SeatCount)
                throw new InvalidOperationException(
                    $"Only {seats.Count} seat(s) available, requested {request.SeatCount}.");

            // 4. Redis-lock each picked seat (prevents race window before DB commit)
            var locked = new List<string>();
            try
            {
                foreach (var seatNo in seats)
                {
                    if (!await _cache.TryLockSeatAsync(request.ShowId, seatNo, TimeSpan.FromSeconds(100)))
                        throw new InvalidOperationException($"Seat {seatNo} was just taken. Please retry.");
                    locked.Add(seatNo);
                }

                // 5. Persist Bookings + BookingSeats atomically
                var now = DateTime.UtcNow;
                var booking = new Booking
                {
                    UserId = request.UserId,
                    ShowId = request.ShowId,
                    SeatCount = seats.Count,
                    PaymentMethod = request.PaymentMethod,
                    PaymentStatus = "Completed",
                    TotalAmount = seats.Count * showDetails.Price,
                    Created = now,
                    Modified = now
                };

                var created = await _bookingRepository.CreateBookingWithSeatsAsync(
                    booking,
                    seats.Select(s => new BookingSeat
                    {
                        ShowId = request.ShowId,
                        SeatNo = s,
                        Price = showDetails.Price,
                        Created = now,
                        Modified = now
                    }).ToList());

                // 6. Publish event (booking is already committed → MQ failure must not 500)
                try
                {
                    var showTime = TimeSpan.TryParse(showDetails.ShowTime, out var ts) ? ts : TimeSpan.Zero;
                    await PublishBookingEventAsync(created, movie.Title, showTime);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Booking {BookingId} created but MQ publish failed.", created.Id);
                }

                // 7. Return booking WITH its seats
                var bookedSeats = await _seatRepository.GetSeatsByBookingIdAsync(created.Id);
                return created.ToResponse(bookedSeats);
            }
            catch
            {
                // Release any locks we took — otherwise the seats are stuck for 100s
                foreach (var seat in locked)
                    await _cache.ReleaseSeatLockAsync(request.ShowId, seat);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Guid> CreateBookingWithSeatsAsync(CreateBookingWithSeatsRequest request)
        {
            var showDetails = await _gateway.GetShowDetailsAsync(request.ShowId);

            // 1. Lock ALL seats first
            var locked = new List<string>();
            try
            {
                foreach (var seatNo in request.SeatNos)
                {
                    if (!await _cache.TryLockSeatAsync(request.ShowId, seatNo, TimeSpan.FromSeconds(100)))
                        throw new InvalidOperationException($"Seat {seatNo} is already booked or in progress.");
                    locked.Add(seatNo);
                }

                // 2. Insert Booking + BookingSeats in ONE transaction
                var now = DateTime.UtcNow;
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

                var createdBooking = await _bookingRepository.CreateBookingWithSeatsAsync(
                     booking,
                     request.SeatNos.Select(no => new BookingSeat
                     {
                         ShowId = request.ShowId,
                         SeatNo = no,
                         Price = showDetails.Price,
                         Created = now,
                         Modified = now
                     }).ToList());
                await BulkBookSeatsAsync(createdBooking.Id, request.ShowId, request.SeatNos);

                // 3. Publish event
                var movie = await _gateway.GetMovieAsync(showDetails.MovieId);
                var parsedShowTime = TimeSpan.TryParse(showDetails.ShowTime, out var ts) ? ts : TimeSpan.Zero;
                await PublishBookingEventAsync(createdBooking, movie.Title, parsedShowTime);

                return createdBooking.Id;
            }
            catch
            {
                foreach (var seat in locked)
                    await _cache.ReleaseSeatLockAsync(request.ShowId, seat);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task BulkBookSeatsAsync(Guid bookingId, Guid showId, List<string> seatNos)
        {
            // 1. Get existing booking (validate it exists)
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId)
                ?? throw new KeyNotFoundException($"Booking {bookingId} not found.");

            // 2. Lock all seats (with cleanup on partial failure)
            var locked = new List<string>();
            try
            {
                foreach (var seatNo in seatNos)
                {
                    if (!await _cache.TryLockSeatAsync(showId, seatNo, TimeSpan.FromSeconds(100)))
                        throw new InvalidOperationException($"Seat {seatNo} is already booked.");
                    locked.Add(seatNo);
                }

                // 3. Insert seats (TVP)
                var showDetails = await _gateway.GetShowDetailsAsync(showId);
                var seats = seatNos.Select(no => new BookingSeatTvpDto
                {
                    SeatNo = no,
                    Price = showDetails.Price
                }).ToList();

                await _seatRepository.BulkInsertSeatsAsync(bookingId, showId, seats);

                // 4. ✅ Update SeatCount to keep consistent
                booking.SeatCount += seatNos.Count;
                booking.TotalAmount += seatNos.Count * showDetails.Price;
                booking.Modified = DateTime.UtcNow;
                await _bookingRepository.UpdateBookingAsync(booking);
            }
            catch
            {
                foreach (var seat in locked)
                    await _cache.ReleaseSeatLockAsync(showId, seat);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> CancelBookingAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(bookingId);
            if (booking == null) return false;

            // 1. Soft-delete seats so they become re-bookable
            await _seatRepository.SoftDeleteSeatsByBookingIdAsync(bookingId);

            // 2. Update booking status
            return await UpdateBookingStatusAsync(bookingId, "Cancelled");
        }

        /// <inheritdoc />
        public async Task<bool> UpdateBookingStatusAsync(Guid bookingId, string status)
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
        private static List<string> AllocateSeats(ScreenLayoutDto layout, HashSet<string> occupied, int count)
        {
            var result = new List<string>(count);
            for (var r = 0; r < layout.Rows && result.Count < count; r++)
            {
                var rowLabel = ((char)('A' + r)).ToString();
                for (var c = 1; c <= layout.SeatsPerRow && result.Count < count; c++)
                {
                    var seatNo = $"{rowLabel}{c}";
                    if (!occupied.Contains(seatNo))
                        result.Add(seatNo);
                }
            }
            return result;
        }
        private async Task PublishBookingEventAsync(Booking booking, string movieTitle, TimeSpan showTime)
        {
            var eventMessage = new BookingConfirmed
            {
                BookingId = booking.Id,
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