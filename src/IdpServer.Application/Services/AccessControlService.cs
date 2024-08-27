using System;
using System.Net.Http;
using System.Threading.Tasks;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using IdpServer.Application.Constant;
using IdpServer.Application.Model;
using IdpServer.Application.Service.Abstraction;

namespace IdpServer.Application.Service
{
    public class AccessControlService : IAccessControlService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITenantContext _tenantContext;
        private readonly ILoggerAdapter<AccessControlService> _logger;

        public AccessControlService(
                            ITenantContext tenantContext,
                            IHttpClientFactory httpClientFactory,
                            ILoggerAdapter<AccessControlService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _tenantContext = tenantContext;
            _logger = logger;
        }

        public async Task<string> GetApplicationNameAsync(string applicationId, Guid correlationId)
        {
            try
            {
                if (string.IsNullOrEmpty(applicationId))
                {
                    return string.Empty;
                }
                var httpClient = _httpClientFactory.CreateClient(HttpClients.ACCESS_CONTROL_SERVICE, _tenantContext);
                var response = await httpClient.GetAsync($"acm/applications/{applicationId}?correlationId={correlationId}&isGetSimpleDto=true&excludeUserContext=true");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsByteArrayAsync();
                var application = content.Deserialize<ApplicationDto>();
                return application.Name;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetApplicationNameAsync");
                return string.Empty;
            }
        }
    }
}