using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Model;
using AHI.Infrastructure.SystemContext.Enum;

using Function.Constant;
using Function.Model;
using Function.Service.Abstraction;

using Newtonsoft.Json;

namespace Function.Service
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;
        private readonly ITenantContext _tenantContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILoggerAdapter<ConfigurationService> _logger;

        public ConfigurationService(IConfiguration configuration,
                            IHttpClientFactory httpClientFactory,
                            ILoggerAdapter<ConfigurationService> logger,
                            ITenantContext tenantContext)
        {
            _configuration = configuration;
            _tenantContext = tenantContext;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
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

        private async Task<IEnumerable<SystemConfigDto>> GetSystemConfigsAsync(string key)
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClients.CONFIGURATION_SERVICE, _tenantContext);
            httpClient.DefaultRequestHeaders.Add("x-app-level", nameof(AppLevel.SUBSCRIPTION));
            var response = await httpClient.GetAsync($"cnm/configs?key={key}");
            response.EnsureSuccessStatusCode();
            var payload = await response.Content.ReadAsStringAsync();
            var body = JsonConvert.DeserializeObject<BaseSearchResponse<SystemConfigDto>>(payload);
            return body.Data;
        }
    }
}
