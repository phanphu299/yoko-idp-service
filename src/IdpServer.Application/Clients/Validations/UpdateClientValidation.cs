using AHI.Infrastructure.Exception;
using FluentValidation;
using IdpServer.Application.Constant;
using IdpServer.Application.Client.Command;
using IdpServer.Application.Client.Model;
using IdpServer.Application.Service.Abstraction;

namespace IdpServer.Application.Handler
{
    public class UpdateClientValidation : AbstractValidator<UpdateClient>
    {
        public UpdateClientValidation(IConfigurationService configurationService)
        {
            RuleFor(x => x.ClientName)
                .NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED)
                .MaximumLength(200).WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_MAX_LENGTH)
                .SetValidator(new MatchRegex(RemoteValidationKeys.name, null, configurationService)).When(x => x.GrantType != GrantTypes.AUTHORIZATION_CODE);
            RuleForEach(x => x.Privileges).SetValidator(
               new InlineValidator<ApplicationProjectClientOverrideDto> {
                    agValidator => agValidator.RuleFor(x => x.EntityCode)
                                              .Must(x => x.ToLowerInvariant() != Privileges.Project.ENTITY_NAME ).WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_INVALID)
               }
           );
        }
    }
}
