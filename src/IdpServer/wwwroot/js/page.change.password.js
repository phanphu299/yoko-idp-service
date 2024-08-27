$(document).ready(function () {
    const keys = [
        'ERROR.VALIDATION.FIELD_REQUIRED',
        'ERROR.VALIDATION.PASSWORDS_NOT_MATCH',
        'PAGE.CHANGE_PASSWORD.FIELD.CURRENT_PASSWORD',
        'PAGE.CHANGE_PASSWORD.FIELD.NEW_PASSWORD',
        'PAGE.CHANGE_PASSWORD.FIELD.CONFIRM_NEW_PASSWORD'
    ]
    getTranslations(keys, function (data) {
        const requiredError = data?.['ERROR.VALIDATION.FIELD_REQUIRED'] ?? '{0} is required.';
        const oldPasswordRequired = requiredError.replace('{0}', data?.['PAGE.CHANGE_PASSWORD.FIELD.CURRENT_PASSWORD'] ?? 'Current Password');
        const newPasswordRequired = requiredError.replace('{0}', data?.['PAGE.CHANGE_PASSWORD.FIELD.NEW_PASSWORD'] ?? 'Password');
        const confirmPasswordRequired = requiredError.replace('{0}', data?.['PAGE.CHANGE_PASSWORD.FIELD.CONFIRM_NEW_PASSWORD'] ?? 'Confirm Password');
        const confirmPasswordNotMatch = data?.['ERROR.VALIDATION.PASSWORDS_NOT_MATCH'] ?? 'Confirm Password does not match with Password.';

        var validationOptions = {
            rules: {
                CurrentPassword: {
                    required: true,
                },
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
                CurrentPassword: oldPasswordRequired,
                NewPassword: {
                    required:newPasswordRequired,
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
