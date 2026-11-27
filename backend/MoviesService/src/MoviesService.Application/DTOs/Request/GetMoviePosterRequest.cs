namespace MoviesService.Application.DTOs.Request
{
    /// <summary>
    /// Represents the request payload for retrieving a movie poster.
    /// </summary>
    public class GetMoviePosterRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier of the movie.
        /// </summary>
        public Guid MovieId { get; set; }
    }
}