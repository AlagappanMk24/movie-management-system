using TheatreMasterService.Domain.Entities;

namespace TheatreMasterService.Application.Abstractions.Persistence
{
    /// <summary>
    /// Defines repository contract operations for managing movie show schedule persistence.
    /// </summary>
    public interface IShowRepository
    {
        /// <summary>Retrieves all show schedule records.</summary>
        Task<IEnumerable<Show>> GetShowsAsync();

        /// <summary>Retrieves a show by its unique identifier.</summary>
        Task<Show?> GetShowByIdAsync(Guid id);

        /// <summary>Creates a new show schedule record.</summary>
        Task<Show> CreateShowAsync(Show show);

        /// <summary>Updates an existing show record.</summary>
        Task<bool> UpdateShowAsync(Show show);

        /// <summary>Deletes a show record by identifier.</summary>
        Task<bool> DeleteShowAsync(Guid id);

        /// <summary>Retrieves all shows associated with a specific movie identifier.</summary>
        Task<IEnumerable<Show>> GetShowsByMovieId(Guid movieId);
    }
}