using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AHI.Infrastructure.SharedKernel.Model;
using IdpServer.Application.Client.Model;
using IdpServer.Application.Service.Abstraction;
using IdpServer.Application.Client.Command;

namespace IdpServer.Application.Client.Handler.Command.Handler
{
    public class SearchClientRequestHandler : IRequestHandler<SearchClient, BaseSearchResponse<ClientDto>>
    {
        private readonly IClientService _service;
        public SearchClientRequestHandler(IClientService service)
        {
            _service = service;
        }

        public Task<BaseSearchResponse<ClientDto>> Handle(SearchClient request, CancellationToken cancellationToken)
        {
            return _service.SearchAsync(request);
        }
    }
}
