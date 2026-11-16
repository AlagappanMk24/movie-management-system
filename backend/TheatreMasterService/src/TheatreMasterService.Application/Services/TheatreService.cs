using TheatreMasterService.Application.Abstractions.Persistence;
using TheatreMasterService.Application.Abstractions.Services;
using TheatreMasterService.Domain.Entities;

namespace TheatreMasterService.Application.Services
{
    /// <inheritdoc />
    public class TheatreService(ITheatreRepository theatreRepository) : ITheatreService
    {
        #region Configuration
        private readonly ITheatreRepository _theatreRepository = theatreRepository;
        #endregion

        /// <inheritdoc />
        public async Task<Theatre> CreateTheatreAsync(Theatre theatre)
        {
            return await _theatreRepository.CreateTheatreAsync(theatre);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteTheatreAsync(int id)
        {
            return await _theatreRepository.DeleteTheatreAsync(id);
        }

        /// <inheritdoc />
        public async Task<Theatre?> GetTheatreByIdAsync(int id)
        {
            return await _theatreRepository.GetTheatreByIdAsync(id);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Theatre>> GetTheatresAsync()
        {
            return await _theatreRepository.GetTheatresAsync();
        }

        /// <inheritdoc />
        public async Task<bool> UpdateTheatreAsync(int id, Theatre theatre)
        {
            var existingTheatre = await _theatreRepository.GetTheatreByIdAsync(id);
            if (existingTheatre == null)
                return false;
            existingTheatre.TheatreName = theatre.TheatreName;
            existingTheatre.City = theatre.City;
            return await _theatreRepository.UpdateTheatreAsync(existingTheatre);
        }
    }
}