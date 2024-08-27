using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.Exception;
using AHI.Infrastructure.SharedKernel.Model;
using IdpServer.Application.Constant;
using IdpServer.Application.Service.Abstraction;
using MediatR;

namespace IdpServer.Application.User.Command.Handler
{
    public class ChangePasswordRequestHandler : IRequestHandler<ChangePassword, BaseResponse>
    {
        private readonly IUserService _service;

        public ChangePasswordRequestHandler(IUserService service)
        {
            _service = service;
        }

        public async Task<BaseResponse> Handle(ChangePassword request, CancellationToken cancellationToken)
        {
            var usr = await _service.FindByIdAsync(request.UserId);
            var result = await _service.ChangePasswordAsync(usr.Upn, usr.LoginTypeCode, request.NewPassword, request.CurrentPassword, sendEmail: true);
            if (!result.Result)
            {
                var ex = result.Exception ?? new GenericProcessFailedException(MessageConstants.OPERATION_INVALID);
                throw ex;
            }
            return BaseResponse.Success;
        }
    }
}