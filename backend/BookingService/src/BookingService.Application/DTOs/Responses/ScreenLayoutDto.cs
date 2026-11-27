namespace BookingService.Application.DTOs.Responses
{
    public class ScreenLayoutDto
    {
        public int Rows { get; set; }
        public int SeatsPerRow { get; set; }   // e.g. 10 → A1..A10, B1..B10
    }
}