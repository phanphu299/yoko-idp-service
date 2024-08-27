using IdpServer.Application.BrokerClient.Model;
using MediatR;

namespace IdpServer.Application.BrokerClient.Command
{
    public class GetBrokerClientById : IRequest<BrokerClientDto>
    {
        public string Id { get; set; }

        public GetBrokerClientById(string id)
        {
            Id = id;
        }
    }
}
