using Shared.Contracts.Common;

namespace BookingService.Domain.Entities
{
    /// <summary>
    /// Represents an individual seat reserved as part of a booking.
    /// </summary>
    public class BookingSeat : BaseEntity
    {
        /// <summary>Gets or sets the booking identifier this seat belongs to.</summary>
        public Guid BookingId { get; set; }

        /// <summary>Gets or sets the show identifier for this seat reservation.</summary>
        public Guid ShowId { get; set; }

        /// <summary>Gets or sets the seat label (e.g., "A1", "B4").</summary>
        public string SeatNo { get; set; } = string.Empty;

        /// <summary>Gets or sets the price of this seat.</summary>
        public int Price { get; set; }
    }
}