namespace MoviesService.Application.DTOs.Response
{
    /// <summary>
    /// Represents the response payload containing the binary file stream and content type for a movie poster.
    /// </summary>
    public class MoviePosterResponse
    {
        /// <summary>
        /// Gets or sets the binary content bytes of the poster image.
        /// </summary>
        public byte[] FileBytes { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Gets or sets the MIME content type (e.g., image/jpeg, image/png).
        /// </summary>
        public string ContentType { get; set; } = "image/jpeg";
    }
}