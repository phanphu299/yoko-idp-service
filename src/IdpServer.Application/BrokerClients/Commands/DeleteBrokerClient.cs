using System.Collections.Generic;
using IdpServer.Application.SharedKernel;
using MediatR;

namespace IdpServer.Application.BrokerClient.Command
{
    public class DeleteBrokerClient : IRequest<BaseResponse>
    {
        public IEnumerable<string> Ids { get; set; }
        public DeleteBrokerClient()
        {
            Ids = new List<string>();
        }
    }
}
