using TheatreMasterService.Application.Abstractions.Persistence;
using TheatreMasterService.Application.Abstractions.Services;
using TheatreMasterService.Application.DTOs.Shows.Requests;
using TheatreMasterService.Application.DTOs.Shows.Responses;
using TheatreMasterService.Domain.Entities;

namespace TheatreMasterService.Application.Services
{
    /// <inheritdoc />
    public class ShowService(IShowRepository showRepository, IMicroServiceGateway gateway, IScreenRepository screenRepository) : IShowService
    {
        #region Configuration
        private readonly IShowRepository _showRepository = showRepository;
        private readonly IScreenRepository _screenRepository = screenRepository;
        private readonly IMicroServiceGateway _gateway = gateway;
        #endregion

        /// <inheritdoc />
        public async Task<IEnumerable<ShowResponse>> GetShowsAsync()
        {
            var shows = await _showRepository.GetShowsAsync();
            return shows.Select(MapToResponse);
        }

        /// <inheritdoc />
        public async Task<ShowResponse?> GetShowByIdAsync(Guid id)
        {
            var show = await _showRepository.GetShowByIdAsync(id);
            return show is null ? null : MapToResponse(show);
        }

        /// <inheritdoc />
        public async Task<ShowResponse> CreateShowAsync(CreateShowRequest request)
        {
            var screen = await _screenRepository.GetScreenByIdAsync(request.ScreenId);
            if (screen is null)
                throw new KeyNotFoundException($"Screen with ID {request.ScreenId} was not found.");

            var now = DateTime.UtcNow;
            var show = new Show
            {
                Id = Guid.NewGuid(),
                MovieId = request.MovieId,
                ScreenId = request.ScreenId,
                ShowDate = request.ShowDate,
                ShowTime = request.ShowTime,
                Price = request.Price,
                Created = now,
                Modified = now,
                IsDeleted = false
            };

            var createdShow = await _showRepository.CreateShowAsync(show);
            return MapToResponse(createdShow);
        }

        /// <inheritdoc />
        public async Task<bool> UpdateShowAsync(Guid id, UpdateShowRequest request)
        {
            var existingShow = await _showRepository.GetShowByIdAsync(id);
            if (existingShow is null)
                return false;

            existingShow.MovieId = request.MovieId;
            existingShow.ScreenId = request.ScreenId;
            existingShow.ShowDate = request.ShowDate;
            existingShow.ShowTime = request.ShowTime;
            existingShow.Price = request.Price;
            existingShow.Modified = DateTime.UtcNow;

            return await _showRepository.UpdateShowAsync(existingShow);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteShowAsync(Guid id)
        {
            var hasBooking = await _gateway.HasBookingByShow(id);
            if (hasBooking)
                throw new InvalidOperationException("Cannot delete show with active bookings.");

            return await _showRepository.DeleteShowAsync(id);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ShowResponse>> GetShowsByMovieIdAsync(Guid movieId)
        {
            var shows = await _showRepository.GetShowsByMovieId(movieId);
            return shows.Select(MapToResponse);
        }

        /// <inheritdoc />
        public async Task<AvailableSeatsResponse> GetAvailableSeatsAsync(Guid showId)
        {
            var show = await _showRepository.GetShowByIdAsync(showId);
            if (show is null)
                throw new KeyNotFoundException($"Show with ID {showId} was not found.");

            var screen = await _screenRepository.GetScreenByIdAsync(show.ScreenId);
            if (screen is null)
                throw new KeyNotFoundException($"Screen with ID {show.ScreenId} was not found.");

            var bookedSeats = await _gateway.GetBookedSeatsByShowAsync(showId);
            var availableSeats = Math.Max(0, screen.SeatCapacity - bookedSeats);

            return new AvailableSeatsResponse(
                ShowId: showId,
                ScreenId: screen.Id,
                TotalSeats: screen.SeatCapacity,
                BookedSeats: bookedSeats,
                AvailableSeats: availableSeats);
        }

        /// <inheritdoc />
        public async Task ReduceSeatsAsync(Guid showId, int seatCount)
        {
            var show = await _showRepository.GetShowByIdAsync(showId);
            if (show is null)
                throw new KeyNotFoundException($"Show with ID {showId} was not found.");

            Console.WriteLine($"[RabbitMQ] Booking confirmed. ShowId={showId}, Seats={seatCount}");
        }

        public async Task<ScreenLayoutDto> GetScreenLayoutAsync(Guid showId)
        {
            var show = await _showRepository.GetShowByIdAsync(showId) ??
                throw new KeyNotFoundException($"Show with ID {showId} was not found.");

            var screen = await _screenRepository.GetScreenByIdAsync(show.ScreenId) ??
                throw new KeyNotFoundException($"Screen with ID {show.ScreenId} was not found.");

            // Generate the grid: A1..A{SeatsPerRow}, B1..B{SeatsPerRow}, ...
            var seatNos = new List<string>(screen.Rows * screen.SeatsPerRow);
            for (var r = 0; r < screen.Rows; r++)
            {
                var rowLabel = ((char)('A' + r)).ToString();
                for (var c = 1; c <= screen.SeatsPerRow; c++)
                    seatNos.Add($"{rowLabel}{c}");
            }

            return new ScreenLayoutDto
            {
                ShowId = showId,
                ScreenId = screen.Id,
                Rows = screen.Rows,
                SeatsPerRow = screen.SeatsPerRow,
                TotalSeats = screen.SeatCapacity,
                SeatNumbers = seatNos
            };
        }
        private static ShowResponse MapToResponse(Show show)
        {
            return new 
            (
                ShowId: show.Id,
                MovieId: show.MovieId,
                ScreenId: show.ScreenId,
                ShowDate: show.ShowDate,
                ShowTime: show.ShowTime,
                Price: show.Price,
                Created: show.Created,
                Modified: show.Modified
            );
        }
    }
}