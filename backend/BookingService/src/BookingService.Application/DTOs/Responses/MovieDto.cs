namespace BookingService.Application.DTOs.Responses
{
    /// <summary>Represents external movie details.</summary>
    public class MovieDto
    {
        public int MovieId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
    }
}