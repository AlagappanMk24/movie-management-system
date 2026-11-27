using Shared.Contracts.Common;
using System.ComponentModel.DataAnnotations;

namespace BookingService.Domain.Entities
{
    /// <summary>
    /// Represents a movie ticket booking entity in the database.
    /// </summary>
    public class Booking : BaseEntity
    {
        /// <summary>Gets or sets the identifier of the user who made the booking.</summary>
        [Required]
        public Guid UserId { get; set; }

        /// <summary>Gets or sets the show identifier associated with the booking.</summary>
        [Required]
        public Guid ShowId { get; set; }

        /// <summary>Gets or sets the total number of seats reserved in this booking.</summary>
        [Required]
        public int SeatCount { get; set; }

        /// <summary>Gets or sets the payment method used (e.g., UPI, Credit Card).</summary>
        public string PaymentMethod { get; set; } = string.Empty;

        /// <summary>Gets or sets the current status of the payment (e.g., Completed, Cancelled).</summary>
        public string PaymentStatus { get; set; } = string.Empty;

        /// <summary>Gets or sets the total monetary amount for the booking.</summary>
        public int TotalAmount { get; set; }
    }
}