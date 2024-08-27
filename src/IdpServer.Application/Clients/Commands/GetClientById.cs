using System;
using IdpServer.Application.Client.Model;
using MediatR;

namespace IdpServer.Application.Client.Command
{
    public class GetClientById : IRequest<ClientInformationDto>
    {
        public Guid ClientId { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public GetClientById(Guid clientId)
        {
            ClientId = clientId;
        }
    }
}
