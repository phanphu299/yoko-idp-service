using AHI.Infrastructure.SharedKernel.Model;
using MediatR;
using IdpServer.Application.Client.Model;

namespace IdpServer.Application.Client.Command
{
    public class SearchClient : BaseCriteria, IRequest<BaseSearchResponse<ClientDto>>
    {
        public SearchClient()
        {
            Sorts = "created=desc;clientName=asc";
            PageSize = 20;
            PageIndex = 0;
        }
    }
}
