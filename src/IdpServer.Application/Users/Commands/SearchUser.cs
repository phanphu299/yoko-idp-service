using AHI.Infrastructure.SharedKernel.Model;
using IdpServer.Application.Model;
using MediatR;

namespace IdpServer.Application.User.Command
{
    public class SearchUser :  BaseCriteria, IRequest<BaseSearchResponse<UserDto>>
    {
        public bool Deleted { get; set; }
        public SearchUser()
        {
            PageSize = 20;
            PageIndex = 0;
        }
    }
}