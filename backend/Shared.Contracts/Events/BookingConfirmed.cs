using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Contracts.Events
{
    /// <summary>
    /// Represents the message contract published when a booking has been successfully confirmed.
    /// </summary>
    public class BookingConfirmed
    {
        /// <summary>
        /// Gets or sets the unique identifier of the booking.
        /// </summary>
        public Guid BookingId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the show.
        /// </summary>
        public Guid ShowId { get; set; }

        /// <summary>
        /// Gets or sets the number of seats that were booked.
        /// </summary>
        public int SeatCount { get; set; }

        /// <summary>
        /// Gets or sets the title of the movie.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the scheduled start time of the show.
        /// </summary>
        public TimeSpan ShowTime { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user who made the booking.
        /// </summary>
        public Guid UserId { get; set; }
    }
}