using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Service.Abstraction;
using IdpServer.Application.SharedKernel;
using MediatR;

namespace IdpServer.Application.Client.Command.Handler
{
    public class UpdateClientHandler : IRequestHandler<UpdateClient, BaseResponse>
    {
        private readonly IClientService _service;
        public UpdateClientHandler(IClientService service)
        {
            _service = service;
        }

        public Task<BaseResponse> Handle(UpdateClient request, CancellationToken cancellationToken)
        {
            return _service.UpdateClientAsync(request, cancellationToken);
        }
    }
}
