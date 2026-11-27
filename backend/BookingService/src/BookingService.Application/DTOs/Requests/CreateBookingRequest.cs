using System.ComponentModel.DataAnnotations;

namespace BookingService.Application.DTOs.Requests
{
    public class CreateBookingRequest
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid ShowId { get; set; }

        [Range(1, 50)]
        public int SeatCount { get; set; }

        [Required, StringLength(100)]
        public string PaymentMethod { get; set; } = string.Empty;
    }
}