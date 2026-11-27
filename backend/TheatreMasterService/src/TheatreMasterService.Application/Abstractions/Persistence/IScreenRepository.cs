using TheatreMasterService.Domain.Entities;

namespace TheatreMasterService.Application.Abstractions.Persistence
{
    /// <summary>
    /// Defines repository contract operations for managing screen data persistence.
    /// </summary>
    public interface IScreenRepository
    {
        /// <summary>Retrieves all screen records.</summary>
        Task<IEnumerable<Screen>> GetScreensAsync();

        /// <summary>Retrieves a screen by its unique identifier.</summary>
        Task<Screen?> GetScreenByIdAsync(Guid id);

        /// <summary>Creates a new screen record.</summary>
        Task<Screen> CreateScreenAsync(Screen screen);

        /// <summary>Updates an existing screen record.</summary>
        Task<Screen?> UpdateScreenAsync(Screen screen);

        /// <summary>Deletes a screen record by identifier.</summary>
        Task<bool> DeleteScreenAsync(Guid id);
    }
}