using System.ComponentModel.DataAnnotations;
using System.Linq;
using AHI.Infrastructure.Exception;
using IdpServer.Localization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class CustomRequiredAttribute : RequiredAttribute, IClientModelValidator
{
    public void AddValidation(ClientModelValidationContext context)
    {
        if (context == null)
        {
            throw new EntityInvalidException(nameof(context));
        }

        //get DisplayName if any, otherwise use PropertyName
        var properyName = context.ModelMetadata.ContainerMetadata
                            .ModelType.GetProperty(context.ModelMetadata.PropertyName)
                            .GetCustomAttributes(typeof(DisplayAttribute), false)
                            .Cast<DisplayAttribute>()
                            .FirstOrDefault()?.Name ?? context.ModelMetadata.PropertyName;

        AttributeUtils.MergeAttribute(context.Attributes, "data-val", "true");
        AttributeUtils.MergeAttribute(context.Attributes, "data-val-required", $"{properyName} is required.");
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var result = ValidationResult.Success;
        if (!IsValid(value))
        {
            var memberNames = validationContext.MemberName != null ? new string[] { validationContext.MemberName } : null;
            var localizer = validationContext.GetService(typeof(TranslationService)) as TranslationService;
            var displayName = localizer.Translate(validationContext.DisplayName);
            ErrorMessage = localizer.Translate("ERROR.VALIDATION.FIELD_REQUIRED");

            result = new ValidationResult(this.FormatErrorMessage(displayName), memberNames);
        }

        return result;
    }
}
