$(document).ready(function () {
    const keys = [
      'ERROR.VALIDATION.FIELD_REQUIRED',
      'COMMON.FIELD.PASSWORD',
    ]
    getTranslations(keys, function (data) {
      const requiredError = data?.['ERROR.VALIDATION.FIELD_REQUIRED'] ?? '{0} is required.';
      const passwordRequired = requiredError.replace('{0}', data?.['COMMON.FIELD.PASSWORD'] ?? 'Password');

      var validationOptions = {
        rules: {
          Password: {
            required: true,
          }
        },
        messages: {
          Password: passwordRequired
        }
      };

      initFormValidation(validationOptions);
    });
  });
