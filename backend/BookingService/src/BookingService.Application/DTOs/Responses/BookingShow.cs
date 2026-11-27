namespace BookingService.Application.DTOs.Responses
{
    /// <summary>
    /// Data transfer object representing show details retrieved from the theatre microservice.
    /// </summary>
    public class BookingShow
    {
        public Guid ShowId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ShowTime { get; set; } = string.Empty;
        public Guid MovieId { get; set; }
        public int Price { get; set; }
        public Guid ScreenId { get; set; }
        public DateTime ShowDate { get; set; }
    }
}