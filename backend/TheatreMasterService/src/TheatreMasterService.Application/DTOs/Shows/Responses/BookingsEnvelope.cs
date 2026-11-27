using System;
namespace TheatreMasterService.Application.DTOs.Shows.Responses
{
    public sealed class BookingsEnvelope
    {
        public bool Success { get; set; }
        public List<object>? Data { get; set; }
    }
}
