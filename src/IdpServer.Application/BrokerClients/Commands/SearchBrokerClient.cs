using AHI.Infrastructure.SharedKernel.Model;
using MediatR;
using IdpServer.Application.BrokerClient.Model;

namespace IdpServer.Application.BrokerClient.Command
{
    public class SearchBrokerClient : BaseCriteria, IRequest<BaseSearchResponse<BrokerClientDto>>
    {   
        public SearchBrokerClient()
        {
            PageSize = 20;
            PageIndex = 0;
        }
    }
}
