using System.Collections.Generic;
using IdpServer.Application.Constant;
using IdpServer.Application.Client.Model;
using MediatR;
using System;
using AHI.Infrastructure.Service.Tag.Model;

namespace IdpServer.Application.Client.Command
{
    public class AddClient : UpsertTagCommand, IRequest<ClientDto>
    {
        //[Required]
        //[DynamicValidation(RemoteValidationKeys.name)]
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public int? AccessTokenLifeTime { get; set; }
        public IEnumerable<ApplicationProjectClientOverrideDto> Privileges { get; set; }
        public string ApplicationUrl { get; set; }
        public IEnumerable<string> RedirectUris { get; set; }
        public string GrantType { get; set; } = GrantTypes.CLIENT_CREDENTIALS;
        public AddClient()
        {
            ClientId = Guid.NewGuid();
            Privileges = new List<ApplicationProjectClientOverrideDto>();
            RedirectUris = new List<string>();
        }
    }
}
