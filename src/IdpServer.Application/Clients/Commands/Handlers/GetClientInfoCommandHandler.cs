using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.Cache.Abstraction;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.MultiTenancy.Internal;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using IdpServer.Application.Client.Model;
using IdpServer.Application.Constant;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Application.Service.Abstraction;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace IdpServer.Application.Client.Command.Handler
{
    public class GetClientInfoCommandHandler : IRequestHandler<GetClientInfoById, ClientShortInfoDto>
    {
        private readonly ILoggerAdapter<GetClientInfoCommandHandler> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IClientRoleRepository _clientRoleRepository;
        private readonly ICache _cache;
        private readonly IMasterService _masterService;

        public GetClientInfoCommandHandler(
            ILoggerAdapter<GetClientInfoCommandHandler> logger,
            IHttpClientFactory httpClientFactory,
            IClientRoleRepository clientRoleRepository,
            IConfiguration configuration,
            ICache cache,
            IMasterService masterService)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _clientRoleRepository = clientRoleRepository;
            _cache = cache;
            _masterService = masterService;
        }

        public async Task<ClientShortInfoDto> Handle(GetClientInfoById request, CancellationToken cancellationToken)
        {
            if (request.CorrelationId == Guid.Empty)
            {
                request.CorrelationId = Guid.NewGuid();
            }
            var key = string.Format(CacheKey.CLIENT, request.ClientId.ToString().ToUpperInvariant());
            _logger.LogInformation($"{request.CorrelationId} GetClientInfo - Step 1: Start");
            if (request.ForceUpdate)
            {
                _logger.LogInformation($"{request.CorrelationId} GetClientInfo - Step 1.1: _cache.DeleteAllKeysAsync(wildCardKey)");
                await _cache.DeleteHashByKeysAsync(key, new List<string>(){ CacheKey.HASH_FIELD_ROLES, CacheKey.HASH_FIELD_INFO });
            }

            _logger.LogInformation($"{request.CorrelationId} GetClientInfo - Step 2: _cache.GetStringAsync(key)");
            
            var clientInfo = await _cache.GetHashByKeyAsync<ClientShortInfoDto>(key, CacheKey.HASH_FIELD_INFO);
            if (clientInfo == null)
            {
                clientInfo = new ClientShortInfoDto();
                clientInfo.ClientId = request.ClientId;
                var rights = new List<string>();
                var objectRights = new List<string>();

                _logger.LogInformation($"{request.CorrelationId} GetClientInfo - Step 4: _clientRoleRepository.GetByClientIdAsync(request.ClientId)");
                var clientRoles = await _cache.GetHashByKeyAsync<IEnumerable<Domain.Entity.ClientRole>>(key, CacheKey.HASH_FIELD_ROLES);
                if (clientRoles == null)
                {
                    clientRoles = await _clientRoleRepository.GetByClientIdAsync(request.ClientId);
                    await _cache.SetHashByKeyAsync(key, CacheKey.HASH_FIELD_ROLES, clientRoles);
                    _logger.LogInformation($"{request.CorrelationId} GetClientInfo - Step 4.1: _cache.SetHashByKeyAsync");
                }
                if (clientRoles.Any())
                {
                    _logger.LogInformation($"{request.CorrelationId} GetClientInfo - Step 5: GetRoleDetailAsync");
                    var roleTasks = clientRoles.Select(r => GetRoleDetailAsync(r.ApplicationId, r.RoleId, r.TenantId, r.SubscriptionId, request.ClientId, request.CorrelationId));
                    var completedRoles = await Task.WhenAll(roleTasks);
                    var roles = completedRoles.SelectMany(x => x);
                    foreach (var clientRole in clientRoles)
                    {
                        var tenantRoles = roles.Where(x => x.Id == clientRole.RoleId);
                        rights.AddRange(tenantRoles.SelectMany(role => role.Entities.SelectMany(e =>
                        {
                            return e.Privileges.Select(p =>
                            {
                                if (clientRole.ProjectId == null)
                                {
                                    return $"t/{clientRole.TenantId?.ToString() ?? "*"}/s/{clientRole.SubscriptionId?.ToString() ?? "*"}/a/{clientRole.ApplicationId}/p/*/e/{e.Code}/o/*/p/{p.Code}".ToLowerInvariant();
                                }
                                return $"t/{clientRole.TenantId?.ToString() ?? "*"}/s/{clientRole.SubscriptionId?.ToString() ?? "*"}/a/{clientRole.ApplicationId}/p/{clientRole.ProjectId}/e/{e.Code}/o/*/p/{p.Code}".ToLowerInvariant();
                            });
                        })));

                        // Client create by user
                        rights.AddRange(tenantRoles.SelectMany(x => x.RoleOverrides));
                        objectRights.AddRange(tenantRoles.SelectMany(x => x.ObjectOverrides));
                    }
                    clientInfo.ObjectRightShorts = objectRights.Distinct();
                    clientInfo.RightShorts = rights.Distinct();

                    _logger.LogInformation($"{request.CorrelationId} GetClientInfo - Step 6: _cache.SetHashByKeyAsync");
                    await _cache.SetHashByKeyAsync(key, CacheKey.HASH_FIELD_INFO, clientInfo);
                }
            }

            _logger.LogInformation($"{request.CorrelationId} GetClientInfo - Step 8: End");
            return clientInfo;
        }

        private async Task<RoleDto> GetRoleDetailAsync(Guid applicationId, Guid roleId, ITenantContext tenantContext, Guid clientId, Guid correlationId)
        {
            _logger.LogInformation($"{correlationId} GetClientInfo - GetRoleDetailAsync - Step 5.2: httpClient.GetAsync(acm/roles/");
            var httpClient = _httpClientFactory.CreateClient(HttpClients.ACCESS_CONTROL_SERVICE, tenantContext);
            var responseBody = await httpClient.GetAsync($"acm/roles/{roleId}?applicationId={applicationId}&excludeUserContext=true&includeFullAccess=true&clientId={clientId}");
            responseBody.EnsureSuccessStatusCode();
            var payload = await responseBody.Content.ReadAsByteArrayAsync();
            var dto = payload.Deserialize<RoleDto>();
            dto.TenantId = tenantContext.TenantId;
            dto.SubscriptionId = tenantContext.SubscriptionId;
            return dto;
        }

        private async Task<IEnumerable<RoleDto>> GetRoleDetailAsync(Guid applicationId, Guid roleId, string tenantId, string subscriptionId, Guid clientId, Guid correlationId)
        {
            _logger.LogInformation($"{correlationId} GetClientInfo - GetRoleDetailAsync - Step 5.1: GetRoleDetailAsync");
            var subscriptions = await _masterService.GetAllSubscriptionsAsync();
            var list = new List<RoleDto>();
            if (tenantId != null && subscriptionId != null)
            {
                var subscriptionDto = subscriptions.First(x => x.Id == subscriptionId);
                var tenantContext = new TenantContext();
                tenantContext.SetTenantId(subscriptionDto.TenantResourceId);
                tenantContext.SetSubscriptionId(subscriptionDto.Id);
                var roleDto = await GetRoleDetailAsync(applicationId, roleId, tenantContext, clientId, correlationId);
                list.Add(roleDto);
            }
            else if (tenantId != null && subscriptionId == null)
            {
                var roleTasks = subscriptions.Where(x => x.TenantResourceId == tenantId).Select(subscriptionDto =>
                {
                    var tenantContext = new TenantContext();
                    tenantContext.RetrieveFromString(subscriptionDto.TenantResourceId, subscriptionDto.Id);
                    return tenantContext;
                }).Select(x => GetRoleDetailAsync(applicationId, roleId, x, clientId, correlationId));
                var roles = await Task.WhenAll(roleTasks);
                list.AddRange(roles.Where(x => x != null));
            }
            else if (tenantId == null && subscriptionId == null)
            {
                // assign to entire system
                var roleTasks = subscriptions.Select(subscriptionDto =>
                {
                    var tenantContext = new TenantContext();
                    tenantContext.RetrieveFromString(subscriptionDto.TenantResourceId, subscriptionDto.Id);
                    return tenantContext;
                }).Select(x => GetRoleDetailAsync(applicationId, roleId, x, clientId, correlationId));
                var roles = await Task.WhenAll(roleTasks);
                list.AddRange(roles.Where(x => x != null));
            }

            _logger.LogInformation($"{correlationId} GetClientInfo - GetRoleDetailAsync - Step 5.3: End GetRoleDetailAsync");
            return list;
        }
    }
}
