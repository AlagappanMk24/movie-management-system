using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace MoviesService.Infrastructure.Persistence.Context
{
    /// <summary>
    /// Provides database connection factory management for Dapper micro-ORM queries.
    /// </summary>
    public class DapperContext(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        /// <summary>
        /// Creates and returns a new SQL Server database connection.
        /// </summary>
        /// <returns>An active <see cref="IDbConnection"/> instance.</returns>
        public IDbConnection CreateConnection()
            => new SqlConnection(_configuration.GetConnectionString("MovieServiceConnection"));
    }
}