using System;
using System.Collections.Generic;
using IdpServer.Application.Constant;
using IdpServer.Application.Client.Model;
using IdpServer.Application.SharedKernel;
using MediatR;
using AHI.Infrastructure.Service.Tag.Model;

namespace IdpServer.Application.Client.Command
{
    public class UpdateClient : UpsertTagCommand, IRequest<BaseResponse>
    {
        public Guid ClientId { get; set; }
        //[Required]
        //[DynamicValidation(RemoteValidationKeys.name)]
        public string ClientName { get; set; }
        public string ApplicationUrl { get; set; }
        public string GrantType { get; set; } = GrantTypes.CLIENT_CREDENTIALS;
        public IEnumerable<string> RedirectUris { get; set; }
        public IEnumerable<ApplicationProjectClientOverrideDto> Privileges { get; set; }
        public UpdateClient()
        {
            Privileges = new List<ApplicationProjectClientOverrideDto>();
            RedirectUris = new List<string>();
        }
    }
}
