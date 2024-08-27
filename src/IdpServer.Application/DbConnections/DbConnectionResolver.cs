using System.Data;

namespace IdpServer.Application.DbConnections
{
    public class DbConnectionResolver : IDbConnectionResolver
    {
        private readonly IReadDbConnectionFactory _readDbConnectionFactory;
        private readonly IWriteDbConnectionFactory _writeConnectionFactory;

        public DbConnectionResolver(IWriteDbConnectionFactory writeConnectionFactory, IReadDbConnectionFactory readDbConnectionFactory)
        {
            _writeConnectionFactory = writeConnectionFactory;
            _readDbConnectionFactory = readDbConnectionFactory;
        }

        public IDbConnection CreateConnection(bool isReadOnly = false)
        {
            return isReadOnly ? _readDbConnectionFactory.CreateConnection() : _writeConnectionFactory.CreateConnection();
        }
    }
}
