using AHI.Infrastructure.SharedKernel.Model;
using IdpServer.Application.User.Model;
using MediatR;

namespace IdpServer.Application.User.Command
{
    public class SearchLoginType : BaseCriteria, IRequest<BaseSearchResponse<LoginTypeDto>>
    {
        public SearchLoginType()
        {
            PageSize = 20;
            PageIndex = 0;
        }
    }
}