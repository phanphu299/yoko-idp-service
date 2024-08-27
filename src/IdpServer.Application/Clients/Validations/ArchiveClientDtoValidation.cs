using AHI.Infrastructure.Exception;
using FluentValidation;
using IdpServer.Application.Client.Model;

namespace IdpServer.Application.Handler
{
    public class ArchiveClientDtoValidation : AbstractValidator<ArchiveClientDto>
    {
        public ArchiveClientDtoValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);

            RuleFor(x => x.ClientId)
                .NotEmpty()
                .WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);

            RuleForEach(x => x.Privileges)
                .ChildRules(privilege =>
                {
                    privilege.RuleFor(x => x.EntityCode).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
                    privilege.RuleFor(x => x.PrivilegeCode).NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
                    privilege.RuleFor(x => x.ApplicationId).NotNull().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED);
                });
        }
    }
}
