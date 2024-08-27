using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.Service.Tag.Extension;
using AHI.Infrastructure.Service.Tag.Model;
using IdpServer.Domain.Entity;

namespace IdpServer.Application.Client.Model
{
    public class ArchiveClientDto : TagDtos
    {
        public string ClientId { get; set; }
        public string Name { get; set; }
        public IEnumerable<ApplicationProjectClientOverrideDto> Privileges { get; set; }
        public IEnumerable<ApplicationObjectOverrideDto> ObjectPrivileges { get; set; }
        public IEnumerable<string> GrantTypes { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        private static Func<Domain.Entity.Client, ArchiveClientDto> DtoConverter = DtoProjection.Compile();
        private static Func<ArchiveClientDto, string, ITenantContext, Domain.Entity.Client> EntityConverter = EntityProjection.Compile();
        public static Expression<Func<Domain.Entity.Client, ArchiveClientDto>> DtoProjection
        {
            get
            {
                return entity => new ArchiveClientDto
                {
                    ClientId = entity.ClientId,
                    Name = entity.ClientName,
                    CreatedUtc = entity.Created,
                    UpdatedUtc = entity.Updated,
                    GrantTypes = entity.ClientGrantTypes.Select(x => x.GrantType),
                    Tags = entity.EntityTags.MappingTagDto()
                };
            }
        }

        public static Expression<Func<ArchiveClientDto, string, ITenantContext, Domain.Entity.Client>> EntityProjection
        {
            get
            {
                return (clientDto, clientSecret, tenantContext) => new Domain.Entity.Client
                {
                    ClientId = clientDto.ClientId,
                    ClientName = clientDto.Name,
                    Created = DateTime.UtcNow,
                    Enabled = true,
                    ProtocolType = "oidc",
                    RequireClientSecret = true,
                    Description = $"{clientDto.Name} description",
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
                    NonEditable = true,
                    ClientSecrets = new List<ClientSecret>()
                    {
                        new ClientSecret(clientDto.Name, clientSecret, clientSecret.HashAsSha256String())
                    },
                    ClientGrantTypes = clientDto.GrantTypes != null && clientDto.GrantTypes.Any() ? clientDto.GrantTypes.Select(grantType => new ClientGrantType
                    {
                        GrantType = grantType
                    }).ToList()
                    : new List<ClientGrantType>(),
                    ClientClaims = new List<ClientClaim>()
                    {
                        new ClientClaim
                        {
                            Type = "tenantId",
                            Value = tenantContext.TenantId
                        },
                        new ClientClaim
                        {
                            Type = "subscriptionId",
                            Value = tenantContext.SubscriptionId
                        },
                        new ClientClaim
                        {
                            Type = "projectId",
                            Value = tenantContext.ProjectId
                        }
                    },
                    ProjectClient = new ProjectClient
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantContext.TenantId,
                        SubscriptionId = tenantContext.SubscriptionId,
                        ProjectId = tenantContext.ProjectId,
                    }
                };
            }
        }

        public static ArchiveClientDto Create(Domain.Entity.Client entity)
        {
            if (entity == null)
                return null;
            return DtoConverter(entity);
        }

        public static Domain.Entity.Client Create(ArchiveClientDto clientDto, string clientSecret, ITenantContext tenantContext)
        {
            if (clientDto == null)
                return null;
            return EntityConverter(clientDto, clientSecret, tenantContext);
        }
    }
}
