var mainForm = document.getElementById("mainForm");
var userPassword = document.getElementById("user_password");
var userPasswordChange = document.getElementById("user_password_change");
var passwordLength = document.getElementById("password-policy-length");
var validatePasswordBox = document.getElementById("validate-password-box");
var passwordCategories = document.getElementById("password-policy-categories");
var passwordChanged = document.getElementById("password-policy-changed");

var passwordContainInfo = document.getElementById(
    "password-policy-contain-info"
);
var confirmPassword = document.getElementById("user_confirm_password");
var passwordPolicyRuleElement = document.getElementById("password-policy-rule");
var currentPasswordElement = document.getElementById("user_current_password");
var sizeScreen = screen.width;
const classIconApprove =
    "validate-password-icon validate-password-icon-approve";
const classIconError = "validate-password-icon validate-password-icon-error";

const passwordPolicyConfigKey = {
    PASSWORD_LENGTH_ENABLED: "password.policy.length.enabled",
    PASSWORD_LENGTH_MIN: "password.policy.length.min",
    PASSWORD_LENGTH_MAX: "password.policy.length.max",
    PASSWORD_HISTORY_ENABLED: "password.policy.history.enabled",
    PASSWORD_HISTORY_NUMBER: "password.policy.history.number",
    PASSWORD_HISTORY_LETTER_CHANGE_NUMBER:
        "password.policy.history_letter_change",
    PASSWORD_EXPIRATION_ENABLED: "password.policy.expiration.enabled",
    PASSWORD_EXPIRATION_DAY: "password.policy.expiration.day",
    ACCOUNT_LOCKOUT_ENABLED: "password.policy.lockout.enabled",
    ACCOUNT_LOCKOUT_ATTEMPT: "password.policy.lockout.attempt",
    ACCOUNT_LOCKOUT_DURATION: "password.policy.lockout.duration",
    PASSWORD_COMPLEXITY_ENABLED: "password.policy.complexity.enabled",
    PASSWORD_COMPLEXITY_LOWERCASE_MIN:
        "password.policy.complexity.lowercase_min",
    PASSWORD_COMPLEXITY_UPPERCASE_MIN:
        "password.policy.complexity.uppercase_min",
    PASSWORD_COMPLEXITY_SPECIAL_CHAR_MIN:
        "password.policy.complexity.special_char_min",
    PASSWORD_COMPLEXITY_NUMERIC_CHAR_MIN:
        "password.policy.complexity.numeric_char_min",
};
const handleShowPasswordPolicy = function (value, rule, userInfo, isChange) {
    var elm = document.getElementsByClassName("validate-password")[0];
    elm.style.display = "block";
    elm.style.top = `-${elm.offsetHeight / 2 - 20}px`;
    document.getElementsByClassName(
        "validate-password-arrow"
    )[0].style.display = "block";
    handleValidatePassword(value, rule, userInfo, isChange);
};
const handleHiddenPasswordPolicy = function (e, rule, userInfo, isChange) {
    document.getElementsByClassName("validate-password")[0].style.display =
        "none";
    document.getElementsByClassName(
        "validate-password-arrow"
    )[0].style.display = "none";
    handleValidatePassword(e.target.value, rule, userInfo, isChange);
};
function formatConfigPasswordPolicyToNumber(str) {
    if (!str) return 0;
    return Number(str) > 0 ? Number(str) : 0;
}
function countLowercaseCharacters(str) {
    var lowercaseChars = str.split("").filter(function (char) {
        return char >= "a" && char <= "z";
    });

    return lowercaseChars.length;
}
function countUppercaseCharacters(str) {
    var uppercaseChars = str.split("").filter(function (char) {
        return char >= "A" && char <= "Z";
    });

    return uppercaseChars.length;
}
function countNumericCharacters(str) {
    var numericChars = str.split("").filter(function (char) {
        return char >= "0" && char <= "9";
    });

    return numericChars.length;
}
function countSpecialCharacters(str) {
    var specialChars = str.split("").filter(function (char) {
        return /[~`!@#$%^&*()_+\-=\[\]{};':"\\|,\s.<>\/?]+/.test(char);
    });

    return specialChars.length;
}
function countLettersChanged(str1, str2) {
    const charCount = {};

    for (let i = 0; i < str1.length; i++) {
        const char = str1[i];
        charCount[char] = (charCount[char] || 0) + 1;
    }

    let differentCount = 0;
    for (let i = 0; i < str2.length; i++) {
        const char = str2[i];
        if (!charCount[char]) {
            differentCount++;
        } else {
            charCount[char]--;
        }
    }

    return differentCount;
}
const handleValidatePassword = function (
    value,
    rule,
    userInfo,
    isChange = false
) {
    var isValidate = true;
    if (!value) {
        if (passwordLength) passwordLength.className = classIconError;
        if (passwordCategories) passwordCategories.className = classIconError;
        if (passwordContainInfo) passwordContainInfo.className = classIconError;
        if (isChange && passwordChanged)
            passwordChanged.className = classIconError;
        isValidate = false;
    } else {
        if (
            rule[passwordPolicyConfigKey.PASSWORD_LENGTH_ENABLED] === "true" &&
            (value.length >
                formatConfigPasswordPolicyToNumber(
                    rule[passwordPolicyConfigKey.PASSWORD_LENGTH_MAX]
                ) ||
                value.length <
                    formatConfigPasswordPolicyToNumber(
                        rule[passwordPolicyConfigKey.PASSWORD_LENGTH_MIN]
                    ))
        ) {
            if (passwordLength) passwordLength.className = classIconError;
            isValidate = false;
        } else {
            if (passwordLength) passwordLength.className = classIconApprove;
        }
        const isInvalidCountLowercaseCharacters =
            countLowercaseCharacters(value) <
            formatConfigPasswordPolicyToNumber(
                rule[passwordPolicyConfigKey.PASSWORD_COMPLEXITY_LOWERCASE_MIN]
            );
        const isInvalidCountUppercaseCharacters =
            countUppercaseCharacters(value) <
            formatConfigPasswordPolicyToNumber(
                rule[passwordPolicyConfigKey.PASSWORD_COMPLEXITY_UPPERCASE_MIN]
            );
        const isInvalidCountNumericCharacters =
            countNumericCharacters(value) <
            formatConfigPasswordPolicyToNumber(
                rule[
                    passwordPolicyConfigKey.PASSWORD_COMPLEXITY_NUMERIC_CHAR_MIN
                ]
            );
        const isInvalidCountSpecialCharacters =
            countSpecialCharacters(value) <
            formatConfigPasswordPolicyToNumber(
                rule[
                    passwordPolicyConfigKey.PASSWORD_COMPLEXITY_SPECIAL_CHAR_MIN
                ]
            );
        const numberConditionNeedCheck =
            (formatConfigPasswordPolicyToNumber(
                rule[passwordPolicyConfigKey.PASSWORD_COMPLEXITY_LOWERCASE_MIN]
            ) > 0
                ? 1
                : 0) +
            (formatConfigPasswordPolicyToNumber(
                rule[passwordPolicyConfigKey.PASSWORD_COMPLEXITY_UPPERCASE_MIN]
            ) > 0
                ? 1
                : 0) +
            (formatConfigPasswordPolicyToNumber(
                rule[
                    passwordPolicyConfigKey.PASSWORD_COMPLEXITY_NUMERIC_CHAR_MIN
                ]
            ) > 0
                ? 1
                : 0) +
            (formatConfigPasswordPolicyToNumber(
                rule[
                    passwordPolicyConfigKey.PASSWORD_COMPLEXITY_SPECIAL_CHAR_MIN
                ]
            ) > 0
                ? 1
                : 0);
        if (
            rule[passwordPolicyConfigKey.PASSWORD_COMPLEXITY_ENABLED] ===
                "true" &&
            numberConditionNeedCheck !== 4 &&
            (isInvalidCountLowercaseCharacters ||
                isInvalidCountUppercaseCharacters ||
                isInvalidCountNumericCharacters ||
                isInvalidCountSpecialCharacters)
        ) {
            isValidate = false;
            if (passwordCategories)
                passwordCategories.className = classIconError;
        } else if (
            rule[passwordPolicyConfigKey.PASSWORD_COMPLEXITY_ENABLED] ===
                "true" &&
            numberConditionNeedCheck === 4 &&
            [
                isInvalidCountLowercaseCharacters,
                isInvalidCountUppercaseCharacters,
                isInvalidCountNumericCharacters,
                isInvalidCountSpecialCharacters,
            ].filter((value) => value === false).length < 3
        ) {
            isValidate = false;
            if (passwordCategories)
                passwordCategories.className = classIconError;
        } else {
            if (passwordCategories)
                passwordCategories.className = classIconApprove;
        }
        if (
            (userInfo.FirstName &&
                value
                    .toLowerCase()
                    .includes(userInfo.FirstName.toLowerCase())) ||
            (userInfo.Upn &&
                value
                    .toLowerCase()
                    .includes(userInfo.Upn.split("@")[0].toLowerCase())) ||
            (userInfo.LastName &&
                value.toLowerCase().includes(userInfo.LastName.toLowerCase()))
        ) {
            isValidate = false;
            if (passwordContainInfo)
                passwordContainInfo.className = classIconError;
        } else {
            if (passwordContainInfo)
                passwordContainInfo.className = classIconApprove;
        }
        const letterChange = formatConfigPasswordPolicyToNumber(
            rule[passwordPolicyConfigKey.PASSWORD_HISTORY_LETTER_CHANGE_NUMBER]
        );
        if (
            isChange &&
            rule[passwordPolicyConfigKey.PASSWORD_HISTORY_ENABLED] === "true" &&
            letterChange > 0 &&
            ((currentPasswordElement &&
                countLettersChanged(currentPasswordElement.value || "", value) <
                    letterChange) ||
                !currentPasswordElement.value)
        ) {
            isValidate = false;
            if (passwordChanged) passwordChanged.className = classIconError;
        } else {
            if (isChange && passwordChanged)
                passwordChanged.className = classIconApprove;
        }
    }

    if (!isValidate) {
        if (sizeScreen <= 1024) {
            document.getElementsByClassName(
                "password-policy-reveal"
            )[0].style.right = "18px";
        } else {
            document.getElementsByClassName(
                "password-policy-reveal"
            )[0].style.right = "24px";
        }
    } else {
        document.getElementsByClassName(
            "password-policy-reveal"
        )[0].style.right = 0;
    }
    return isValidate;
};
const handleIconConfirmPasswordField = function (e) {
    if (e.target.value && e.target.value === userPassword?.value) {
        document.getElementsByClassName(
            "password-confirm-reveal"
        )[0].style.right = 0;
    } else {
        if (sizeScreen <= 1024) {
            document.getElementsByClassName(
                "password-confirm-reveal"
            )[0].style.right = "18px";
        } else {
            document.getElementsByClassName(
                "password-confirm-reveal"
            )[0].style.right = "24px";
        }
    }
};
if (confirmPassword) {
    confirmPassword.addEventListener("input", handleIconConfirmPasswordField);
    confirmPassword.addEventListener("blur", handleIconConfirmPasswordField);
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
const handleIconCurrentPasswordField = function (e, rule, userInfo, isChange) {
    if (e.target.value) {
        document.getElementsByClassName(
            "password-current-reveal"
        )[0].style.right = 0;
    } else {
        if (sizeScreen <= 1024) {
            document.getElementsByClassName(
                "password-current-reveal"
            )[0].style.right = "18px";
        } else {
            document.getElementsByClassName(
                "password-current-reveal"
            )[0].style.right = "24px";
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

        if (userPassword)
            handleEyesIcon(
                userPassword,
                "password-policy-reveal",
                "user_password-error"
            );
    });
}
$(document).ready(function () {
    const data = $("#password-policy-rule").data();
    const userInfo = {
        FirstName: data.firstName,
        LastName: data.lastName,
        Upn: data.userUpn,
    };
    if (userPassword) {
        userPassword.addEventListener("blur", (e) =>
            handleHiddenPasswordPolicy(
                e,
                data.passwordPolicy,
                userInfo,
                Boolean(data.isChange)
            )
        );
        userPassword.addEventListener("focus", (e) => {
            handleShowPasswordPolicy(
                e.target.value,
                data.passwordPolicy,
                userInfo,
                Boolean(data.isChange)
            );
            $("span.field-validation-error").remove();
        });
        userPassword.addEventListener("input", (e) =>
            handleShowPasswordPolicy(
                e.target.value,
                data.passwordPolicy,
                userInfo,
                Boolean(data.isChange)
            )
        );
    }
    if (currentPasswordElement) {
        currentPasswordElement.addEventListener("input", (e) =>
            handleIconCurrentPasswordField(
                e,
                data.passwordPolicy,
                userInfo,
                Boolean(data.isChange)
            )
        );
        currentPasswordElement.addEventListener("blur", (e) => {
            handleIconCurrentPasswordField(
                e,
                data.passwordPolicy,
                userInfo,
                Boolean(data.isChange)
            );
            if (userPassword?.value) {
                userPassword.focus();
                userPassword.blur();
            }
        });
    }
});
jQuery.validator.addMethod(
    "validatePasswordPolicy",
    function (value) {
        const data = $("#password-policy-rule").data();
        const userInfo = {
            FirstName: data.firstName,
            LastName: data.lastName,
            Upn: data.userUpn,
        };
        return handleValidatePassword(
            value,
            data.passwordPolicy,
            userInfo,
            Boolean(data.isChange)
        );
    },
    "Password invalid"
);
