using Shared.Contracts.Common;

namespace MoviesService.Domain.Entities
{
    /// <summary>
    /// Represents a movie domain entity containing core movie attributes and audit timestamps.
    /// </summary>
    public class Movies : BaseEntity
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
        /// Gets or sets the release date of the movie.
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
        /// Gets or sets the primary lead actor.
        /// </summary>
        public string Actor { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the primary lead actress.
        /// </summary>
        public string Actress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the language in which the movie is presented.
        /// </summary>
        public string Language { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the duration of the movie in minutes.
        /// </summary>
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the relative URL path of the movie poster image.
        /// </summary>
        public string? PosterUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL link for the movie trailer.
        /// </summary>
        public string? TrailerUrl { get; set; }
    }
}