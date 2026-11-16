namespace BookingService.Application.DTOs.Excel
{
    /// <summary>
    /// Data transfer object mapping seat records for SQL Table-Valued Parameters.
    /// </summary>
    public class BookingSeatTvpDto
    {
        /// <summary>Gets or sets the seat number.</summary>
        public string SeatNo { get; set; } = string.Empty;

        /// <summary>Gets or sets the price of the individual seat.</summary>
        public int Price { get; set; }
    }
}