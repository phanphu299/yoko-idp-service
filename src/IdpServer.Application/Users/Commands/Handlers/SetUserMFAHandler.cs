using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Service.Abstraction;
using IdpServer.Application.SharedKernel;
using MediatR;

namespace IdpServer.Application.User.Command
{
    public class SetUserMFAHandler : IRequestHandler<SetUserMFA, BaseResponse>
    {
        private readonly IUserService _userService;
        public SetUserMFAHandler(IUserService userService)
        {
            _userService = userService;
        }
        public async Task<BaseResponse> Handle(SetUserMFA request, CancellationToken cancellationToken)
        {
            await _userService.UpdateMFAAsync(request.UserId, request.MFA, false);
            return BaseResponse.Success;
        }
    }
}
