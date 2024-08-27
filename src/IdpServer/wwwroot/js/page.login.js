$(document).ready(function () {
    const keys = [
        'ERROR.VALIDATION.FIELD_REQUIRED',
        'ERROR.VALIDATION.INVALID_EMAIL',
        'COMMON.FIELD.EMAIL',
        'COMMON.FIELD.PASSWORD'
    ]
    getTranslations(keys, function (data) {
        const emailInvalid = data?.['ERROR.VALIDATION.INVALID_EMAIL'] ?? 'Invalid email';
        const requiredError = data?.['ERROR.VALIDATION.FIELD_REQUIRED'] ?? '{0} is required.';
        const emailRequired = requiredError.replace('{0}', data?.['COMMON.FIELD.EMAIL'] ?? 'Email');
        const passwordRequired = requiredError.replace('{0}', data?.['COMMON.FIELD.PASSWORD'] ?? 'Password');

        var validationOptions = {
            rules: {
                Username: {
                    required: true,
                    email: true
                },
                Password: {
                    required: true,
                }
            },
            messages: {
                Username: {
                    required: emailRequired,
                    email: emailInvalid
                },
                Password: passwordRequired
            }
        };

        initFormValidation(validationOptions);
    });
});
