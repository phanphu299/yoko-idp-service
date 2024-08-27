using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.User.Command.Handler
{
    public class RemoveUserByIdHandler : IRequestHandler<RemoveUserById, bool>
    {
        private readonly IUserService _service;
        public RemoveUserByIdHandler(IUserService service)
        {
            _service = service;
        }
        public Task<bool> Handle(RemoveUserById request, CancellationToken cancellationToken)
        {
            return _service.RemoveUserAsync(request.Id);
        }
    }
}
