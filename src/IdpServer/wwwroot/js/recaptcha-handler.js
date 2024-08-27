var passwordE = document.getElementById("password_input");
var userPasswordE = document.getElementById("user_password");
var confirmPasswordE = document.getElementById("user_confirm_password");
var currentPasswordE = document.getElementById("user_current_password");
function onSubmit(token) {
    var currentForm = $("form.page-login")[0];
    if (currentForm !== null) {
        if (!$(currentForm).valid()) {
            $(currentForm).validate().form();
            handleEyesIconBeforeSubmit();
        } else {
            currentForm.submit();
        }
    }
}

function validate(event) {
    event.preventDefault();
    var currentForm = $("form.page-login")[0];
    if (!$(currentForm).valid()) {
        $(currentForm).validate().form();
        handleEyesIconBeforeSubmit();
        return;
    }
    grecaptcha.execute();
}

function expiredCallback(token) {
    grecaptcha.reset("recaptcha");
}

$(window).on("load", function () {
    var buttons = document.getElementsByClassName("btn-primary");
    for (let i = 0; i < buttons.length; i++) {
        let button = buttons[i];
        if (button.nodeName.toUpperCase() === "INPUT") {
            button.onclick = validate;
        }
    }
});
const handleEyesIconBeforeSubmit = () => {
    if (passwordE)
        handleEyesIcon(passwordE, "password-reveal", "password_input-error");
    if (userPasswordE)
        handleEyesIcon(
            userPasswordE,
            "password-policy-reveal",
            "user_password-error"
        );
    if (confirmPasswordE)
        handleEyesIcon(
            confirmPasswordE,
            "password-confirm-reveal",
            "user_confirm_password-error"
        );
    if (currentPasswordE)
        handleEyesIcon(
            currentPasswordE,
            "password-current-reveal",
            "user_current_password-error"
        );
};
