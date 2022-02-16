using System.Data;

namespace test.Infrastructure.Database
{
    public interface IConnectionFactory
    {
         public Task<IDbConnection> getReadyConnectionAsync();
    }
}