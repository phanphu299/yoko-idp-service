using System;
using System.Linq;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Domain.Entity;
using IdpServer.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AHI.Infrastructure.Repository.Generic;
using System.Threading.Tasks;
using IdpServer.Application.DbConnections;
using System.Data;
using Dapper;

namespace IdpServer.Persistence.Repository
{
    public class PersistedGrantRepository : GenericRepository<PersistedGrant, string>, IPersistedGrantRepository
    {
        private readonly IdpDbContext _context;
        private readonly ITenantContext _tenantContext;
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public PersistedGrantRepository(
            IdpDbContext context
            , IDbConnectionFactory dbConnectionFactory
            , ITenantContext tenantContext) : base(context)
        {
            _context = context;
            _tenantContext = tenantContext;
            _dbConnectionFactory = dbConnectionFactory;
        }

        protected IDbConnection GetDbConnection() => _dbConnectionFactory.CreateConnection();

        public override IQueryable<PersistedGrant> AsFetchable()
        {
            return _context.PersistedGrants.AsNoTracking().Select(x => new PersistedGrant { Id = x.Id, SubjectId = x.SubjectId, Expiration = x.Expiration });
        }

        public async Task RemoveAllAsync(string subjectId)
        {
            using (var connection = GetDbConnection())
            {
                var result = await connection.ExecuteAsync(@$"DELETE FROM PersistedGrants WHERE SubjectId = @SubjectId", new { SubjectId = subjectId });
                connection.Close();
            }
        }

        protected override void Update(PersistedGrant requestObject, PersistedGrant targetObject)
        {
            
        }
    }
}
