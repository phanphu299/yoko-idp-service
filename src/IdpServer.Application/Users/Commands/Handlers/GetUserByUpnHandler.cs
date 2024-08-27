using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Model;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.User.Command.Handler
{
    public class GetUserByUpnHandler : IRequestHandler<GetUserByUpn, UserDto>
    {
        private readonly IUserService _service;
        public GetUserByUpnHandler(IUserService service)
        {
            _service = service;
        }
        public Task<UserDto> Handle(GetUserByUpn request, CancellationToken cancellationToken)
        {
            return _service.FindByUpnAsync(request.Upn, request.ignoreQueryFilters);
        }
    }
}
