using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Model;
using IdpServer.Application.Model;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.User.Command.Handler
{
    public class SearchUserRequestHandler : IRequestHandler<SearchUser, BaseSearchResponse<UserDto>>
    {
        private readonly IUserService _service;

        public SearchUserRequestHandler(IUserService service)
        {
            _service = service;
        }

        public async Task<BaseSearchResponse<UserDto>> Handle(SearchUser request, CancellationToken cancellationToken)
        {
            return await _service.SearchAsync(request);
        }
    }
}
