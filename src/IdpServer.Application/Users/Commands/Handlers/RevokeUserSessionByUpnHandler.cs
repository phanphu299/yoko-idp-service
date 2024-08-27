using System;
using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.User.Command.Handler
{
    public class RevokeUserSessionByUpnHandler : IRequestHandler<RevokeUserSessionByUpn, bool>
    {
        private readonly IUserService _service;
        public RevokeUserSessionByUpnHandler(IUserService service)
        {
            _service = service;
        }
        public async Task<bool> Handle(RevokeUserSessionByUpn request, CancellationToken cancellationToken)
        {
            var user = await _service.FindByUpnAsync(request.Upn);
            if (user != null)
            {
                return await _service.RevokeUserSessionAsync(request.Upn, user.TenantId, user.SubscriptionId, Guid.NewGuid());
            }
            return false;
        }
    }
}
