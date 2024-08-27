using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Stores;
using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using AHI.Infrastructure.Cache.Abstraction;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Application.Service.Abstraction;
namespace IdpServer.Customized
{
    public class CachePersistedGrantStore : PersistedGrantStore, ICachePersistedGrantStore
    {
        private readonly ICache _cache;
        private readonly IPersistedGrantRepository _persistedGrantRepository;
        private readonly ILogger<PersistedGrantStore> _logger;

        public CachePersistedGrantStore(PersistedGrantDbContext context, ILogger<PersistedGrantStore> logger, ICache cache, IPersistedGrantRepository persistedGrantRepository) : base(context, logger)
        {
            _cache = cache;
            _logger = logger;
            _persistedGrantRepository = persistedGrantRepository;
        }

        public override async Task<PersistedGrant> GetAsync(string key)
        {
            _logger.LogInformation($"{nameof(CachePersistedGrantStore)} {nameof(GetAsync)} with key {key}");
            var result = await _cache.GetAsync<PersistedGrant>($"idp_persited_grant_{key}");
            if (result == null)
            {
                result = await base.GetAsync(key);
            }
            return result;
        }

        public override async Task RemoveAsync(string key)
        {
            _logger.LogInformation($"{nameof(CachePersistedGrantStore)} {nameof(RemoveAsync)} with key {key}");
            await _cache.DeleteAsync($"idp_persited_grant_{key}");
            await base.RemoveAsync(key);
        }

        public override async Task StoreAsync(PersistedGrant token)
        {
            _logger.LogInformation($"{nameof(CachePersistedGrantStore)} {nameof(StoreAsync)} with key idp_persited_grant_{token.Key}");
            await base.StoreAsync(token);
            if (token.Expiration.HasValue)
            {
                var duration = token.Expiration.Value - DateTime.Now;
                await _cache.StoreAsync($"idp_persited_grant_{token.Key}", token, duration);
            }
            else
            {
                await _cache.StoreAsync($"idp_persited_grant_{token.Key}", token);
            }
        }

        public async Task RemoveAllAsync(string subjectId)
        {
            var tokens = await base.GetAllAsync(subjectId);
            foreach (var token in tokens)
            {
                await _cache.DeleteAsync($"idp_persited_grant_{token.Key}");
            }
            await _persistedGrantRepository.RemoveAllAsync(subjectId);
        }

        public override async Task RemoveAllAsync(string subjectId, string clientId)
        {
            _logger.LogInformation($"{nameof(CachePersistedGrantStore)} {nameof(RemoveAllAsync)} with subjectId {subjectId}, clientId {clientId}");
            var tokens = await base.GetAllAsync(subjectId);
            var removedTokens = tokens.Where(x => string.Equals(x.ClientId, clientId, StringComparison.OrdinalIgnoreCase));
            foreach (var token in removedTokens)
            {
                await _cache.DeleteAsync($"idp_persited_grant_{token.Key}");
            }
            await base.RemoveAllAsync(subjectId, clientId);
        }

        public override async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            _logger.LogInformation($"{nameof(CachePersistedGrantStore)} {nameof(RemoveAllAsync)} with subjectId {subjectId}, clientId {clientId}, type {type}");
            var tokens = await base.GetAllAsync(subjectId);
            var removedTokens = tokens.Where(x => string.Equals(x.ClientId, clientId, StringComparison.OrdinalIgnoreCase) && x.Type == type);
            foreach (var token in removedTokens)
            {
                await _cache.DeleteAsync($"idp_persited_grant_{token.Key}");
            }
            await base.RemoveAllAsync(subjectId, clientId, type);
        }
    }
}
