namespace BookingService.Application.DTOs
{
    /// <summary>
    /// Represents the complete data model required to render a ticket PDF document and QR code.
    /// </summary>
    public class TicketPdfModel
    {
        /// <summary>Gets or sets the booking identifier.</summary>
        public int BookingId { get; set; }

        /// <summary>Gets or sets the movie title.</summary>
        public string MovieName { get; set; } = string.Empty;

        /// <summary>Gets or sets the theatre name.</summary>
        public string TheatreName { get; set; } = string.Empty;

        /// <summary>Gets or sets the screen name.</summary>
        public string Screen { get; set; } = string.Empty;

        /// <summary>Gets or sets the total seat count.</summary>
        public int SeatCount { get; set; }

        /// <summary>Gets or sets the scheduled show time string.</summary>
        public string ShowTime { get; set; } = string.Empty;

        /// <summary>Gets or sets the total monetary amount.</summary>
        public decimal Amount { get; set; }

        /// <summary>Gets or sets the name of the user who booked the ticket.</summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>Gets or sets the theatre physical address.</summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>Gets or sets the scheduled show date.</summary>
        public DateTime ShowDate { get; set; }

        /// <summary>Gets or sets the movie poster image as a byte array.</summary>
        public byte[]? MoviePoster { get; set; } = null;

        /// <summary>Gets or sets the generated QR code image as a byte array.</summary>
        public byte[]? QrCodeImage { get; set; } = null;

        /// <summary>Gets or sets the list of specific assigned seat numbers.</summary>
        public List<string>? SeatNos { get; set; } = [];
    }
}