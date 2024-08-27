using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.User.Command.Handler
{
    public class RemoveUserBySubscriptionIdHandler : IRequestHandler<RemoveUserBySubscriptionId, bool>
    {
        private readonly IUserService _service;
        public RemoveUserBySubscriptionIdHandler(IUserService service)
        {
            _service = service;
        }
        public async Task<bool> Handle(RemoveUserBySubscriptionId request, CancellationToken cancellationToken)
        {
            await _service.RemoveUserBySubscriptionIdAsync(request.Id);
            return true;
        }
    }
}
