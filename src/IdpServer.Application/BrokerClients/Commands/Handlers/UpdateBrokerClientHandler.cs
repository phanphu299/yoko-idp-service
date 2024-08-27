using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Service.Abstraction;
using MediatR;
using IdpServer.Application.BrokerClient.Model;

namespace IdpServer.Application.BrokerClient.Command.Handler
{
    public class UpdateBrokerClientHandler : IRequestHandler<UpdateBrokerClient, BrokerClientDto>
    {
        private readonly IBrokerClientService _service;
        public UpdateBrokerClientHandler(IBrokerClientService service)
        {
            _service = service;
        }

        public Task<BrokerClientDto> Handle(UpdateBrokerClient request, CancellationToken cancellationToken)
        {
            return _service.UpdateBrokerClientAsync(request, cancellationToken);
        }
    }
}
