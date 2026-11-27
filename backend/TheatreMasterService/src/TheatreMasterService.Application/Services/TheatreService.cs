using TheatreMasterService.Application.Abstractions.Persistence;
using TheatreMasterService.Application.Abstractions.Services;
using TheatreMasterService.Application.DTOs.Request;
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
        public async Task<Theatre> CreateTheatreAsync(TheatreCreateUpdateDto dto)
        {
            var theatre = new Theatre
            {
                Id = Guid.NewGuid(),
                TheatreName = dto.TheatreName,
                City = dto.City,
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber,
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                IsDeleted = false
            };

            return await _theatreRepository.CreateTheatreAsync(theatre);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteTheatreAsync(Guid id)
        {
            return await _theatreRepository.DeleteTheatreAsync(id);
        }

        /// <inheritdoc />
        public async Task<Theatre?> GetTheatreByIdAsync(Guid id)
        {
            return await _theatreRepository.GetTheatreByIdAsync(id);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Theatre>> GetTheatresAsync()
        {
            return await _theatreRepository.GetTheatresAsync();
        }

        /// <inheritdoc />
        public async Task<Theatre?> UpdateTheatreAsync(Guid id, TheatreCreateUpdateDto dto)
        {
            var existingTheatre = await _theatreRepository.GetTheatreByIdAsync(id);
            if (existingTheatre == null)
                return null;

            existingTheatre.TheatreName = dto.TheatreName;
            existingTheatre.City = dto.City;
            existingTheatre.Address = dto.Address;
            existingTheatre.PhoneNumber = dto.PhoneNumber;
            existingTheatre.Modified = DateTime.UtcNow;

            return await _theatreRepository.UpdateTheatreAsync(existingTheatre);
        }
    }
}