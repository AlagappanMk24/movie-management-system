using TheatreMasterService.Domain.Entities;

namespace TheatreMasterService.Application.Abstractions.Persistence
{
    /// <summary>
    /// Defines repository contract operations for managing theatre complex data persistence.
    /// </summary>
    public interface ITheatreRepository
    {
        /// <summary>Retrieves all theatre complex records.</summary>
        Task<IEnumerable<Theatre>> GetTheatresAsync();

        /// <summary>Retrieves a theatre by its unique identifier.</summary>
        Task<Theatre?> GetTheatreByIdAsync(int id);

        /// <summary>Creates a new theatre record.</summary>
        Task<Theatre> CreateTheatreAsync(Theatre theatre);

        /// <summary>Updates an existing theatre record.</summary>
        Task<bool> UpdateTheatreAsync(Theatre theatre);

        /// <summary>Deletes a theatre record by identifier.</summary>
        Task<bool> DeleteTheatreAsync(int id);
    }
}