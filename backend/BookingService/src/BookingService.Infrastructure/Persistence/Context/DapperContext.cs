using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BookingService.Infrastructure.Persistence.Context
{
    /// <summary>
    /// Context provider for creating high-performance Dapper database connections.
    /// </summary>
    public class DapperContext(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        /// <summary>
        /// Creates and returns a new SQL Server database connection instance.
        /// </summary>
        /// <returns>An openable <see cref="IDbConnection"/> instance.</returns>
        public IDbConnection CreateConnection()
            => new SqlConnection(_configuration.GetConnectionString("BookingServiceConnection"));
    }
}