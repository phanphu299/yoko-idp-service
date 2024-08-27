using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Service.Abstraction;
using MediatR;
using IdpServer.Application.BrokerClient.Model;

namespace IdpServer.Application.BrokerClient.Command.Handler
{
    public class GetBrokerClientHandler : IRequestHandler<GetBrokerClientById, BrokerClientDto>
    {
        private readonly IBrokerClientService _service;
        public GetBrokerClientHandler(IBrokerClientService service)
        {
            _service = service;
        }

        public Task<BrokerClientDto> Handle(GetBrokerClientById request, CancellationToken cancellationToken)
        {
            return _service.GetByIdAsync(request.Id, cancellationToken);
        }
    }
}
