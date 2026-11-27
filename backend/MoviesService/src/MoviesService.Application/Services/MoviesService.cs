using Microsoft.Extensions.Logging;
using MoviesService.Application.Abstractions;
using MoviesService.Application.Abstractions.Persistence;
using MoviesService.Application.DTOs.Request;
using MoviesService.Application.DTOs.Response;
using MoviesService.Application.Helpers;
using MoviesService.Domain.Entities;

namespace MoviesService.Application.Services
{
    /// <summary>
    /// Implements business logic operations for movie management, integrating data access, gateway validation, and file handling.
    /// </summary>
    public class MoviesService(IMoviesRepository moviesRepository, IMicroServiceGateway microServiceGateway, ImageHelper imageHelper, ILogger<MoviesService> logger) : IMoviesService
    {
        #region Configuration
        private readonly IMoviesRepository _moviesRepository = moviesRepository;
        private readonly IMicroServiceGateway _microServiceGateway = microServiceGateway;
        private readonly ImageHelper _imageHelper = imageHelper;
        private readonly ILogger<MoviesService> _logger = logger;
        #endregion

        #region CreateMoviesAsync
        /// <inheritdoc/>
        public async Task<Movies> CreateMoviesAsync(MovieCreateUpdateDto dto)
        {
            string? posterUrl = null;

            if (dto.File != null)
            {
                posterUrl = _imageHelper.SaveImage(dto.File, "posters/movies");
            }

            var movie = new Movies
            {
                Id = Guid.NewGuid(),
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
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                IsDeleted = false
            };

            return await _moviesRepository.CreateMoviesAsync(movie);
        }
        #endregion

        #region DeleteMoviesAsync
        /// <inheritdoc/>
        public async Task<bool> DeleteMoviesAsync(Guid id)
        {
            // 1. Fetch movie to get the PosterUrl before marking as deleted
            var movie = await _moviesRepository.GetMoviesByIdAsync(id);
            if (movie == null)
            {
                throw new KeyNotFoundException("Movie not found.");
            }

            // 2. Check downstream dependencies
            var hasShow = await _microServiceGateway.HasShow(id);
            if (hasShow)
                throw new InvalidOperationException("Cannot delete movie with active shows.");

            var hasBooking = await _microServiceGateway.HasBooking(id);
            if (hasBooking)
                throw new InvalidOperationException("Cannot delete movie with active bookings.");

            var deleted = await _moviesRepository.DeleteMoviesAsync(id);
            if (!deleted)
                return false;

            // 4. Clean up the physical poster file from disk if it exists
            if (!string.IsNullOrEmpty(movie.PosterUrl))
            {
                var relativePath = movie.PosterUrl.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);
                var filePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath));

                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch(Exception ex)
                    {
                        // Optional: Log warning if file deletion fails, but do not block the soft delete transaction
                        _logger.LogError(ex, "An error occurred while deleting the poster file for movie ID {MovieId}", id);
                    }
                }
            }

            return true;
        }
        #endregion

        #region GetMoviesAsync
        /// <inheritdoc/>
        public async Task<IEnumerable<Movies>> GetMoviesAsync()
        {
            return await _moviesRepository.GetMoviesAsync();
        }
        #endregion

        #region GetMoviesByIdAsync
        /// <inheritdoc/>
        public async Task<Movies?> GetMoviesByIdAsync(Guid id)
        {
            return await _moviesRepository.GetMoviesByIdAsync(id);
        }
        #endregion

        #region UpdateMoviesAsync
        /// <inheritdoc/>
        public async Task<Movies?> UpdateMoviesAsync(Guid id, MovieCreateUpdateDto dto)
        {
            var movie = await _moviesRepository.GetMoviesByIdAsync(id);
            if (movie == null)
                return null;

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

        #region GetMoviePosterAsync
        /// <inheritdoc/>
        public async Task<MoviePosterResponse> GetMoviePosterAsync(GetMoviePosterRequest request)
        {
            var movie = await _moviesRepository.GetMoviesByIdAsync(request.MovieId);
            if (movie == null || string.IsNullOrEmpty(movie.PosterUrl))
            {
                throw new KeyNotFoundException("Movie or poster image not found.");
            }

            // Normalize slashes and trim leading slashes
            var relativePath = movie.PosterUrl.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar);

            // Point directly to the wwwroot folder where static files are stored
            var filePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath));

            if (!File.Exists(filePath))
            {
                throw new KeyNotFoundException($"Poster file does not exist on disk at path: {filePath}");
            }

            var fileBytes = await File.ReadAllBytesAsync(filePath);
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            var contentType = extension switch
            {
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "image/jpeg"
            };

            return new MoviePosterResponse
            {
                FileBytes = fileBytes,
                ContentType = contentType
            };
        }
        #endregion
    }
}