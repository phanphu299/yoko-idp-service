using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AHI.Infrastructure.Cache.Abstraction;
using Microsoft.Extensions.Configuration;
using IdpServer.Application.Constant;

namespace IdpServer.Customized
{
    public class CacheClientStore : ClientStore
    {
        private readonly ICache _cache;
        public CacheClientStore(IConfigurationDbContext context, ILogger<ClientStore> logger, ICache cache, IConfiguration configuration) : base(context, logger)
        {
            _cache = cache;
        }

        public override async Task<Client> FindClientByIdAsync(string clientId)
        {
            var key = string.Format(CacheKey.CLIENT, clientId);
            var clientStore = await _cache.GetHashByKeyAsync<IdentityServer4.EntityFramework.Entities.Client>(key, CacheKey.HASH_FIELD_STORE);
            if (clientStore == null)
            {
                var baseQuery = Context.Clients
                     .Where(x => x.ClientId == clientId);

                clientStore = (await baseQuery.ToArrayAsync())
                    .SingleOrDefault(x => x.ClientId == clientId);

                if (clientStore == null)
                    return null;

                await baseQuery.Include(x => x.AllowedCorsOrigins).SelectMany(c => c.AllowedCorsOrigins).LoadAsync();
                await baseQuery.Include(x => x.AllowedGrantTypes).SelectMany(c => c.AllowedGrantTypes).LoadAsync();
                await baseQuery.Include(x => x.AllowedScopes).SelectMany(c => c.AllowedScopes).LoadAsync();
                await baseQuery.Include(x => x.Claims).SelectMany(c => c.Claims).LoadAsync();
                await baseQuery.Include(x => x.ClientSecrets).SelectMany(c => c.ClientSecrets).LoadAsync();
                await baseQuery.Include(x => x.IdentityProviderRestrictions).SelectMany(c => c.IdentityProviderRestrictions).LoadAsync();
                await baseQuery.Include(x => x.PostLogoutRedirectUris).SelectMany(c => c.PostLogoutRedirectUris).LoadAsync();
                await baseQuery.Include(x => x.Properties).SelectMany(c => c.Properties).LoadAsync();
                await baseQuery.Include(x => x.RedirectUris).SelectMany(c => c.RedirectUris).LoadAsync();

                //Clear client to avoid reference loop
                clientStore.AllowedCorsOrigins?.ForEach(x => x.Client = null);
                clientStore.AllowedGrantTypes?.ForEach(x => x.Client = null);
                clientStore.AllowedScopes?.ForEach(x => x.Client = null);
                clientStore.Claims?.ForEach(x => x.Client = null);
                clientStore.ClientSecrets?.ForEach(x => x.Client = null);
                clientStore.IdentityProviderRestrictions?.ForEach(x => x.Client = null);
                clientStore.PostLogoutRedirectUris?.ForEach(x => x.Client = null);
                clientStore.Properties?.ForEach(x => x.Client = null);
                clientStore.RedirectUris?.ForEach(x => x.Client = null);

                await _cache.SetHashByKeyAsync(key, CacheKey.HASH_FIELD_STORE, clientStore);
            }
            return clientStore.ToModel();
        }
    }
}
