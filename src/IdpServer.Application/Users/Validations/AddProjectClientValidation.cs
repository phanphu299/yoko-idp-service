using AHI.Infrastructure.Exception;
using FluentValidation;
using IdpServer.Application.User.Command;

namespace IdpServer.Application.Handler
{
    public class CreateUserValidation : AbstractValidator<CreateUser>
    {
        public CreateUserValidation()
        {
            RuleFor(x => x.Upn).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
        }
    }
}
