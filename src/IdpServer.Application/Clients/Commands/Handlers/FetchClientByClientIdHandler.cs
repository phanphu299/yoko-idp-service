using System.Threading;
using System.Threading.Tasks;
using MediatR;
using IdpServer.Application.Client.Command;
using IdpServer.Application.Client.Model;
using IdpServer.Application.Service.Abstraction;

namespace IdpServer.Application.Client.Handler.Command.Handler
{
    public class FetchClientByClientIdHandler : IRequestHandler<FetchClientById, ClientDto>
    {
        private readonly IClientService _service;
        public FetchClientByClientIdHandler(IClientService service)
        {
            _service = service;
        }

        public Task<ClientDto> Handle(FetchClientById request, CancellationToken cancellationToken)
        {
            return _service.FetchByClientIdAsync(request.ClientId);
        }
    }
}
