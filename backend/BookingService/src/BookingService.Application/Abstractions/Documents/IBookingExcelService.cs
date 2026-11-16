namespace BookingService.Application.Abstractions.Documents
{
    /// <summary>
    /// Handles parsing and bulk processing of booking seat spreadsheets.
    /// </summary>
    public interface IBookingExcelService
    {
        /// <summary>
        /// Validates, parses, and bulk-inserts seat records from an uploaded Excel stream.
        /// </summary>
        Task UploadAndBulkInsertSeatsAsync(Stream fileStream);
    }
}