namespace TheatreMasterService.Application.DTOs.Shows.Responses
{
    /// <summary>
    /// Response contract representing seat availability for a show.
    /// </summary>
    public sealed record AvailableSeatsResponse(
        Guid ShowId,
        Guid ScreenId,
        int TotalSeats,
        int BookedSeats,
        int AvailableSeats);
}
