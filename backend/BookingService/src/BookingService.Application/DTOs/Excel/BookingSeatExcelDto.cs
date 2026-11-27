namespace BookingService.Application.DTOs.Excel
{
    /// <summary>
    /// Represents a single row parsed from the booking seats Excel upload file.
    /// </summary>
    public class BookingSeatExcelDto
    {
        public Guid BookingId { get; set; }
        public Guid ShowId { get; set; }
        public string SeatNo { get; set; } = string.Empty;
        public int Price { get; set; }
    }
}