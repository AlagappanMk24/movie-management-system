using TheatreMasterService.Application.DTOs.Shows.Requests;
using TheatreMasterService.Application.DTOs.Shows.Responses;
using TheatreMasterService.Domain.Entities;

namespace TheatreMasterService.Application.Abstractions.Services
{
    /// <summary>
    /// Defines business use cases and operations for managing movie shows and schedules.
    /// </summary>
    public interface IShowService
    {
        /// <summary>Retrieves all movie show schedules.</summary>
        /// <returns>A collection of show schedules.</returns>
        Task<IEnumerable<ShowResponse>> GetShowsAsync();

        /// <summary>Retrieves a specific show by its identifier.</summary>
        /// <param name="id">The show identifier.</param>
        /// <returns>The matching show object.</returns>
        Task<ShowResponse?> GetShowByIdAsync(Guid id);

        /// <summary>Creates a new movie show schedule.</summary>
        /// <param name="show">The show entity to create.</param>
        /// <returns>The created show entity.</returns>
        Task<ShowResponse> CreateShowAsync(CreateShowRequest request);

        /// <summary>Updates an existing show schedule.</summary>
        /// <param name="id">The show identifier.</param>
        /// <param name="show">The updated show payload.</param>
        /// <returns>True if update was successful; otherwise, false.</returns>
        Task<bool> UpdateShowAsync(Guid id, UpdateShowRequest request);

        /// <summary>Deletes a show schedule by identifier.</summary>
        /// <param name="id">The show identifier.</param>
        /// <returns>True if deletion was successful; otherwise, false.</returns>
        Task<bool> DeleteShowAsync(Guid id);

        /// <summary>Retrieves all shows associated with a specific movie identifier.</summary>
        /// <param name="movieId">The movie identifier.</param>
        /// <returns>A collection of shows for the movie.</returns>
        Task<IEnumerable<ShowResponse>> GetShowsByMovieIdAsync(Guid movieId);

        /// <summary>Calculates available seating inventory for a specific show.</summary>
        /// <param name="showId">The show identifier.</param>
        /// <returns>An anonymous object containing seating summary details.</returns>
        Task<AvailableSeatsResponse> GetAvailableSeatsAsync(Guid showId);

        /// <summary>Reduces available seat count upon booking confirmation.</summary>
        /// <param name="showId">The show identifier.</param>
        /// <param name="seatCount">The number of seats to reduce.</param>
        Task ReduceSeatsAsync(Guid showId, int seatCount);

        Task<ScreenLayoutDto> GetScreenLayoutAsync(Guid showId);
    }
}