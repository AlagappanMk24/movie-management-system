namespace Shared.Contracts.Common
{
    /// <summary>
    /// Standard API response wrapper containing operation status, messages, payload data, and error details.
    /// </summary>
    /// <typeparam name="T">The type of the data payload returned by the endpoint.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>Gets or sets a value indicating whether the request succeeded.</summary>
        public bool Success { get; set; }

        /// <summary>Gets or sets a human-readable message describing the outcome.</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>Gets or sets the response data payload.</summary>
        public T? Data { get; set; }

        /// <summary>Gets or sets optional diagnostic or error details.</summary>
        public string? Details { get; set; }
    }
}