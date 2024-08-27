using FluentValidation.Validators;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using IdpServer.Application.Service.Abstraction;

public class MatchRegex : AsyncValidatorBase
{
    private readonly IConfigurationService _configurationService;
    private readonly string _regexKey;
    private readonly string _descriptionKey;
    private readonly bool _acceptNullEmpty;

    public MatchRegex(string regexKey, string descriptionKey, IConfigurationService configurationService, bool acceptNullEmpty = false) : base("{Message}")
    {
        _configurationService = configurationService;
        _regexKey = regexKey;
        _descriptionKey = descriptionKey;
        _acceptNullEmpty = acceptNullEmpty;
    }

    protected async override Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation)
    {
        var value = context.PropertyValue as string;
        if (_acceptNullEmpty && string.IsNullOrEmpty(value))
            return true;

        if (string.IsNullOrEmpty(value))
        {
            return false;
        }
        try
        {
            var regexString = await _configurationService.GetValueAsync(_regexKey, null);
            var result = Regex.IsMatch(value, regexString, RegexOptions.IgnoreCase);
            
            if (!result)
            {
                var errorMessage = await _configurationService.GetValueAsync(_descriptionKey, null);
            }

            return result;
        }
        catch
        {
            // for cant load regex from system
            throw new System.InvalidOperationException("Failed to load validation data.");
        }
    }
}
