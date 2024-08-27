using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Service.Abstraction;
using IdpServer.Application.SharedKernel;
using MediatR;

namespace IdpServer.Application.Client.Command.Handler
{
    public class DeleteClientHandler : IRequestHandler<DeleteClient, BaseResponse>
    {
        private readonly IClientService _service;
        public DeleteClientHandler(IClientService service)
        {
            _service = service;
        }

        public Task<BaseResponse> Handle(DeleteClient request, CancellationToken cancellationToken)
        {
            return _service.DeleteClientAsync(request, cancellationToken);
        }
    }
}
