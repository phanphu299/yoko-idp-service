using AHI.Infrastructure.Exception;
using FluentValidation;
using IdpServer.Application.Client.Command;
using IdpServer.Application.Constant;
using IdpServer.Application.Service.Abstraction;

namespace IdpServer.Application.Handler
{
    public class AddClientValidation : AbstractValidator<AddClient>
    {
        public AddClientValidation(IConfigurationService configurationService)
        {
            RuleFor(x => x.ClientName)
                .NotEmpty().WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_REQUIRED)
                .MaximumLength(200).WithMessage(ExceptionErrorCode.DetailCode.ERROR_VALIDATION_MAX_LENGTH)
                .SetValidator(new MatchRegex(RemoteValidationKeys.name, null, configurationService)).When(x => x.GrantType != GrantTypes.AUTHORIZATION_CODE);
        }
    }
}
