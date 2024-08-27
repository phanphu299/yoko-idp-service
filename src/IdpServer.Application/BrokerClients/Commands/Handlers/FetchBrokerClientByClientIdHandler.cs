using System.Threading;
using System.Threading.Tasks;
using MediatR;
using IdpServer.Application.BrokerClient.Command;
using IdpServer.Application.BrokerClient.Model;
using IdpServer.Application.Service.Abstraction;

namespace IdpServer.Application.BrokerClient.Handler.Command.Handler
{
    public class FetchBrokerClientByClientIdHandler : IRequestHandler<FetchBrokerClientByClientId, BrokerClientDto>
    {
        private readonly IBrokerClientService _service;
        public FetchBrokerClientByClientIdHandler(IBrokerClientService service)
        {
            _service = service;
        }

        public Task<BrokerClientDto> Handle(FetchBrokerClientByClientId request, CancellationToken cancellationToken)
        {
            return _service.FetchByIdAsync(request.Id);
        }
    }
}
