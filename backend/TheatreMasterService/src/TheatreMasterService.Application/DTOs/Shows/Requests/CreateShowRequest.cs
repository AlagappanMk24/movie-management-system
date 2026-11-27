using System.ComponentModel.DataAnnotations;

namespace TheatreMasterService.Application.DTOs.Shows.Requests
{
    /// <summary>
    /// Request payload for creating a new movie show schedule.
    /// </summary>
    public sealed record CreateShowRequest(
        [Required] Guid MovieId,
        [Required] Guid ScreenId,
        [Required] DateTime ShowDate,
        [Required] TimeSpan ShowTime,
        [Range(0, int.MaxValue, ErrorMessage = "Price must be zero or greater.")] int Price);
}