using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Model;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.User.Command.Handler
{
    public class UpdateUserInfoHandler : IRequestHandler<UpdateUserInfo, UserDto>
    {
        private readonly IUserService _service;
        public UpdateUserInfoHandler(IUserService service)
        {
            _service = service;
        }
        public Task<UserDto> Handle(UpdateUserInfo request, CancellationToken cancellationToken)
        {
            return _service.UpdateUserBasicInfoAsync(request);
        }
    }
}
