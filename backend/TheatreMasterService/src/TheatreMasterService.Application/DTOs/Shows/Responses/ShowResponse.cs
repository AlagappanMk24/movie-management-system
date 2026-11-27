namespace TheatreMasterService.Application.DTOs.Shows.Responses
{
    /// <summary>
    /// Response contract representing a scheduled movie show.
    /// </summary>
    public sealed record ShowResponse(
        Guid ShowId,
        Guid MovieId,
        Guid ScreenId,
        DateTime ShowDate,
        TimeSpan ShowTime,
        int Price,
        DateTime Created,
        DateTime Modified);
}