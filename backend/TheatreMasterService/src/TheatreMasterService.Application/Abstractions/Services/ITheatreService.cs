using TheatreMasterService.Domain.Entities;

namespace TheatreMasterService.Application.Abstractions.Services
{
    /// <summary>
    /// Defines business use cases and operations for managing theatre complexes.
    /// </summary>
    public interface ITheatreService
    {
        /// <summary>Retrieves all theatre complexes.</summary>
        /// <returns>A collection of theatre complexes.</returns>
        Task<IEnumerable<Theatre>> GetTheatresAsync();

        /// <summary>Retrieves a specific theatre by its identifier.</summary>
        /// <param name="id">The theatre identifier.</param>
        /// <returns>The matching theatre object, or null if not found.</returns>
        Task<Theatre?> GetTheatreByIdAsync(int id);

        /// <summary>Creates a new theatre complex record.</summary>
        /// <param name="theatre">The theatre entity to create.</param>
        /// <returns>The created theatre entity.</returns>
        Task<Theatre> CreateTheatreAsync(Theatre theatre);

        /// <summary>Updates an existing theatre complex.</summary>
        /// <param name="id">The theatre identifier.</param>
        /// <param name="theatre">The updated theatre payload.</param>
        /// <returns>True if update was successful; otherwise, false.</returns>
        Task<bool> UpdateTheatreAsync(int id, Theatre theatre);

        /// <summary>Deletes a theatre complex by identifier.</summary>
        /// <param name="id">The theatre identifier.</param>
        /// <returns>True if deletion was successful; otherwise, false.</returns>
        Task<bool> DeleteTheatreAsync(int id);
    }
}