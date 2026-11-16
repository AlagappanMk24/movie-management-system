using BookingService.Application.Abstractions.Documents;
using BookingService.Application.Abstractions.Persistence;
using BookingService.Application.DTOs;
using BookingService.Application.DTOs.Excel;
using ClosedXML.Excel;

namespace BookingService.Infrastructure.Documents
{
    /// <inheritdoc />
    public class BookingExcelService(IBookingSeatRepository seatRepository) : IBookingExcelService
    {
        private readonly IBookingSeatRepository _seatRepository = seatRepository;

        /// <inheritdoc />
        public async Task UploadAndBulkInsertSeatsAsync(Stream fileStream)
        {
            var rowsData = new List<BookingSeatExcelDto>();

            using var workbook = new XLWorkbook(fileStream);
            var sheet = workbook.Worksheet(1);

            if (sheet.Cell(1, 1).GetString() != "BookingId" ||
                sheet.Cell(1, 2).GetString() != "ShowId" ||
                sheet.Cell(1, 3).GetString() != "SeatNo" ||
                sheet.Cell(1, 4).GetString() != "Price")
            {
                throw new InvalidOperationException("Invalid Excel header format.");
            }

            foreach (var row in sheet.RowsUsed().Skip(1))
            {
                var seatNo = row.Cell(3).GetString().Trim();

                if (!row.Cell(1).TryGetValue(out int bookingId) ||
                    !row.Cell(2).TryGetValue(out int showId) ||
                    string.IsNullOrWhiteSpace(seatNo) ||
                    !row.Cell(4).TryGetValue(out int price))
                {
                    throw new InvalidOperationException($"Invalid data at row {row.RowNumber()}.");
                }

                rowsData.Add(new BookingSeatExcelDto
                {
                    BookingId = bookingId,
                    ShowId = showId,
                    SeatNo = seatNo,
                    Price = price
                });
            }

            if (rowsData.GroupBy(x => new { x.ShowId, x.SeatNo }).Any(g => g.Count() > 1))
            {
                throw new InvalidOperationException("Duplicate SeatNo found in Excel.");
            }

            foreach (var group in rowsData.GroupBy(x => new { x.BookingId, x.ShowId }))
            {
                var seats = group.Select(x => new BookingSeatTvpDto
                {
                    SeatNo = x.SeatNo,
                    Price = x.Price
                }).ToList();

                await _seatRepository.BulkInsertSeatsAsync(group.Key.BookingId, group.Key.ShowId, seats);
            }
        }
    }
}