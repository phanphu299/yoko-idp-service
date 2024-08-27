const regexString = $('#emailRegex').val() || '^(?=.{1,250}@)[a-zA-Z0-9](?:(?:\\.|_|-)?[a-zA-Z0-9])*@(?=.{1,250}$)[a-zA-Z0-9](?:(?:\\.|-)?[a-zA-Z0-9])*\\.[a-zA-Z]{2,}$';
const emailRegex = new RegExp(regexString);

const commonOptions = {
    errorElement: 'span',
    errorClass: 'val-error-message',
    highlight: function (element) {
        $(element).addClass('val-error');
        $(element.labels[0]).removeClass('required').addClass('val-error');
    },
    unhighlight: function (element) {
        $(element).removeClass('val-error');
        $(element.labels[0]).removeClass('val-error');
    },
    onkeyup: function (element, event) {
        $(element.labels[0]).removeClass('required');
        $(element).parent().find('span').remove();

        if (event.which === 9 && this.elementValue(element) === '') {
            return;
        }
        this.element(element);
    },
    onfocusout: function (element) {
        this.element(element);
    }
};

const initFormValidation = (validationOptions) => {
    var options = Object.assign({}, validationOptions, commonOptions);
    $('form#mainForm').validate(options);

    $('input.required').focus(function () {
        $(this).closest('.form-group').find('label').removeClass('required');
        $(this).parent().find('span.field-validation-error').remove();
    });

    $('input.required').each(function () {
        if ($(this).val()) {
            $(this).closest('.form-group').find('label').removeClass('required');
        }
        else {
            $(this).closest('.form-group').find('label').addClass('required');
        }
    })
}

const getTranslations = (keys, onSuccess) => {
    $.ajax({
        type: "POST",
        dataType: "json",
        url: "/Account/GetTranslations",
        data: { keys: keys },
        success: function (data) {
            onSuccess(data.translations);
        }
    });
}

$.validator.methods.email = function (value, element) {
    return this.optional(element) || emailRegex.test(value);
}

$("i.password-reveal").click(function () {
    const password = $(this).next("input.password");
    const type = password.attr("type") === "password" ? "text" : "password";
    password.attr("type", type);
    $(this).toggleClass("bi-eye");
});
