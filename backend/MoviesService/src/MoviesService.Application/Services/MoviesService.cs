using MoviesService.Application.Abstractions;
using MoviesService.Application.Abstractions.Persistence;
using MoviesService.Application.DTOs;
using MoviesService.Application.Helpers;
using MoviesService.Domain.Entities;

namespace MoviesService.Application.Services
{
    /// <summary>
    /// Implements business logic operations for movie management, integrating data access, gateway validation, and file handling.
    /// </summary>
    public class MoviesService(IMoviesRepository moviesRepository, IMicroServiceGateway microServiceGateway, ImageHelper imageHelper) : IMoviesService
    {
        #region Configuration
        private readonly IMoviesRepository _moviesRepository = moviesRepository;
        private readonly IMicroServiceGateway _microServiceGateway = microServiceGateway;
        private readonly ImageHelper _imageHelper = imageHelper;
        #endregion

        #region CreateMoviesAsync
        /// <summary>
        /// Creates a new movie record, saving any uploaded poster image and persisting the entity via repository.
        /// </summary>
        /// <param name="dto">The movie creation payload.</param>
        /// <returns>The created movie entity.</returns>
        public async Task<Movies> CreateMoviesAsync(MovieCreateUpdateDto dto)
        {
            string? posterUrl = null;

            if (dto.File != null)
            {
                posterUrl = _imageHelper.SaveImage(dto.File, "posters/movies");
            }

            var movie = new Movies
            {
                Title = dto.Title,
                Genre = dto.Genre,
                ReleaseDate = dto.ReleaseDate,
                Director = dto.Director,
                Description = dto.Description,
                Actor = dto.Actor,
                Actress = dto.Actress,
                Language = dto.Language,
                DurationMinutes = dto.DurationMinutes,
                TrailerUrl = dto.TrailerUrl,
                PosterUrl = posterUrl,
                Created = DateTime.Now,
                Modified = DateTime.Now
            };

            return await _moviesRepository.CreateMoviesAsync(movie);
        }
        #endregion

        #region DeleteMoviesAsync
        /// <summary>
        /// Deletes a movie by ID after confirming through external gateways that no active shows or bookings depend on it.
        /// </summary>
        /// <param name="id">The unique identifier of the movie.</param>
        /// <returns>True if deleted successfully.</returns>
        /// <exception cref="InvalidOperationException">Thrown when active shows or bookings exist for the movie.</exception>
        public async Task<bool> DeleteMoviesAsync(int id)
        {
            var hasShow = await _microServiceGateway.HasShow(id);
            if (hasShow)
                throw new InvalidOperationException("Cannot delete movie with active shows.");
            var hasBooking = await _microServiceGateway.HasBooking(id);
            if (hasBooking)
                throw new InvalidOperationException("Cannot delete movie with active bookings.");
            return await _moviesRepository.DeleteMoviesAsync(id);
        }
        #endregion

        #region GetMoviesAsync
        /// <summary>
        /// Retrieves all movie records from the repository.
        /// </summary>
        /// <returns>A collection of movies.</returns>
        public async Task<IEnumerable<Movies>> GetMoviesAsync()
        {
            return await _moviesRepository.GetMoviesAsync();
        }
        #endregion

        #region GetMoviesByIdAsync
        /// <summary>
        /// Retrieves a specific movie by its unique identifier.
        /// </summary>
        /// <param name="id">The movie identifier.</param>
        /// <returns>The matching movie entity, or null if not found.</returns>
        public async Task<Movies?> GetMoviesByIdAsync(int id)
        {
            return await _moviesRepository.GetMoviesByIdAsync(id);
        }
        #endregion

        #region UpdateMoviesAsync
        /// <summary>
        /// Updates an existing movie record, refreshing its details and handling new poster uploads if provided.
        /// </summary>
        /// <param name="id">The movie identifier.</param>
        /// <param name="dto">The update payload.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        public async Task<bool> UpdateMoviesAsync(int id, MovieCreateUpdateDto dto)
        {
            var movie = await _moviesRepository.GetMoviesByIdAsync(id);
            if (movie == null)
                return false;

            if (dto.File != null)
            {
                movie.PosterUrl = _imageHelper.SaveImage(dto.File, "posters/movies");
            }

            movie.Title = dto.Title;
            movie.Genre = dto.Genre;
            movie.ReleaseDate = dto.ReleaseDate;
            movie.Director = dto.Director;
            movie.Description = dto.Description;
            movie.Actor = dto.Actor;
            movie.Actress = dto.Actress;
            movie.Language = dto.Language;
            movie.DurationMinutes = dto.DurationMinutes;
            movie.TrailerUrl = dto.TrailerUrl;
            movie.Modified = DateTime.Now;

            return await _moviesRepository.UpdateMoviesAsync(movie);
        }
        #endregion
    }
}