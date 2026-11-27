using TheatreMasterService.Application.Abstractions.Persistence;
using TheatreMasterService.Application.Abstractions.Services;
using TheatreMasterService.Application.DTOs.Request;
using TheatreMasterService.Domain.Entities;

namespace TheatreMasterService.Application.Services
{
    /// <inheritdoc />
    public class ScreenService(IScreenRepository screenRepository, ITheatreRepository theatreRepository) : IScreenService
    {
        #region Configuration
        private readonly IScreenRepository _screenRepository = screenRepository;
        private readonly ITheatreRepository _theatreRepository = theatreRepository;
        #endregion

        /// <inheritdoc />
        public async Task<IEnumerable<Screen>> GetScreensAsync()
        {
            return await _screenRepository.GetScreensAsync();
        }

        /// <inheritdoc />
        public async Task<Screen?> GetScreenByIdAsync(Guid id)
        {
            return await _screenRepository.GetScreenByIdAsync(id);
        }

        /// <inheritdoc />
        public async Task<Screen> CreateScreenAsync(ScreenCreateUpdateDto dto)
        {
            // Fail fast before hitting SQL
            if (dto.Rows * dto.SeatsPerRow != dto.SeatCapacity)
                throw new InvalidOperationException(
                    $"SeatCapacity ({dto.SeatCapacity}) must equal Rows × SeatsPerRow " +
                    $"({dto.Rows} × {dto.SeatsPerRow} = {dto.Rows * dto.SeatsPerRow}).");

            var theatre = await _theatreRepository.GetTheatreByIdAsync(dto.TheatreId);
            if (theatre is null)
                throw new InvalidOperationException(
                    $"Cannot create screen because theatre {dto.TheatreId} does not exist.");

            var now = DateTime.UtcNow;
            var screen = new Screen
            {
                Id = Guid.NewGuid(),
                TheatreId = dto.TheatreId,
                ScreenName = dto.ScreenName,
                ScreenType = dto.ScreenType,
                SeatCapacity = dto.SeatCapacity,
                Rows = dto.Rows,
                SeatsPerRow = dto.SeatsPerRow,
                Created = now,
                Modified = now,
                IsDeleted = false
            };

            return await _screenRepository.CreateScreenAsync(screen);
        }

        /// <inheritdoc />
        public async Task<Screen?> UpdateScreenAsync(Guid id, ScreenCreateUpdateDto dto)
        {
            // GetScreenByIdAsync already excludes screens whose theatre is soft-deleted
            var existingScreen = await _screenRepository.GetScreenByIdAsync(id);
            if (existingScreen == null)
                return null;

            var theatre = await _theatreRepository.GetTheatreByIdAsync(dto.TheatreId);
            if (theatre == null)
            {
                throw new InvalidOperationException($"Theatre with ID {dto.TheatreId} does not exist or has been deleted.");
            }

            existingScreen.TheatreId = dto.TheatreId;
            existingScreen.ScreenName = dto.ScreenName.Trim();
            existingScreen.ScreenType = dto.ScreenType.Trim();
            existingScreen.SeatCapacity = dto.SeatCapacity;
            existingScreen.Modified = DateTime.UtcNow;

            return await _screenRepository.UpdateScreenAsync(existingScreen);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteScreenAsync(Guid id)
        {
            return await _screenRepository.DeleteScreenAsync(id);
        }
    }
}