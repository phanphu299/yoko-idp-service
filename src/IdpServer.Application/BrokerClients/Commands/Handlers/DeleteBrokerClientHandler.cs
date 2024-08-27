using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Service.Abstraction;
using MediatR;
using IdpServer.Application.SharedKernel;

namespace IdpServer.Application.BrokerClient.Command.Handler
{
    public class DeleteBrokerClientHandler : IRequestHandler<DeleteBrokerClient, BaseResponse>
    {
        private readonly IBrokerClientService _service;
        public DeleteBrokerClientHandler(IBrokerClientService service)
        {
            _service = service;
        }

        public Task<BaseResponse> Handle(DeleteBrokerClient request, CancellationToken cancellationToken)
        {
            return _service.DeleteBrokerClientAsync(request, cancellationToken);
        }
    }
}
