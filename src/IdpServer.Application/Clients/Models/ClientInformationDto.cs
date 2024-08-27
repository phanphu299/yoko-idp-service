using System;
using System.Collections.Generic;
using AHI.Infrastructure.Service.Tag.Model;

namespace IdpServer.Application.Client.Model
{
    public class ClientInformationDto : TagDtos
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public Guid ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public string ProjectId { get; set; }
        public IEnumerable<ClientRedirectUrisDto> ClientRedirectUris { get; set; }
        public IEnumerable<ClientPostLogoutRedirectUrisDto> ClientPostLogoutRedirectUris { get; set; }
        public IEnumerable<ApplicationProjectClientOverrideDto> Privileges { get; set; }
        public ClientInformationDto()
        {
            ClientRedirectUris = new List<ClientRedirectUrisDto>();
            ClientPostLogoutRedirectUris = new List<ClientPostLogoutRedirectUrisDto>();
            Privileges = new List<ApplicationProjectClientOverrideDto>();
        }
    }
}
