namespace TheatreMasterService.Application.DTOs.Shows.Responses
{
    public sealed class SeatCountData
    {
        public Guid ShowId { get; set; }
        public int BookedSeatCount { get; set; }
    }
}