using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.Audit.Service.Abstraction;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using AHI.Infrastructure.Bus.ServiceBus.Enum;
using AHI.Infrastructure.Cache.Abstraction;
using AHI.Infrastructure.Exception;
using AHI.Infrastructure.Exception.Helper;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.MultiTenancy.Internal;
using AHI.Infrastructure.Service;
using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.SystemContext.Enum;
using IdpServer.Application.Client.Command;
using IdpServer.Application.Client.Model;
using IdpServer.Application.Constant;
using IdpServer.Application.Event;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Application.Service.Abstraction;
using IdpServer.Application.SharedKernel;
using IdpServer.Domain.Entity;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FluentValidation;
using AHI.Infrastructure.UserContext.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.Service.Tag.Service.Abstraction;
using AHI.Infrastructure.Service.Tag.Extension;
using AHI.Infrastructure.Service.Tag.Model;

namespace IdpServer.Application.Service
{
    public class ClientService : BaseSearchService<Domain.Entity.Client, int, SearchClient, ClientDto>, IClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly IProjectClientRepository _projectClientRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IClientRoleRepository _clientRoleRepository;
        private readonly IIdpUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;
        private readonly ICache _cache;
        private readonly string _podName;
        private readonly ITenantContext _tenantContext;
        private readonly IValidator<ArchiveClientDto> _clientVerifyValidator;
        private readonly IUserContext _userContext;
        private readonly ILoggerAdapter<ClientService> _logger;
        private readonly ITagService _tagService;

        //private readonly IMemoryCache _memoryCache;
        public ClientService(
            IHttpClientFactory httpClientFactory
            , IServiceProvider serviceProvider
            , IDomainEventDispatcher dispatcher
            , IProjectClientRepository projectClientRepository
            , IClientRepository clientRepository
            , IClientRoleRepository clientRoleRepository
            , IIdpUnitOfWork unitOfWork
            , IAuditLogService auditLogService
            , ICache cache
            , ITenantContext tenantContext
            , IValidator<ArchiveClientDto> clientVerifyValidator
            , IUserContext userContext
            , ILoggerAdapter<ClientService> logger
            , IConfiguration configuration,
            ITagService tagService)
            : base(ClientDto.Create, serviceProvider)
        {
            _httpClientFactory = httpClientFactory;
            _dispatcher = dispatcher;
            _projectClientRepository = projectClientRepository;
            _clientRepository = clientRepository;
            _clientRoleRepository = clientRoleRepository;
            _unitOfWork = unitOfWork;
            _cache = cache;
            _tenantContext = tenantContext;
            _clientVerifyValidator = clientVerifyValidator;
            _userContext = userContext;
            _logger = logger;

            _auditLogService = auditLogService;
            _auditLogService.SetAppLevel(AppLevel.PROJECT);
            _podName = configuration["PodName"] ?? "identity-service";
            _tagService = tagService;
        }

        protected override Type GetDbType()
        {
            return typeof(IClientRepository);
        }

        public async Task<ClientDto> FetchByClientIdAsync(string clientId)
        {
            var entity = await _clientRepository.AsFetchable().Where(x => x.ClientId == clientId).FirstOrDefaultAsync();
            return ClientDto.Create(entity);
        }

        public async Task<ClientDto> AddClientAsync(AddClient model, CancellationToken token)
        {
            Task<long[]> upsertTagsTask = null;
            model.ApplicationId = Guid.Parse(ApplicationInformation.ASSET_MANAGEMENT_APPLICATION_ID);
            model.Upn = _userContext.Upn;
            if (model.Tags != null && model.Tags.Any())
            {
                upsertTagsTask = _tagService.UpsertTagsAsync(model);
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                model.ClientName = model.ClientName.Trim();
                var client = await _clientRepository.FindByClientNameAsync(model.ClientName);
                if (client != null && await ExistsClientAsync(client))
                    throw EntityValidationExceptionHelper.GenerateException(nameof(model.ClientName), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_DUPLICATED);
                var clientId = model.ClientId;
                var clientSecret = GenerateClientSecret(30);

                var entity = CreateClientModel(model, clientSecret, clientId);
                if (model.GrantType == GrantTypes.CLIENT_CREDENTIALS)
                {
                    entity = await _clientRepository.AddAsync(entity);
                    await AddClientRoleAsync(clientId);
                    await PatchScopeAsync(clientId, model.Privileges);
                }
                else if (model.GrantType == GrantTypes.AUTHORIZATION_CODE)
                {
                    clientSecret = null;
                    entity = CreateClientModel(model, clientSecret, clientId);
                }
                entity = await _unitOfWork.Clients.AddWithSaveChangeAsync(entity);
                var jsonSetting = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                var tagIds = Array.Empty<long>();
                if (upsertTagsTask != null)
                    tagIds = await upsertTagsTask;

                var addedTags = Array.Empty<EntityTagDb>();
                if (tagIds.Any())
                {
                    addedTags = tagIds.Distinct().Select(x => new EntityTagDb
                    {
                        EntityType = EntityTagConstants.CLIENT_ENTITY_NAME,
                        EntityIdInt = entity.Id,
                        TagId = x
                    }).ToArray();
                    await _unitOfWork.EntityTags.AddRangeWithSaveChangeAsync(addedTags);
                }

                await _unitOfWork.CommitAsync();
                await _auditLogService.SendLogAsync(ActivityEntityAction.CLIENT, ActionType.Add, ActionStatus.Success, clientId, entity.ClientName, payload: model);

                entity.EntityTags = addedTags;
                var result = ClientDto.Create(entity);
                result.ClientSecret = clientSecret;
                return await _tagService.FetchTagsAsync(result);
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                await _auditLogService.SendLogAsync(ActivityEntityAction.CLIENT, ActionType.Add, ActionStatus.Fail, payload: model);
                throw;
            }
        }

        private async Task<bool> ExistsClientAsync(Domain.Entity.Client client)
        {
            var projectClient = await _projectClientRepository.FindByClientIdAsync(client.Id);
            if (projectClient == null)
                return false;
            if (projectClient.ProjectId == _tenantContext.ProjectId)
                return true;

            return false;
        }

        private async Task<bool> PatchScopeAsync(Guid clientId, IEnumerable<ApplicationProjectClientOverrideDto> privileges)
        {
            foreach (var item in privileges)
            {
                // item.ApplicationId = // TODO _tenantContext.TenantId;
                item.ProjectId = _tenantContext.ProjectId;
                item.ClientId = clientId;
            }

            var patchOject = new List<object>() {
                new {
                    Op = "assign/project-client",
                    Path = $"/{clientId}",
                    Value = new {
                        Items = privileges
                    }
                }
            };
            var json = JsonConvert.SerializeObject(patchOject);
            var httpClient = _httpClientFactory.CreateClient(HttpClients.ACCESS_CONTROL_SERVICE, _tenantContext);
            var result = await httpClient
                .PatchAsync("acm/clients/override", new StringContent(json, Encoding.UTF8, "application/json"))
                .ContinueWith(t => t.Status == TaskStatus.RanToCompletion);
            return result;
        }

        private async Task AddApplicationObjectOverrideAsync(IEnumerable<ArchiveClientDto> clients, IDictionary<Guid, Guid> addedIds)
        {
            var groupList = clients.SelectMany(x => x.ObjectPrivileges)
                                   .GroupBy(x => x.ObjectId)
                                   .Select(x => new
                                   {
                                       ObjectId = x.Key,
                                       Privileges = x.Select(m =>
                                                               {
                                                                   m.ProjectId = _tenantContext.ProjectId;
                                                                   m.TargetId = addedIds.ContainsKey(m.TargetId) ? addedIds[m.TargetId] : m.TargetId;
                                                                   return m;
                                                               })
                                   });
            var httpClient = _httpClientFactory.CreateClient(HttpClients.ACCESS_CONTROL_SERVICE, _tenantContext);
            foreach (var item in groupList)
            {
                var patchOject = new List<object>() {
                new {
                        Op = "assign/project",
                        Path = $"/",
                        Value = new {
                            Items = item.Privileges
                        }
                    }
                };
                var json = JsonConvert.SerializeObject(patchOject);
                var result = await httpClient
                    .PatchAsync("acm/objects/override", new StringContent(json, Encoding.UTF8, "application/json"))
                    .ContinueWith(t => t.Status == TaskStatus.RanToCompletion);
            }
        }

        private Domain.Entity.Client CreateClientModel(AddClient model, string clientSecret, Guid clientId)
        {
            var client = new Domain.Entity.Client
            {
                ClientId = clientId.ToString(),
                ClientName = model.ClientName,
                Created = DateTime.UtcNow,
                Enabled = true,
                ProtocolType = "oidc",
                RequireClientSecret = true,
                Description = $"{model.ClientName} description",
                ClientUri = null,
                LogoUri = null,
                RequireConsent = false,
                AllowRememberConsent = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                RequirePkce = true,
                AllowPlainTextPkce = false,
                AllowAccessTokensViaBrowser = true,
                FrontChannelLogoutUri = null,
                FrontChannelLogoutSessionRequired = false,
                BackChannelLogoutUri = null,
                BackChannelLogoutSessionRequired = false,
                AllowOfflineAccess = true,
                IdentityTokenLifetime = 3600,
                AccessTokenLifetime = 3600,
                AuthorizationCodeLifetime = 3600,
                ConsentLifetime = null,
                AbsoluteRefreshTokenLifetime = 3600,
                SlidingRefreshTokenLifetime = 3600,
                RefreshTokenUsage = 2592000,
                UpdateAccessTokenClaimsOnRefresh = false,
                RefreshTokenExpiration = 2592000,
                AccessTokenType = 1,
                EnableLocalLogin = true,
                IncludeJwtId = true,
                AlwaysSendClientClaims = true,
                ClientClaimsPrefix = " ",
                PairWiseSubjectSalt = null,
                Updated = DateTime.UtcNow,
                LastAccessed = null,
                UserSsoLifetime = null,
                UserCodeType = null,
                DeviceCodeLifetime = 3600,
                NonEditable = true
            };
            if (model.GrantType == GrantTypes.CLIENT_CREDENTIALS)
            {
                client.ClientSecrets = new List<ClientSecret>()
                {
                    new ClientSecret(model.ClientName, clientSecret, clientSecret.HashAsSha256String())
                };
                client.ClientGrantTypes = new List<ClientGrantType>()
                {
                    new ClientGrantType()
                    {
                        GrantType = GrantTypes.CLIENT_CREDENTIALS
                    }
                };
                client.ClientClaims = new List<ClientClaim>()
                {
                    new ClientClaim
                    {
                        Type = "tenantId",
                        Value = _tenantContext.TenantId
                    },
                    new ClientClaim
                    {
                        Type = "subscriptionId",
                        Value = _tenantContext.SubscriptionId
                    },
                    new ClientClaim
                    {
                        Type = "projectId",
                        Value = _tenantContext.ProjectId
                    },
                    new ClientClaim
                    {
                        Type = "apiClient",
                        Value = "true"
                    }
                };
                client.ProjectClient = new ProjectClient
                {
                    Id = Guid.NewGuid(),
                    TenantId = _tenantContext.TenantId,
                    SubscriptionId = _tenantContext.SubscriptionId,
                    ProjectId = _tenantContext.ProjectId,
                };
                client.ClientScopes = GetScopes(client, model.Privileges);
            }
            else if (model.GrantType == GrantTypes.AUTHORIZATION_CODE)
            {
                client.Description = $"{model.ClientName} web app description";
                client.RequireClientSecret = false;
                client.ClientGrantTypes = new List<ClientGrantType>()
                {
                    new ClientGrantType()
                    {
                        GrantType = GrantTypes.AUTHORIZATION_CODE
                    }
                };
                client.ClientScopes = new List<ClientScope>()
                {
                    new ClientScope()
                    {
                        Scope = ScopeData.OPENID
                    },
                    new ClientScope()
                    {
                        Scope = ScopeData.PROFILE
                    },
                    new ClientScope()
                    {
                        Scope = ScopeData.OFFLINE_ACCESS
                    }
                };
                client.ClientRedirectUris = model.RedirectUris.Select(x => new ClientRedirectUris
                {
                    RedirectUri = x
                }).ToArray();
                client.ClientPostLogoutRedirectUris = new List<ClientPostLogoutRedirectUris>()
                {
                    new ClientPostLogoutRedirectUris
                    {
                        PostLogoutRedirectUri = model.ApplicationUrl
                    },
                    new ClientPostLogoutRedirectUris
                    {
                        PostLogoutRedirectUri = $"{model.ApplicationUrl}/signout-callback-oidc"
                    }
                };
            }
            return client;
        }

        private Task AddClientRoleAsync(Guid clientId)
        {
            var clientRole = new ClientRole()
            {
                Id = Guid.NewGuid(),
                ClientId = clientId,
                ProjectId = _tenantContext.ProjectId,
                SubscriptionId = _tenantContext.SubscriptionId,
                TenantId = _tenantContext.TenantId,
                RoleId = Guid.Parse(ApplicationInformation.ASSET_MANAGEMENT_DATA_VIEWER_ID),
                ApplicationId = Guid.Parse(ApplicationInformation.ASSET_MANAGEMENT_APPLICATION_ID),
            };
            return _clientRoleRepository.AddAsync(clientRole);
        }

        public async Task<BaseResponse> UpdateClientAsync(UpdateClient model, CancellationToken token)
        {
            Task<long[]> upsertTagsTask = null;
            model.ApplicationId = Guid.Parse(ApplicationInformation.ASSET_MANAGEMENT_APPLICATION_ID);
            model.Upn = _userContext.Upn;
            if (model.Tags != null && model.Tags.Any())
            {
                upsertTagsTask = _tagService.UpsertTagsAsync(model);
            }

            var client = await _clientRepository.FindByClientIdAsync(model.ClientId);
            if (client == null)
                throw new EntityNotFoundException(MessageConstants.ENTITY_NOT_FOUND);

            var clientName = client.ClientName;
            var duplicatedClient = await _clientRepository.FindByClientNameAsync(model.ClientName);
            if (duplicatedClient != null && await ExistsClientAsync(duplicatedClient) && client.Id != duplicatedClient.Id)
                throw EntityValidationExceptionHelper.GenerateException(nameof(model.ClientName), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_DUPLICATED);


            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (model.RedirectUris.Any() && !string.IsNullOrWhiteSpace(model.ApplicationUrl))
                {
                    await _unitOfWork.Clients.UpdateClientRedirectUrisAsync(client.Id, model.RedirectUris);
                    await _unitOfWork.Clients.UpdateClientPostLogoutRedirectUrisAsync(client.Id, new List<string> { $"{model.ApplicationUrl}/signout-callback-oidc" });
                }

                var result = true;
                client.ClientName = model.ClientName;
                client.Updated = DateTime.UtcNow;
                if (model.GrantType == GrantTypes.CLIENT_CREDENTIALS)
                {
                    client.ClientScopes = GetScopes(client, model.Privileges);
                    await _unitOfWork.Clients.UpdateAsync(client.Id, client);
                    result = await PatchScopeAsync(Guid.Parse(client.ClientId), model.Privileges);
                }
                else if (model.GrantType == GrantTypes.AUTHORIZATION_CODE)
                {
                    await _unitOfWork.Clients.UpdateAsync(client.Id, client);
                }

                if (!result)
                {
                    await _unitOfWork.RollbackAsync();
                    await _auditLogService.SendLogAsync(ActivityEntityAction.CLIENT, ActionType.Update, ActionStatus.Fail, client.ClientId, client.ClientName, model);
                    return BaseResponse.Failed;
                }

                var isSameTag = model.IsSameTags(client.EntityTags);
                if (!isSameTag)
                {
                    await _unitOfWork.EntityTags.RemoveByEntityIdAsync(EntityTagConstants.CLIENT_ENTITY_NAME, client.Id, isTracking: true);

                    var tagIds = Array.Empty<long>();
                    if (upsertTagsTask != null)
                        tagIds = await upsertTagsTask;

                    if (tagIds.Any())
                    {
                        var entitiesTags = tagIds.Distinct().Select(x => new EntityTagDb
                        {
                            EntityType = EntityTagConstants.CLIENT_ENTITY_NAME,
                            EntityIdInt = client.Id,
                            TagId = x
                        }).ToArray();
                        await _unitOfWork.EntityTags.AddRangeWithSaveChangeAsync(entitiesTags);
                    }
                }

                await _unitOfWork.CommitAsync();
                await _auditLogService.SendLogAsync(ActivityEntityAction.CLIENT, ActionType.Update, ActionStatus.Success, client.ClientId, client.ClientName, model);

                var key = $"*{model.ClientId}*";
                await _cache.DeleteAllKeysAsync(key);
                return BaseResponse.Success;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                await _auditLogService.SendLogAsync(ActivityEntityAction.CLIENT, ActionType.Update, ActionStatus.Fail, model.ClientId, clientName, payload: model);
                throw;
            }
        }

        public async Task<BaseResponse> DeleteClientAsync(DeleteClient command, CancellationToken token)
        {
            await _unitOfWork.BeginTransactionAsync();

            var clientIds = command.ClientIds.Distinct();
            if (clientIds == null || clientIds.Any() == false)
            {
                return BaseResponse.Failed;
            }
            var clients = await _clientRepository.AsQueryable().Where(x => clientIds.Select(x => x.ToString()).Contains(x.ClientId)).ToListAsync();

            try
            {
                var result = await ProcessRemoveAsync(clientIds);

                await _unitOfWork.CommitAsync();
                await _auditLogService.SendLogAsync(ActivityEntityAction.CLIENT, ActionType.Delete, ActionStatus.Success, clientIds, clients.Select(x => x.ClientName), command);

                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                await _auditLogService.SendLogAsync(ActivityEntityAction.CLIENT, ActionType.Delete, ActionStatus.Fail, clientIds, clients.Select(x => x.ClientName), command);
                throw;
            }
        }

        private async Task<BaseResponse> ProcessRemoveAsync(IEnumerable<Guid> clientIds)
        {
            foreach (var id in clientIds)
            {
                var client = await _clientRepository.FindByClientIdAsync(id);
                if (client == null)
                    throw new EntityNotFoundException(detailCode: ExceptionErrorCode.DetailCode.ERROR_ENTITY_NOT_FOUND_SOME_ITEMS_DELETED);
                var isAuthorizationCode = client.ClientGrantTypes.Any(x => x.GrantType == GrantTypes.AUTHORIZATION_CODE);

                var projectClient = await _projectClientRepository.FindByClientIdAsync(client.Id);
                if (projectClient == null && !isAuthorizationCode)
                    throw new EntityNotFoundException(detailCode: ExceptionErrorCode.DetailCode.ERROR_ENTITY_NOT_FOUND_SOME_ITEMS_DELETED);

                var clientRoles = (await _clientRoleRepository.GetByClientIdAsync(id)).ToList();
                foreach (var clientRole in clientRoles)
                {
                    await _clientRoleRepository.RemoveAsync(clientRole.Id);
                }

                await _clientRepository.RemoveClientPostLogoutRedirectUrisAsync(client.Id);
                await _clientRepository.RemoveClientRedirectUrisAsync(client.Id);
                await _clientRepository.RemoveClientAsync(client.Id);

                if (!isAuthorizationCode)
                    await _projectClientRepository.RemoveAsync(projectClient.Id);

                var key = $"*{id}*";
                await _cache.DeleteAllKeysAsync(key);

                var acmHttpClient = _httpClientFactory.CreateClient(HttpClients.ACCESS_CONTROL_SERVICE, _tenantContext);
                await acmHttpClient.DeleteAsync($"acm/clients/{client.ClientId}/remove");

                var projectClientDelete = ClientChangedEvent.CreateFrom(client, _tenantContext, ActionTypeEnum.Deleted);
                await _dispatcher.SendAsync(projectClientDelete);
            }
            return BaseResponse.Success;
        }

        private static Guid GenerateClientId()
        {
            return Guid.NewGuid();
        }

        private static string GenerateClientSecret(int passwordLength)
        {
            char[] passwordArray = new char[passwordLength];
            char[] upperCase = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            char[] lowerCase = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            char[] numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] special = { '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '<', '>', '?' };
            var random = new Random();
            for (int i = 0, k; i < passwordLength; i++)
            {
                var charType = random.Next(5);

                switch (charType)
                {
                    case 0:
                        k = random.Next(0, upperCase.Length);
                        passwordArray[i] = upperCase[k];
                        break;

                    case 1:
                        k = random.Next(0, lowerCase.Length);
                        passwordArray[i] = lowerCase[k];
                        break;

                    case 2:
                        k = random.Next(0, numbers.Length);
                        passwordArray[i] = numbers[k];
                        break;

                    default:
                        k = random.Next(0, special.Length);
                        passwordArray[i] = special[k];
                        break;
                }
            }
            return string.Join("", passwordArray);
        }

        public async Task<ClientInformationDto> GetClientByClientIdAsync(GetClientById command, CancellationToken token)
        {
            var client = await _clientRepository.FindByClientIdAsync(command.ClientId);
            if (client == null)
            {
                return null;
            }

            var response = new ClientInformationDto()
            {
                Id = client.Id,
                ClientName = client.ClientName,
                ClientId = Guid.Parse(client.ClientId),
                Tags = client.EntityTags.MappingTagDto()
            };
            await _tagService.FetchTagsAsync(response);

            var isAuthorizationCode = client.ClientGrantTypes.Any(x => x.GrantType == GrantTypes.AUTHORIZATION_CODE);
            if (isAuthorizationCode)
            {
                var clientPosts = client.ClientPostLogoutRedirectUris.Select(x => ClientPostLogoutRedirectUrisDto.Create(x));
                var clientUris = client.ClientRedirectUris.Select(x => ClientRedirectUrisDto.Create(x));
                response.ClientPostLogoutRedirectUris = clientPosts;
                response.ClientRedirectUris = clientUris;
                return response;
            }
            var projectClientInfo = await _projectClientRepository.FindByClientIdAsync(client.Id);
            var tenantContext = new TenantContext();
            tenantContext.RetrieveFromString(command.TenantId ?? projectClientInfo.TenantId, command.SubscriptionId ?? projectClientInfo.SubscriptionId);
            var httpClient = _httpClientFactory.CreateClient(HttpClients.ACCESS_CONTROL_SERVICE, tenantContext);
            var projectResponse = await httpClient.GetByteArrayAsync($"acm/clients/{client.ClientId}");
            var priviliges = projectResponse.Deserialize<IEnumerable<ApplicationProjectClientOverrideDto>>();
            var lastValidSecret = client.ClientSecrets.Where(x => x.Expiration == null || x.Expiration >= DateTime.UtcNow).OrderBy(x => x.Created).LastOrDefault();
            response.Privileges = priviliges;
            response.ClientSecret = lastValidSecret?.ObfuscatedSecret;
            response.ProjectId = projectClientInfo?.ProjectId;
            response.SubscriptionId = projectClientInfo?.SubscriptionId;
            response.TenantId = projectClientInfo?.TenantId;
            return response;
        }

        public async Task<SecretClientDto> GenerateClientSecretAsync(GenerateClientSecret command, CancellationToken token)
        {
            string clientName = null;
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var client = await _clientRepository.FindByClientIdAsync(command.ClientId);
                if (client == null)
                    throw new EntityNotFoundException(MessageConstants.ENTITY_NOT_FOUND);

                clientName = client.ClientName;
                var clientSecret = GenerateClientSecret(30);

                client.Updated = DateTime.UtcNow;

                // revoke api secret
                var activeClientSecret = client.ClientSecrets.Where(x => x.Expiration == null || x.Expiration >= DateTime.UtcNow).OrderBy(x => x.Created).LastOrDefault();
                if (activeClientSecret != null)
                {
                    activeClientSecret.Expiration = DateTime.UtcNow.AddDays(-1);
                }


                //create new api secret
                client.ClientSecrets.Add(new ClientSecret(clientName, clientSecret, clientSecret.HashAsSha256String()));

                await _clientRepository.UpdateAsync(client.Id, client);
                await _unitOfWork.CommitAsync();
                await _cache.DeleteAllKeysAsync($"*{command.ClientId}*");
                await _auditLogService.SendLogAsync(ActivityEntityAction.CLIENT, ActionType.Update, ActionStatus.Success, client.ClientId, client.ClientName, client);
                var result = SecretClientDto.Create(client);
                result.ClientSecretRaw = clientSecret;
                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                await _auditLogService.SendLogAsync(ActivityEntityAction.CLIENT, ActionType.Update, ActionStatus.Fail, command.ClientId, clientName, command);
                throw;
            }
        }

        public Task<BaseResponse> CheckExistClientsAsync(CheckExistClient command, CancellationToken cancellationToken)
        {
            return ValidateExistClientsAsync(command, cancellationToken);
        }

        private async Task<BaseResponse> ValidateExistClientsAsync(CheckExistClient command, CancellationToken cancellationToken)
        {
            var requestIds = new HashSet<string>(command.Ids.Distinct());
            var clients = new HashSet<string>(await _clientRepository.AsFetchable().AsNoTracking().Where(x => requestIds.Contains(x.ClientId)).Select(x => x.ClientId).ToListAsync());
            if (!requestIds.SetEquals(clients))
                throw new EntityNotFoundException(detailCode: ExceptionErrorCode.DetailCode.ERROR_ENTITY_NOT_FOUND_SOME_ITEMS_DELETED);
            return BaseResponse.Success;
        }

        public async Task PartialUpdateAsync(PartialUpdateClient command, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var operation in command.JsonPatchDocument.Operations)
                {
                    switch (operation.op)
                    {
                        case "redirect/uris":
                            await UpdateClientRedirectAsync(operation);
                            break;

                        default:
                            throw new EntityInvalidException(MessageConstants.OPERATION_INVALID);
                    }
                }
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<ArchiveClientDto>> ArchiveAsync(ArchiveClient command, CancellationToken token)
        {
            var clients = await _clientRepository.AsQueryable()
                                            .AsNoTracking()
                                            .Include(x => x.ClientGrantTypes)
                                            .Where(x => x.Created <= command.ArchiveTime)
                                            .Select(x => ArchiveClientDto.Create(x))
                                            .ToListAsync();

            var httpClient = _httpClientFactory.CreateClient(HttpClients.ACCESS_CONTROL_SERVICE, _tenantContext);
            foreach (var client in clients)
            {
                var privilegesResponse = await httpClient.GetByteArrayAsync($"acm/clients/{client.ClientId}");
                var priviliges = privilegesResponse.Deserialize<IEnumerable<ApplicationProjectClientOverrideDto>>();
                client.Privileges = priviliges;

                var objectPrivilegesResponse = await httpClient.GetByteArrayAsync($"acm/applications/{ApplicationInformation.ASSET_MANAGEMENT_APPLICATION_ID}/targets/{client.ClientId}?projectId={_tenantContext.ProjectId}&targetType=api_client");
                var objectPrivileges = objectPrivilegesResponse.Deserialize<IEnumerable<ApplicationObjectOverrideDto>>();
                client.ObjectPrivileges = objectPrivileges;
            }
            return clients;
        }

        public async Task<AHI.Infrastructure.SharedKernel.Model.BaseResponse> RetrieveAsync(RetrieveClient command, CancellationToken token)
        {
            var clients = JsonConvert.DeserializeObject<IEnumerable<ArchiveClientDto>>(command.Data, AHI.Infrastructure.SharedKernel.Extension.Constant.JsonSerializerSetting);
            _userContext.SetUpn(command.Upn);
            await _unitOfWork.BeginTransactionAsync();
            IDictionary<Guid, Guid> addedIds = new Dictionary<Guid, Guid>();
            try
            {
                var entities = new List<Domain.Entity.Client>();
                clients = clients.OrderBy(x => x.CreatedUtc);
                foreach (var client in clients)
                {
                    var clientId = GenerateClientId();
                    var clientSecret = GenerateClientSecret(30);
                    addedIds.TryAdd(Guid.Parse(client.ClientId), clientId);
                    client.ClientId = clientId.ToString();
                    var entity = ArchiveClientDto.Create(client, clientSecret, _tenantContext);
                    entity.ClientScopes = GetScopes(entity, client.Privileges);

                    await AddClientRoleAsync(clientId);
                    await PatchScopeAsync(clientId, client.Privileges);
                    entities.Add(entity);
                }
                await _clientRepository.RetrieveAsync(entities);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }

            await AddApplicationObjectOverrideAsync(clients, addedIds);
            return AHI.Infrastructure.SharedKernel.Model.BaseResponse.Success;
        }

        public async Task<AHI.Infrastructure.SharedKernel.Model.BaseResponse> VerifyArchiveDataAsync(VerifyClient command, CancellationToken token)
        {
            var clients = JsonConvert.DeserializeObject<IEnumerable<ArchiveClientDto>>(command.Data, AHI.Infrastructure.SharedKernel.Extension.Constant.JsonSerializerSetting);
            foreach (var client in clients)
            {
                var validation = await _clientVerifyValidator.ValidateAsync(client);
                if (!validation.IsValid)
                {
                    throw EntityValidationExceptionHelper.GenerateException(nameof(command.Data), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_INVALID);
                }

                if (client.Privileges != null &&
                    (client.Privileges.Any(x => x.ApplicationId == null || x.ApplicationId == Guid.Empty)))
                    throw EntityValidationExceptionHelper.GenerateException(nameof(command.Data), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_INVALID);
            }

            return BaseResponse.Success;
        }

        public override async Task<AHI.Infrastructure.SharedKernel.Model.BaseSearchResponse<ClientDto>> SearchAsync(SearchClient criteria)
        {
            criteria.MappingSearchTags();
            var response = await base.SearchAsync(criteria);
            return await _tagService.FetchTagsAsync(response);
        }

        private async Task UpdateClientRedirectAsync(Operation operation)
        {
            var request = JObject.FromObject(operation.value).ToObject<UpdateClient>();

            var client = await _clientRepository.FindByClientIdAsync(request.ClientId);
            if (client == null)
                throw new EntityNotFoundException(MessageConstants.ENTITY_NOT_FOUND);

            await _clientRepository.UpdateClientRedirectUrisAsync(client.Id, request.RedirectUris);
            await _clientRepository.UpdateClientPostLogoutRedirectUrisAsync(client.Id, new List<string> { $"{request.ApplicationUrl}/signout-callback-oidc" });
        }

        private IList<ClientScope> GetScopes(Domain.Entity.Client client, IEnumerable<ApplicationProjectClientOverrideDto> privileges)
        {
            var scopes = new List<ClientScope>
            {
                new ClientScope()
                {
                    ClientId = client.Id,
                    Scope = ScopeData.CLIENT_DATA
                }
            };

            if (privileges.Any(x => x.EntityCode == EntityCode.ASSET && x.IsSelected))
            {
                scopes.Add(new ClientScope()
                {
                    ClientId = client.Id,
                    Scope = ScopeData.ASSET_DATA
                });
            }

            if (privileges.Any(x => x.EntityCode == EntityCode.ASSET_MEDIA && x.IsSelected))
            {
                scopes.Add(new ClientScope()
                {
                    ClientId = client.Id,
                    Scope = ScopeData.STORAGE_DATA
                });

                scopes.Add(new ClientScope()
                {
                    ClientId = client.Id,
                    Scope = ScopeData.ASSET_MEDIA_DATA
                });
            }

            if (privileges.Any(x => x.EntityCode == EntityCode.ASSET_TABLE && x.IsSelected))
            {
                scopes.Add(new ClientScope()
                {
                    ClientId = client.Id,
                    Scope = ScopeData.ASSET_TABLE_DATA
                });
            }

            return scopes;
        }
    }
}
