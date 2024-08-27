using AHI.Infrastructure.Exception;
using FluentValidation;
using IdpServer.Application.User.Command;

namespace IdpServer.Application.Handler
{
    public class ChangePasswordValidation : AbstractValidator<ChangePassword>
    {
        public ChangePasswordValidation()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
            RuleFor(x => x.NewPassword).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
        }
    }
}