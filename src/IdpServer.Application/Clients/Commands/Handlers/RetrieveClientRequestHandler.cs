using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Model;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.Client.Command.Handler
{
    public class RetrieveClientRequestHandler : IRequestHandler<RetrieveClient, BaseResponse>
    {
        private readonly IClientService _service;
        public RetrieveClientRequestHandler(IClientService service)
        {
            _service = service;
        }

        public Task<BaseResponse> Handle(RetrieveClient request, CancellationToken cancellationToken)
        {
            return _service.RetrieveAsync(request, cancellationToken);
        }
    }
}
