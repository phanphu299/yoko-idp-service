using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AHI.Infrastructure.Cache.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.SharedKernel.Model;
using IdpServer.Application.Constant;
using IdpServer.Application.Model;
using IdpServer.Application.Service.Abstraction;

namespace IdpServer.Application.Service
{
    public class MasterService : IMasterService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICache _cache;
        public MasterService(
                            IHttpClientFactory httpClientFactory,
                            ICache cache)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }

        public async Task<IEnumerable<User.Model.DateTimeFormatDto>> GetAllDateTimeFormatAsync()
        {
            var masterClient = _httpClientFactory.CreateClient(HttpClients.MASTER_SERVICE);
            var response = await masterClient.GetAsync($"mst/formats/datetimes");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsByteArrayAsync();
            var responseData = content.Deserialize<BaseSearchResponse<User.Model.DateTimeFormatDto>>();
            return responseData.Data;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync(bool migrated = true, bool deleted = false)
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClients.MASTER_FUNCTION);
            var response = await httpClient.GetAsync($"fnc/mst/projects?migrated={migrated}&deleted={deleted}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsByteArrayAsync();
            return content.Deserialize<IEnumerable<ProjectDto>>();
        }

        public async Task<IEnumerable<SubscriptionDto>> GetAllSubscriptionsAsync(bool migrated = true, bool deleted = false)
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClients.MASTER_FUNCTION);
            var response = await httpClient.GetAsync($"fnc/mst/subscriptions?includeUser=false&migrated={migrated}&deleted={deleted}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsByteArrayAsync();
            return content.Deserialize<IEnumerable<SubscriptionDto>>();
        }

        public async Task<IEnumerable<TenantDto>> GetAllTenantsAsync(bool migrated = true, bool deleted = false)
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClients.MASTER_FUNCTION);
            var response = await httpClient.GetAsync($"fnc/mst/tenants?migrated={migrated}&deleted={deleted}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsByteArrayAsync();
            return content.Deserialize<IEnumerable<TenantDto>>();
        }
    }
}
