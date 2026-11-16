namespace BookingService.Application.DTOs.Analytics
{
    /// <summary>
    /// Data transfer object representing box-office collection revenue grouped by movie.
    /// </summary>
    public class MovieCollectionDto
    {
        /// <summary>Gets or sets the title of the movie.</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Gets or sets the total cumulative collection revenue for the movie.</summary>
        public int MovieCollection { get; set; }
    }
}