using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IdpServer.Customized
{
    public class MemoryCacheResourceStore : ResourceStore
    {
        private readonly IMemoryCache _memoryCache;
        public MemoryCacheResourceStore(IConfigurationDbContext context, ILogger<ResourceStore> logger, IMemoryCache memoryCache) : base(context, logger)
        {
            _memoryCache = memoryCache;
        }
        public override async Task<ApiResource> FindApiResourceAsync(string name)
        {
            var key = $"idp_resource_{name}";
            var apiResource = _memoryCache.Get<ApiResource>(key);
            if (apiResource == null)
            {
                apiResource = await base.FindApiResourceAsync(name);
                if (apiResource != null)
                {
                    _memoryCache.Set(key, apiResource, TimeSpan.FromHours(1));
                }
            }
            return apiResource;
        }
        public override async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var key = $"idp_resource_scope_{string.Join("_", scopeNames)}";
            var apiResources = _memoryCache.Get<IEnumerable<ApiResource>>(key);
            if (apiResources == null)
            {
                apiResources = await base.FindApiResourcesByScopeAsync(scopeNames);
                if (apiResources != null)
                {
                    _memoryCache.Set(key, apiResources, TimeSpan.FromHours(1));
                }
            }
            return apiResources;
        }
        public override async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var key = $"idp_identity_resource_scope_{string.Join("_", scopeNames)}";
            var identityResoures = _memoryCache.Get<IEnumerable<IdentityResource>>(key);
            if (identityResoures == null)
            {
                identityResoures = await base.FindIdentityResourcesByScopeAsync(scopeNames);
                if (identityResoures != null)
                {
                    _memoryCache.Set(key, identityResoures, TimeSpan.FromHours(1));
                }
            }
            return identityResoures;
        }
        public override async Task<Resources> GetAllResourcesAsync()
        {
            var key = $"idp_resources";
            var resources = _memoryCache.Get<Resources>(key);
            if (resources == null)
            {
                resources = await base.GetAllResourcesAsync();
                if (resources != null)
                {
                    _memoryCache.Set(key, resources, TimeSpan.FromHours(1));
                }
            }
            return resources;
        }
    }
}