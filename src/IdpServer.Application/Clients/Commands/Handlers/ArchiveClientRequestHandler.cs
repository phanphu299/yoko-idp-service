using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Client.Model;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.Client.Command.Handler
{
    public class ArchiveClientRequestHandler : IRequestHandler<ArchiveClient, IEnumerable<ArchiveClientDto>>
    {
        private readonly IClientService _service;
        public ArchiveClientRequestHandler(IClientService service)
        {
            _service = service;
        }

        public Task<IEnumerable<ArchiveClientDto>> Handle(ArchiveClient request, CancellationToken cancellationToken)
        {
            return _service.ArchiveAsync(request, cancellationToken);
        }
    }
}
