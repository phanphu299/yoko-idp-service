var errorPassword = document.getElementById('password_input');
var confirmPassword = document.getElementById('user_confirm_password');
var currentPasswordElement = document.getElementById("user_current_password");
var userPassword = document.getElementById('user_password');
var mainForm = document.getElementById("mainForm");
var sizeScreen = screen.width;
const handleIconPasswordField = function (e, field, eyesIconClassName) {
    if (field) {
        field.value = e.target.value;
    }
    if (e.target.value) {
        document.getElementsByClassName(eyesIconClassName)[0].style.right = 0;
    } else {
        if (sizeScreen <= 1024) {
            document.getElementsByClassName(eyesIconClassName)[0].style.right = "18px";
        } else {
            document.getElementsByClassName(eyesIconClassName)[0].style.right = "24px";
        }
    }
}

const handleIconConfirmPasswordField = function (e) {
    if (e.target.value && e.target.value === userPassword?.value) {
        document.getElementsByClassName('password-confirm-reveal')[0].style.right = 0;
    } else {
        if (sizeScreen <= 1024) {
            document.getElementsByClassName('password-confirm-reveal')[0].style.right = "18px";
        } else {
            document.getElementsByClassName('password-confirm-reveal')[0].style.right = "24px";
        }
    }
}
if (confirmPassword) {
    confirmPassword.addEventListener('input', handleIconConfirmPasswordField);
    confirmPassword.addEventListener('blur', handleIconConfirmPasswordField);

}
if (userPassword) {
    userPassword.addEventListener('input', (e) => handleIconPasswordField(e, userPassword, 'password-policy-reveal'));
    userPassword.addEventListener('focus', (e) => {
        handleIconPasswordField(e, userPassword, 'password-policy-reveal');
        $("span.field-validation-error").remove();
    } );
    userPassword.addEventListener('blur', (e) => handleIconPasswordField(e, userPassword, 'password-policy-reveal'));
}
if (currentPasswordElement) {
    currentPasswordElement.addEventListener('input', (e) => handleIconPasswordField(e, currentPasswordElement, 'password-current-reveal'));
    currentPasswordElement.addEventListener('focus', (e) => {
        handleIconPasswordField(e, currentPasswordElement, 'password-current-reveal');
        $("span.field-validation-error").remove();
    } );
    currentPasswordElement.addEventListener('blur', (e) => handleIconPasswordField(e, currentPasswordElement, 'password-current-reveal'));
}
if (currentPasswordElement) {
    currentPasswordElement.addEventListener('input', handleIconCurrentPasswordField);
    currentPasswordElement.addEventListener('blur', handleIconCurrentPasswordField);
}
if (errorPassword) {
    errorPassword.addEventListener('input', (e) => handleIconPasswordField(e, errorPassword, 'password-reveal'));
    errorPassword.addEventListener('blur', (e) => handleIconPasswordField(e, errorPassword, 'password-reveal'));
}
const handleEyesIcon = (e, className, errorId) => {
    const errorContent = $(`#${errorId}`);
    if (e.value && !errorContent.text()) {
        document.getElementsByClassName(className)[0].style.right = 0;
    } else {
        if (sizeScreen <= 1024) {
            document.getElementsByClassName(className)[0].style.right = "18px";
        } else {
            document.getElementsByClassName(className)[0].style.right = "24px";
        }
    }
};
if (mainForm) {
    mainForm.addEventListener("submit", (event) => {
        if (currentPasswordElement)
            handleEyesIcon(
                currentPasswordElement,
                "password-current-reveal",
                "user_current_password-error"
            );
        if (confirmPassword)
            handleEyesIcon(
                confirmPassword,
                "password-confirm-reveal",
                "user_confirm_password-error"
            );
        if (errorPassword)
            handleEyesIcon(
                errorPassword,
                "password-reveal",
                "password_input-error"
            );
        if (userPassword)
            handleEyesIcon(
                userPassword,
                "password-policy-reveal",
                "user_password-error"
            );
    });
}
jQuery.validator.addMethod(
    "validatePasswordPolicy",
    function (value) {
        return true;
    },
    "Password invalid"
);
