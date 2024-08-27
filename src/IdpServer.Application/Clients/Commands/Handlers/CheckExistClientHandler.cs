using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Service.Abstraction;
using MediatR;
using IdpServer.Application.SharedKernel;

namespace IdpServer.Application.Client.Command.Handler
{
    public class CheckExistClientHandler : IRequestHandler<CheckExistClient, BaseResponse>
    {
        private readonly IClientService _service;
        public CheckExistClientHandler(IClientService service)
        {
            _service = service;
        }

        public Task<BaseResponse> Handle(CheckExistClient request, CancellationToken cancellationToken)
        {
            return _service.CheckExistClientsAsync(request, cancellationToken);
        }
    }
}
