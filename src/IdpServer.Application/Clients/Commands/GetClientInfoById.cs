using System;
using IdpServer.Application.Client.Model;
using MediatR;

namespace IdpServer.Application.Client.Command
{
    public class GetClientInfoById : IRequest<ClientShortInfoDto>
    {
        public bool ForceUpdate { get; }
        public Guid ClientId { get; set; }
        public bool AllowHeader { get; set; }
        public Guid CorrelationId { get; set; }

        public GetClientInfoById(Guid clientId, bool forceUpdate = false, bool allowHeader = false)
        {
            ClientId = clientId;
            ForceUpdate = forceUpdate;
            AllowHeader = allowHeader;
        }
    }
}