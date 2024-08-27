using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.Exception;
using IdpServer.Application.Constant;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Application.SharedKernel;
using MediatR;

namespace IdpServer.Application.User.Command
{
    public class SetUserDefaultPageRequestHandler : IRequestHandler<SetUserDefaultPage, BaseResponse>
    {
        private readonly IIdpUnitOfWork _unitOfWork;
        public SetUserDefaultPageRequestHandler(IIdpUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseResponse> Handle(SetUserDefaultPage request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.FindAsync(request.Upn);
            if (user == null)
            {
                throw new EntityNotFoundException(MessageConstants.ENTITY_NOT_FOUND);
            }
            if (user.UserTypeCode == UserTypes.LOCAL)
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    user.DefaultPage = request.DefaultPage;
                    await _unitOfWork.Users.UpdateAsync(user.Id, user);
                    await _unitOfWork.CommitAsync();
                }
                catch
                {
                    await _unitOfWork.RollbackAsync();
                    throw;
                }
            }
            return BaseResponse.Success;
        }
    }
}
