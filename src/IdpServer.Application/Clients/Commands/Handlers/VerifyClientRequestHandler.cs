using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Model;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.Client.Command.Handler
{
    public class VerifyClientRequestHandler : IRequestHandler<VerifyClient, BaseResponse>
    {
        private readonly IClientService _service;
        public VerifyClientRequestHandler(IClientService service)
        {
            _service = service;
        }

        public Task<BaseResponse> Handle(VerifyClient request, CancellationToken cancellationToken)
        {
            return _service.VerifyArchiveDataAsync(request, cancellationToken);
        }
    }
}
