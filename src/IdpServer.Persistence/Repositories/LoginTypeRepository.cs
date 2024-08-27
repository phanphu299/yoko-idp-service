using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AHI.Infrastructure.Repository.Generic;
using Dapper;
using IdpServer.Application.DbConnections;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Application.User.Model;
using IdpServer.Domain.Entity;
using IdpServer.Persistence.Context;

namespace IdpServer.Persistence.Repository
{
    public class LoginTypeRepository : GenericRepository<LoginType, string>, ILoginTypeRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public LoginTypeRepository(IdpDbContext context, IDbConnectionFactory dbConnectionFactory)
            : base(context)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        protected IDbConnection GetDbConnection() => _dbConnectionFactory.CreateConnection();

        protected override void Update(LoginType requestObject, LoginType targetObject)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<LoginTypeDto>> GetLoginTypesAsync()
        {
            using (var connection = GetDbConnection())
            {
                var result = await connection.QueryAsync<LoginTypeDto>(@$"select
                                                                        code as Code,
                                                                        name as Name
                                                                    from login_types with (nolock) where deleted=0");
                connection.Close();
                return result.ToList();
            }
        }
    }
}
