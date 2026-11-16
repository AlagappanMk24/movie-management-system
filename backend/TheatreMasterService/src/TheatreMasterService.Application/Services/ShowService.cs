using TheatreMasterService.Application.Abstractions.Persistence;
using TheatreMasterService.Application.Abstractions.Services;
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
        public async Task<IEnumerable<Show>> GetShowsAsync()
        {
            return await _showRepository.GetShowsAsync();
        }

        /// <inheritdoc />
        public async Task<Show?> GetShowByIdAsync(int id)
        {
            var show = await _showRepository.GetShowByIdAsync(id);
            if (show == null)
                throw new KeyNotFoundException("Show not found");

            return show;
        }

        /// <inheritdoc />
        public async Task<Show> CreateShowAsync(Show show)
        {
            show.Created = DateTime.Now;
            show.Modified = DateTime.Now;
            return await _showRepository.CreateShowAsync(show);
        }

        /// <inheritdoc />
        public async Task<bool> UpdateShowAsync(int id, Show show)
        {
            var existingShow = await _showRepository.GetShowByIdAsync(id);
            if (existingShow == null)
                return false;
            existingShow.ShowDate = show.ShowDate;
            existingShow.ScreenId = show.ScreenId;
            existingShow.Price = show.Price;
            existingShow.ShowTime = show.ShowTime;
            existingShow.Modified = DateTime.Now;
            return await _showRepository.UpdateShowAsync(existingShow);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteShowAsync(int id)
        {
            var hasBooking = await _gateway.HasBookingByShow(id);
            if (hasBooking)
                throw new InvalidOperationException("Cannot delete show with active bookings.");
            return await _showRepository.DeleteShowAsync(id);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Show>> GetShowsByMovieId(int movieId)
        {
            var allShows = await _showRepository.GetShowsByMovieId(movieId);
            return allShows;
        }

        /// <inheritdoc />
        public async Task<object> GetAvailableSeatsAsync(int showId)
        {
            var show = await _showRepository.GetShowByIdAsync(showId);
            if (show == null)
                throw new KeyNotFoundException("Show not found");

            var screen = await _screenRepository.GetScreenByIdAsync(show.ScreenId);
            if (screen == null)
                throw new KeyNotFoundException("Screen not found");

            var bookedSeats = await _gateway.GetBookedSeatsByShowAsync(showId);

            var availableSeats = screen.SeatCapacity - bookedSeats;
            if (availableSeats < 0) availableSeats = 0;

            return new
            {
                ShowId = showId,
                ScreenId = screen.ScreenId,
                TotalSeats = screen.SeatCapacity,
                BookedSeats = bookedSeats,
                AvailableSeats = availableSeats
            };
        }

        /// <inheritdoc />
        public async Task ReduceSeatsAsync(int showId, int seatCount)
        {
            var show = await _showRepository.GetShowByIdAsync(showId);
            if (show == null)
                throw new KeyNotFoundException("Show not found");

            Console.WriteLine(
                $"[RabbitMQ] Booking confirmed. ShowId={showId}, Seats={seatCount}");
        }
    }
}