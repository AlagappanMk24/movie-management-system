namespace BookingService.Application.DTOs.Responses
{
    public class BookingResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ShowId { get; set; }
        public int SeatCount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public int TotalAmount { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public List<SeatResponse> Seats { get; set; } = [];
    }
    public class SeatResponse
    {
        public string SeatNo { get; set; } = "";
        public int Price { get; set; }
    }
}