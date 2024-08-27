using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SharedKernel.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AHI.Infrastructure.MultiTenancy.Extension;
using IdpServer.Application.Constant;
using IdpServer.Application.Service.Abstraction;
using IdpServer.Application.Model;
using System.Collections.Generic;
using AHI.Infrastructure.SystemContext.Enum;

namespace IdpServer.Application.Service
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITenantContext _tenantContext;
        private readonly ILogger _logger;

        public ConfigurationService(IHttpClientFactory httpClientFactory, ITenantContext tenantContext, ILogger<ConfigurationService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _tenantContext = tenantContext;
            _logger = logger;
        }
        
        public async Task<string> GetValueAsync(string key, string defaultValue)
        {
            var result = defaultValue;
            try
            {
                var systemConfig = await GetSystemConfigsAsync(key);
                result = systemConfig.First().Value;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return result;

        }

        public async Task<IEnumerable<SystemConfigDto>> GetConfigsAsync(string keys)
        {
            var result = new List<SystemConfigDto>();
            try
            {
                result = (await GetSystemConfigsAsync(keys)).ToList();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
            return result;
        }

        private async Task<IEnumerable<SystemConfigDto>> GetSystemConfigsAsync(string keys)
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClients.CONFIGURATION_SERVICE, _tenantContext);
            httpClient.DefaultRequestHeaders.Add("x-app-level", nameof(AppLevel.SUBSCRIPTION));
            var response = await httpClient.GetAsync($"cnm/configs?key={keys}");
            response.EnsureSuccessStatusCode();
            var payload = await response.Content.ReadAsStringAsync();
            var body = JsonConvert.DeserializeObject<BaseSearchResponse<SystemConfigDto>>(payload);
            return body.Data;
        }
    }
}
