using MediatR;
using IdpServer.Application.BrokerClient.Model;

namespace IdpServer.Application.BrokerClient.Command
{
    public class FetchBrokerClientByClientId : IRequest<BrokerClientDto>
    {
        public string Id { get; set; }

        public FetchBrokerClientByClientId(string id)
        {
            Id = id;
        }
    }
}
