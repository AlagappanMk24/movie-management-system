using BookingService.Application.Abstractions.Persistence;
using BookingService.Application.DTOs.Excel;
using BookingService.Infrastructure.Persistence.Context;
using Dapper;
using System.Data;

namespace BookingService.Infrastructure.Repositories
{
    /// <inheritdoc />
    public class BookingSeatRepository(DapperContext context) : IBookingSeatRepository
    {
        #region Configuration
        private readonly DapperContext _context = context;
        #endregion

        /// <inheritdoc />
        public async Task BulkInsertSeatsAsync(
            int bookingId,
            int showId,
            List<BookingSeatTvpDto> seats)
        {
            using var connection = _context.CreateConnection();

            var table = new DataTable();
            table.Columns.Add("SeatNo", typeof(string));
            table.Columns.Add("Price", typeof(int));

            foreach (var seat in seats)
            {
                table.Rows.Add(seat.SeatNo, seat.Price);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@BookingId", bookingId);
            parameters.Add("@ShowId", showId);
            parameters.Add(
                "@Seats",
                table.AsTableValuedParameter("BookingSeatType")
            );

            await connection.ExecuteAsync(
                "SP_BulkInsertBookingSeats",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        /// <inheritdoc />
        public async Task<List<string>> GetBookedSeatsByBookingIdAsync(int bookingId)
        {
            using var connection = _context.CreateConnection();

            var seats = await connection.QueryAsync<string>(
                "SELECT SeatNo FROM BookingSeats WHERE BookingId = @BookingId",
                new { BookingId = bookingId });

            return seats.ToList();
        }
    }
}