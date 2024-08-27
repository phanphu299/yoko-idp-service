using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.SharedKernel.Model;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.User.Command.Handler
{
    public class RestoreUserRequestHandler : IRequestHandler<RestoreUser, BaseResponse>
    {
        private readonly IUserService _service;

        public RestoreUserRequestHandler(IUserService service)
        {
            _service = service;
        }

        public async Task<BaseResponse> Handle(RestoreUser request, CancellationToken cancellationToken)
        {
            await _service.RestoreUserAsync(request.Id);
            return BaseResponse.Success;
        }
    }
}