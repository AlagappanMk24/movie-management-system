namespace BookingService.Application.DTOs.Responses
{
    /// <summary>Represents external user profile details.</summary>
    public class UserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}