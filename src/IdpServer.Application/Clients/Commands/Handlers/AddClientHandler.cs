using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Client.Model;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.Client.Command.Handler
{
    public class AddClientHandler : IRequestHandler<AddClient, ClientDto>
    {
        private readonly IClientService _service;
        public AddClientHandler(IClientService service)
        {
            _service = service;
        }

        public Task<ClientDto> Handle(AddClient request, CancellationToken cancellationToken)
        {
            return _service.AddClientAsync(request, cancellationToken);
        }
    }
}
