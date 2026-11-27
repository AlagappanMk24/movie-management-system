using BookingService.Application.Abstractions.Persistence;
using BookingService.Application.DTOs.Excel;
using BookingService.Domain.Entities;
using BookingService.Infrastructure.Persistence.Context;
using Dapper;
using System.Data;

namespace BookingService.Infrastructure.Persistence.Repositories
{
    /// <inheritdoc />
    public class BookingSeatRepository(DapperContext context) : IBookingSeatRepository
    {
        #region Configuration
        private readonly DapperContext _context = context;
        #endregion

        /// <inheritdoc />
        #region BulkInsertSeatsAsync
        public async Task BulkInsertSeatsAsync(
            Guid bookingId,
            Guid showId,
            List<BookingSeatTvpDto> seats)
        {
            if (seats is null || seats.Count == 0)
                return;

            var table = new DataTable();
            table.Columns.Add("SeatNo", typeof(string));
            table.Columns.Add("Price", typeof(int));

            foreach (var seat in seats)
            {
                table.Rows.Add(seat.SeatNo, seat.Price);
            }

            var parameters = new DynamicParameters();
            parameters.Add("@BookingId", bookingId, DbType.Guid);
            parameters.Add("@ShowId", showId, DbType.Guid);
            parameters.Add(
                "@Seats",
                table.AsTableValuedParameter("dbo.BookingSeatType")
            );

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(
                "SP_BulkInsertBookingSeats",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
        #endregion

        /// <inheritdoc />
        public async Task<List<string>> GetBookedSeatsByBookingIdAsync(Guid bookingId)
        {
            using var connection = _context.CreateConnection();

            var seats = await connection.QueryAsync<string>(
                "SP_GetBookedSeatsByBookingId",
                new { BookingId = bookingId },
                commandType: CommandType.StoredProcedure
            );

            return seats.ToList();
        }

        public async Task<HashSet<string>> GetOccupiedSeatNosAsync(Guid showId)
        {
            using var conn = _context.CreateConnection();
            var seats = await conn.QueryAsync<string>(
                "SP_GetBookedSeatNosByShow",
                new { ShowId = showId },
                commandType: CommandType.StoredProcedure);
            return seats.ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public async Task<List<BookingSeat>> GetSeatsByBookingIdAsync(Guid bookingId)
        {
            using var conn = _context.CreateConnection();
            var rows = await conn.QueryAsync<BookingSeat>(
                "SP_GetBookedSeatsByBookingId",
                new { BookingId = bookingId },
                commandType: CommandType.StoredProcedure);
            return rows.ToList();
        }

        public async Task InsertSeatsAsync(
            Guid bookingId, Guid showId, IEnumerable<BookingSeat> seats,
            IDbConnection conn, IDbTransaction tx)
        {
            foreach (var seat in seats)
            {
                await conn.ExecuteAsync(
                    "SP_InsertBookingSeat",
                    new
                    {
                        Id = Guid.NewGuid(),
                        BookingId = bookingId,
                        ShowId = showId,
                        SeatNo = seat.SeatNo,
                        Price = seat.Price,
                        Created = DateTime.UtcNow,
                        Modified = DateTime.UtcNow
                    },
                    transaction: tx,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task SoftDeleteSeatsByBookingIdAsync(Guid bookingId)
        {
            using var conn = _context.CreateConnection();
            await conn.ExecuteAsync(
                @"UPDATE dbo.BookingSeats 
          SET IsDeleted = 1, Modified = SYSUTCDATETIME() 
          WHERE BookingId = @BookingId AND IsDeleted = 0",
                new { BookingId = bookingId });
        }
    }
}