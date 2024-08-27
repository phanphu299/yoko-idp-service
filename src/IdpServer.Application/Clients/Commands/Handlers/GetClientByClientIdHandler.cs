using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Client.Model;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.Client.Command.Handler
{
    public class GetClientByClientIdHandler : IRequestHandler<GetClientById, ClientInformationDto>
    {
        private readonly IClientService _service;
        public GetClientByClientIdHandler(IClientService service)
        {
            _service = service;
        }
        public Task<ClientInformationDto> Handle(GetClientById request, CancellationToken cancellationToken)
        {
            return _service.GetClientByClientIdAsync(request, cancellationToken);
        }
    }
}
