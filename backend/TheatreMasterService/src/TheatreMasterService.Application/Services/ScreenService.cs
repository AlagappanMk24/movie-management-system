using TheatreMasterService.Application.Abstractions.Persistence;
using TheatreMasterService.Application.Abstractions.Services;
using TheatreMasterService.Domain.Entities;

namespace TheatreMasterService.Application.Services
{
    /// <inheritdoc />
    public class ScreenService(IScreenRepository screenRepository) : IScreenService
    {
        #region Configuration
        private readonly IScreenRepository _screenRepository = screenRepository;
        #endregion

        /// <inheritdoc />
        public async Task<IEnumerable<Screen>> GetScreensAsync()
        {
            return await _screenRepository.GetScreensAsync();
        }

        /// <inheritdoc />
        public async Task<Screen?> GetScreenByIdAsync(int id)
        {
            return await _screenRepository.GetScreenByIdAsync(id);
        }

        /// <inheritdoc />
        public async Task<Screen> CreateScreenAsync(Screen screen)
        {
            screen.Created = DateTime.Now;
            screen.Modified = DateTime.Now;
            return await _screenRepository.CreateScreenAsync(screen);
        }

        /// <inheritdoc />
        public async Task<bool> UpdateScreenAsync(int id, Screen screen)
        {
            var existingScreen = await _screenRepository.GetScreenByIdAsync(id);
            if (existingScreen == null)
                throw new KeyNotFoundException("Screen not found");

            existingScreen.SeatCapacity = screen.SeatCapacity;
            existingScreen.ScreenName = screen.ScreenName;
            existingScreen.TheatreId = screen.TheatreId;
            existingScreen.Modified = DateTime.Now;
            return await _screenRepository.UpdateScreenAsync(existingScreen);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteScreenAsync(int id)
        {
            return await _screenRepository.DeleteScreenAsync(id);
        }
    }
}