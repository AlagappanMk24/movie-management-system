using System.ComponentModel.DataAnnotations;

namespace BookingService.Application.DTOs.Requests
{
    public class UpdateBookingStatusRequest
    {
        [Required, StringLength(100)]
        public string PaymentStatus { get; set; } = string.Empty;
    }
}