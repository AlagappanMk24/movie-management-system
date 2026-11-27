using System.ComponentModel.DataAnnotations;

namespace TheatreMasterService.Application.DTOs.Request
{
    /// <summary>
    /// Data transfer object used for creating and updating theatre records.
    /// </summary>
    public class TheatreCreateUpdateDto
    {
        /// <summary>
        /// Gets or sets the commercial name of the theatre complex.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string TheatreName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city where the theatre is located.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the physical street address of the theatre.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the contact telephone number for the theatre.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}