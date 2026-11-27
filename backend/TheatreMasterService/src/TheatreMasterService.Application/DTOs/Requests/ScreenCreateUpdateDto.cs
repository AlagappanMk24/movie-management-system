
using System.ComponentModel.DataAnnotations;

namespace TheatreMasterService.Application.DTOs.Request
{
    /// <summary>
    /// Data transfer object used for creating and updating cinema screen records.
    /// </summary>
    public class ScreenCreateUpdateDto
    {
        /// <summary>Gets or sets the identifier of the parent theatre complex.</summary>
        [Required]
        public Guid TheatreId { get; set; }

        /// <summary>Gets or sets the display name or number of the screen (e.g., Screen 1, IMAX).</summary>
        [Required]
        [MaxLength(100)]
        public string ScreenName { get; set; } = string.Empty;

        /// <summary>Gets or sets the projection/audio format type (e.g., 2D, 3D, IMAX, 4DX).</summary>
        [Required]
        [MaxLength(50)]
        public string ScreenType { get; set; } = string.Empty;

        /// <summary>Gets or sets the maximum seating capacity of the screen.</summary>
        [Range(1, 2000)]
        public int SeatCapacity { get; set; }

        // NEW — needed for auto-allocation in Booking
        [Range(1, 30)] public int Rows { get; set; } = 10;
        [Range(1, 60)] public int SeatsPerRow { get; set; } = 12;
    }
}