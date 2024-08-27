using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AHI.Infrastructure.SharedKernel.Model;
using IdpServer.Application.BrokerClient.Command;
using IdpServer.Application.BrokerClient.Model;
using IdpServer.Application.Service.Abstraction;

namespace IdpServer.Application.BrokerClient.Handler.Command.Handler
{
    public class SearchBrokerClientRequestHandler : IRequestHandler<SearchBrokerClient, BaseSearchResponse<BrokerClientDto>>
    {
        private readonly IBrokerClientService _service;
        public SearchBrokerClientRequestHandler(IBrokerClientService service)
        {
            _service = service;
        }

        public Task<BaseSearchResponse<BrokerClientDto>> Handle(SearchBrokerClient request, CancellationToken cancellationToken)
        {
            return _service.SearchAsync(request);
        }
    }
}
