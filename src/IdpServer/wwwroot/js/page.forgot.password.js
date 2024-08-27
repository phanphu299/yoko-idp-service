$(document).ready(function () {
    const keys = [
        'ERROR.VALIDATION.FIELD_REQUIRED',
        'COMMON.FIELD.EMAIL'
    ]
    getTranslations(keys, function (data) {
        const emailInvalid = data?.['ERROR.VALIDATION.INVALID_EMAIL'] ?? 'Invalid email';
        const requiredError = data?.['ERROR.VALIDATION.FIELD_REQUIRED'] ?? '{0} is required.';
        const emailRequired = requiredError.replace('{0}', data?.['COMMON.FIELD.EMAIL'] ?? 'Email');

        var validationOptions = {
            rules: {
                Username: {
                    required: true,
                    email: true
                }
            },
            messages: {
                Username: {
                    required: emailRequired,
                    email: emailInvalid
                }
            }
        };

        initFormValidation(validationOptions);
    });
});
