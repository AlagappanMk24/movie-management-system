namespace BookingService.Application.DTOs.Excel
{
    /// <summary>
    /// Represents a single row parsed from the booking seats Excel upload file.
    /// </summary>
    public class BookingSeatExcelDto
    {
        public int BookingId { get; set; }
        public int ShowId { get; set; }
        public string SeatNo { get; set; } = string.Empty;
        public int Price { get; set; }
    }
}