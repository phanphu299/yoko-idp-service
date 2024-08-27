using System.Data;

namespace IdpServer.Application.DbConnections
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
