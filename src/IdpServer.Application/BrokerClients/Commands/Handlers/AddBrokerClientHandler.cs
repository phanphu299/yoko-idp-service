using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Service.Abstraction;
using MediatR;
using IdpServer.Application.BrokerClient.Model;

namespace IdpServer.Application.BrokerClient.Command.Handler
{
    public class AddBrokerClientHandler : IRequestHandler<AddBrokerClient, BrokerClientDto>
    {
        private readonly IBrokerClientService _service;
        public AddBrokerClientHandler(IBrokerClientService service)
        {
            _service = service;
        }

        public Task<BrokerClientDto> Handle(AddBrokerClient request, CancellationToken cancellationToken)
        {
            return _service.AddBrokerClientAsync(request, cancellationToken);
        }
    }
}
