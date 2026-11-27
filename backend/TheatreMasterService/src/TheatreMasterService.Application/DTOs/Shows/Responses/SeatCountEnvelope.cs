namespace TheatreMasterService.Application.DTOs.Shows.Responses
{
    public sealed class SeatCountEnvelope
    {
        public bool Success { get; set; }
        public SeatCountData? Data { get; set; }
    }
}