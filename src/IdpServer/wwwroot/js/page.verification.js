let resendEmailMessage = 'A new email has been sent. Please try again after {X} second(s).'
function resendToken(tokenLink) {
    $.get(tokenLink, null).done(function (value) {
        var timeleft = parseInt(value.timeLeft);

        $("#resend-token").css({ "display": "none" });
        $("#countdown-text").text(resendEmailMessage.replace('{X}', timeleft));
        $("#countdown-text").css({ "display": "block" });
        var downloadTimer = setInterval(function () {
            if (timeleft <= 0) {
                clearInterval(downloadTimer);
                $("#countdown-text").css({ "display": "none" });
                $("#resend-token").css({ "display": "block" });
            }
            else {
                timeleft -= 1;
                $("#countdown-text").text(resendEmailMessage.replace('{X}', timeleft));
            }
        }, 1000);
    });
}

$(document).ready(function () {
    const keys = [
        'ERROR.VALIDATION.FIELD_REQUIRED',
        'PAGE.VERIFICATION.FIELD.VERFICATION_CODE',
        'PAGE.VERIFICATION.MESSAGE.RESEND'
    ]
    getTranslations(keys, function (data) {
        const requiredError = data?.['ERROR.VALIDATION.FIELD_REQUIRED'] ?? '{0} is required.';
        const verificationCodeRequired = requiredError.replace('{0}', data?.['PAGE.VERIFICATION.FIELD.VERFICATION_CODE'] ?? 'Verification Code');
        if (data?.['PAGE.VERIFICATION.MESSAGE.RESEND']) resendEmailMessage = data?.['PAGE.VERIFICATION.MESSAGE.RESEND'];
        var validationOptions = {
            rules: {
                Token: {
                    required: true,
                }
            },
            messages: {
                Token: verificationCodeRequired
            }
        };

        initFormValidation(validationOptions);
    });
});

$(window).on('load', function () {
    if ($('#resend-token-link').length > 0) {
        var tokenLink = $('#resend-token-link').val();
        if ($('#resend-token').length > 0) {
            $('#resend-token').click(function () {
                resendToken(tokenLink);
            });
        }
    }
});
