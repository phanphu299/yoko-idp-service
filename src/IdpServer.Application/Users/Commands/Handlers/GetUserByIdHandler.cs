using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Model;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.User.Command.Handler
{
    public class GetUserByIdHandler : IRequestHandler<GetUserById, UserDto>
    {
        private readonly IUserService _service;
        public GetUserByIdHandler(IUserService service)
        {
            _service = service;
        }
        public async Task<UserDto> Handle(GetUserById request, CancellationToken cancellationToken)
        {
            var userDto = await _service.FindByIdAsync(request.Id, request.IgnoreQueryFilters);
            if (userDto != null)
            {
                userDto.Password = null;
            }
            return userDto;
        }
    }
}
