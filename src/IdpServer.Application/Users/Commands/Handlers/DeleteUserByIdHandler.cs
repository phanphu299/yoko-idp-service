using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.User.Command.Handler
{
    public class DeleteUserByIdHandler : IRequestHandler<DeleteUserById, bool>
    {
        private readonly IUserService _service;
        public DeleteUserByIdHandler(IUserService service)
        {
            _service = service;
        }
        public Task<bool> Handle(DeleteUserById request, CancellationToken cancellationToken)
        {
            return _service.DeleteUserAsync(request.Id);
        }
    }
}
