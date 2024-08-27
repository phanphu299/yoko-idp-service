using System.Data;

namespace IdpServer.Application.DbConnections
{
    public interface IDbConnectionResolver
    {
        IDbConnection CreateConnection(bool isReadOnly = false);
    }
}
