$(document).ready(function () {
    const keys = [
        'ERROR.VALIDATION.FIELD_REQUIRED',
        'ERROR.VALIDATION.PASSWORDS_NOT_MATCH',
        'COMMON.FIELD.PASSWORD',
        'PAGE.SET_PASSWORD.FIELD.CONFIRM_PASSWORD'
    ];
    getTranslations(keys, function (data) {
        const requiredError = data?.['ERROR.VALIDATION.FIELD_REQUIRED'] ?? '{0} is required.';
        const passwordRequired = requiredError.replace('{0}', data?.['COMMON.FIELD.PASSWORD'] ?? 'Password');
        const confirmPasswordRequired = requiredError.replace('{0}', data?.['PAGE.SET_PASSWORD.FIELD.CONFIRM_PASSWORD'] ?? 'Confirm Password');
        const confirmPasswordNotMatch = data?.['ERROR.VALIDATION.PASSWORDS_NOT_MATCH'] ?? 'Confirm Password does not match with Password.';

        var validationOptions = {
            rules: {
                NewPassword: {
                    required: true,
                    validatePasswordPolicy: true
                },
                ConfirmNewPassword: {
                    required: true,
                    equalTo: '#user_password'
                }
            },
            messages: {
                NewPassword: {
                    required: passwordRequired,
                    validatePasswordPolicy: 'Password invalid'
                },
                ConfirmNewPassword: {
                    required: confirmPasswordRequired,
                    equalTo: confirmPasswordNotMatch
                }
            }
        };

        initFormValidation(validationOptions);
    });
});
