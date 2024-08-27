using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.User.Command.Handler
{
    public class CreateUserHandler : IRequestHandler<CreateUser, System.Guid>
    {
        private readonly IUserService _service;
        public CreateUserHandler(IUserService service)
        {
            _service = service;
        }
        public Task<System.Guid> Handle(CreateUser request, CancellationToken cancellationToken)
        {
            return _service.CreateUserAsync(request);
        }
    }
}
