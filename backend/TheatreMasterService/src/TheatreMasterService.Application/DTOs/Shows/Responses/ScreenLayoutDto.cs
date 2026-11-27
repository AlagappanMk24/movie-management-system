namespace TheatreMasterService.Application.DTOs.Shows.Responses
{

    public class ScreenLayoutDto
    {
        public Guid ShowId { get; set; }
        public Guid ScreenId { get; set; }
        public int Rows { get; set; }
        public int SeatsPerRow { get; set; }
        public int TotalSeats { get; set; }
        public List<string> SeatNumbers { get; set; } = [];
    }
}
