using System.Data;
using System.Data.SqlClient;

namespace test.Infrastructure.Database.SqlServer
{
    public class SqlServerConnectionFactory: IConnectionFactory
    {
        private readonly string connectionString;

        public SqlServerConnectionFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<IDbConnection> getReadyConnectionAsync()
        {
            var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }
}