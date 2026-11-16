using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace UserService.Infrastructure.Persistence.Context
{
    /// <summary>
    /// Manages database connections for Dapper data access.
    /// </summary>
    public class DapperContext(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        /// <summary>
        /// Creates and returns a new SQL database connection.
        /// </summary>
        public IDbConnection CreateConnection()
            => new SqlConnection(_configuration.GetConnectionString("UserServiceConnection"));
    }
}