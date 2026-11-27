using BookingService.Application.DTOs.Requests;
using BookingService.Domain.Entities;

namespace BookingService.Application.DTOs.Responses
{
    public static class BookingMappingExtensions
    {
        public static BookingResponse ToResponse(this Booking b, IEnumerable<BookingSeat>? seats = null) => new()
        {
            Id = b.Id,
            UserId = b.UserId,
            ShowId = b.ShowId,
            SeatCount = b.SeatCount,
            PaymentMethod = b.PaymentMethod,
            PaymentStatus = b.PaymentStatus,
            TotalAmount = b.TotalAmount,
            Created = b.Created,
            Modified = b.Modified,
            Seats = seats?.Select(s => new SeatResponse { SeatNo = s.SeatNo, Price = s.Price }).ToList() ?? []
        };

        public static IEnumerable<BookingResponse> ToResponses(this IEnumerable<Booking> list)
            => list.Select(b => b.ToResponse());

        public static Booking ToEntity(this CreateBookingRequest r) => new()
        {
            UserId = r.UserId,
            ShowId = r.ShowId,
            SeatCount = r.SeatCount,
            PaymentMethod = r.PaymentMethod
        };
    }
}