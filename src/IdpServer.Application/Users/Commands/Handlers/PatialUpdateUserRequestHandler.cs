using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.User.Command.Handler
{
    public class PatialUpdateUserRequestHandler : IRequestHandler<PartialUpdateUser, bool>
    {
        private readonly IUserService _service;

        public PatialUpdateUserRequestHandler(IUserService service)
        {
            _service = service;
        }

        public async Task<bool> Handle(PartialUpdateUser request, CancellationToken cancellationToken)
        {
            await _service.PartialUpdateAsync(request);
            return true;
        }
    }
}