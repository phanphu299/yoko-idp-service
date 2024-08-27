using System;
using System.Linq;
using System.Threading.Tasks;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Domain.Entity;
using IdpServer.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AHI.Infrastructure.Repository.Generic;
using System.Collections.Generic;
using IdpServer.Application.Constant;

namespace IdpServer.Persistence.Repository
{
    public class ClientRepository : GenericRepository<Client, int>, IClientRepository
    {
        private readonly IdpDbContext _context;
        private readonly ITenantContext _tenantContext;

        public ClientRepository(
            IdpDbContext context
            , ITenantContext tenantContext) : base(context)
        {
            _context = context;
            _tenantContext = tenantContext;
        }

        public override IQueryable<Client> AsQueryable()
        {
            return AsQueryableTag(base.AsQueryable())
                    .Where(x =>
                        x.ProjectClient.TenantId == _tenantContext.TenantId &&
                        x.ProjectClient.SubscriptionId == _tenantContext.SubscriptionId &&
                        x.ProjectClient.ProjectId == _tenantContext.ProjectId)
                    .Include(x => x.ClientSecrets);
        }

        public override IQueryable<Client> AsFetchable()
        {
            return _context.Clients.AsNoTracking().Select(x => new Client { Id = x.Id, ClientId = x.ClientId, ClientName = x.ClientName });
        }

        public Task<Client> FindByClientNameAsync(string name)
        {
            return AsQueryableTag(_context.Clients)
                    .Where(x => x.ClientName == name)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

        }

        public async Task<Client> FindByClientIdAsync(Guid clientId)
        {
            // this is not good for performance
            var clientIdInString = clientId.ToString();
            var baseQuery = _context.Clients.Where(x => x.ClientId == clientIdInString);

            var client = (await baseQuery.ToArrayAsync())
                .SingleOrDefault(x => x.ClientId == clientIdInString);
            if (client == null)
                return null;

            await baseQuery.Include(x => x.ClientSecrets).LoadAsync();
            await baseQuery.Include(x => x.ClientPostLogoutRedirectUris).LoadAsync();
            await baseQuery.Include(x => x.ClientRedirectUris).LoadAsync();
            await baseQuery.Include(x => x.ClientGrantTypes).LoadAsync();
            await baseQuery.Include(x => x.ClientScopes).LoadAsync();
            await AsQueryableTag(baseQuery).LoadAsync();
            return client;
        }

        protected override void Update(Client requestObject, Client targetObject)
        {
            targetObject.ClientName = requestObject.ClientName;
            targetObject.Updated = requestObject.Updated;
            targetObject.ClientSecrets = requestObject.ClientSecrets;
            targetObject.ClientScopes = requestObject.ClientScopes;
        }

        public Task RetrieveAsync(IEnumerable<Client> input)
        {
            _context.Database.SetCommandTimeout(RetrieveConstants.TIME_OUT);
            return _context.Clients.AddRangeAsync(input);
        }

        public async Task<bool> UpdateClientRedirectUrisAsync(int id, IEnumerable<string> redirectUris)
        {
            await RemoveClientRedirectUrisAsync(id);
            await _context.ClientRedirectUris.AddRangeAsync(redirectUris.Select(x => new ClientRedirectUris()
            {
                ClientId = id,
                RedirectUri = x
            }));
            return true;
        }

        public async Task<bool> UpdateClientPostLogoutRedirectUrisAsync(int id, IEnumerable<string> redirectUris)
        {
            await RemoveClientPostLogoutRedirectUrisAsync(id);
            await _context.ClientPostLogoutRedirectUris.AddRangeAsync(redirectUris.Select(x => new ClientPostLogoutRedirectUris()
            {
                ClientId = id,
                PostLogoutRedirectUri = x
            }));
            return true;
        }

        public async Task RemoveClientRedirectUrisAsync(int id)
        {
            var entities = await _context.ClientRedirectUris.Where(x => x.ClientId == id).ToListAsync();
            _context.ClientRedirectUris.RemoveRange(entities);
        }

        public async Task RemoveClientAsync(int id)
        {
            var entity = await _context.Clients.FirstOrDefaultAsync(x => x.Id == id);
            _context.Clients.Remove(entity);
        }

        public async Task RemoveClientPostLogoutRedirectUrisAsync(int id)
        {
            var entities = await _context.ClientPostLogoutRedirectUris.Where(x => x.ClientId == id).ToListAsync();
            _context.ClientPostLogoutRedirectUris.RemoveRange(entities);
        }

        public async Task<Client> AddWithSaveChangeAsync(Client e)
        {
            await base.AddAsync(e);
            await _context.SaveChangesAsync();
            return e;
        }

        public IQueryable<Client> AsQueryableTag(IQueryable<Client> query)
        {
            if (query == null)
            {
                query = base.AsQueryable();
            }

            return query.Include(x => x.EntityTags)
                    .Where(x => !x.EntityTags.Any() || x.EntityTags.Any(a => a.EntityType == EntityTagConstants.CLIENT_ENTITY_NAME));
        }
    }
}