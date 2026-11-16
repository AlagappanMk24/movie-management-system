using Microsoft.AspNetCore.Http;

namespace MoviesService.Application.DTOs
{
    /// <summary>
    /// Data transfer object used for creating and updating movie records.
    /// </summary>
    public class MovieCreateUpdateDto
    {
        /// <summary>
        /// Gets or sets the title of the movie.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the genre of the movie.
        /// </summary>
        public string Genre { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the official release date of the movie.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Gets or sets the director of the movie.
        /// </summary>
        public string Director { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description or synopsis of the movie.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the lead actor of the movie.
        /// </summary>
        public string Actor { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the lead actress of the movie.
        /// </summary>
        public string Actress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the primary language of the movie.
        /// </summary>
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the duration of the movie in minutes.
        /// </summary>
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the URL for the movie trailer.
        /// </summary>
        public string? TrailerUrl { get; set; }

        /// <summary>
        /// Gets or sets the optional poster image file uploaded with the request.
        /// </summary>
        public IFormFile? File { get; set; }
    }
}