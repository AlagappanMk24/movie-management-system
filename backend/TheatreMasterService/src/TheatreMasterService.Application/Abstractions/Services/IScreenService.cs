using TheatreMasterService.Domain.Entities;

namespace TheatreMasterService.Application.Abstractions.Services
{
    /// <summary>
    /// Defines business use cases and operations for managing cinema screens.
    /// </summary>
    public interface IScreenService
    {
        /// <summary>Retrieves all screen records.</summary>
        /// <returns>A collection of screens.</returns>
        Task<IEnumerable<Screen>> GetScreensAsync();

        /// <summary>Retrieves a specific screen by its unique identifier.</summary>
        /// <param name="id">The screen identifier.</param>
        /// <returns>The matching screen object, or null if not found.</returns>
        Task<Screen?> GetScreenByIdAsync(int id);

        /// <summary>Creates a new screen record.</summary>
        /// <param name="screen">The screen entity to create.</param>
        /// <returns>The created screen entity.</returns>
        Task<Screen> CreateScreenAsync(Screen screen);

        /// <summary>Updates an existing screen record.</summary>
        /// <param name="id">The screen identifier.</param>
        /// <param name="screen">The updated screen payload.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        Task<bool> UpdateScreenAsync(int id, Screen screen);

        /// <summary>Deletes a screen record by identifier.</summary>
        /// <param name="id">The screen identifier.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        Task<bool> DeleteScreenAsync(int id);
    }
}