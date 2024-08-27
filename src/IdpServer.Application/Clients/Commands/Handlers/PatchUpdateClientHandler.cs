using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.Client.Command.Handler
{
    public class PatchUpdateClientHandler : IRequestHandler<PartialUpdateClient, bool>
    {
        private readonly IClientService _clientService;
        public PatchUpdateClientHandler(IClientService clientService)
        {
            _clientService = clientService;
        }
        public async Task<bool> Handle(PartialUpdateClient request, CancellationToken cancellationToken)
        {
            await _clientService.PartialUpdateAsync(request, cancellationToken);
            return true;
        }
    }
}
