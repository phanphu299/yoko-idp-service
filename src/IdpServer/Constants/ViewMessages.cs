// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace IdpServer.Constant
{
    public class ViewMessages
    {
        public static string InvalidEmailFormat = "Invalid email format.";
        public static string AccountNotFound = "Can not find user account with email {0}.";
        public static string SendEmailFailed = "There was a problem sending email to you, please try again later.";
        public static string TokenExpired = "Oops! The link already expired.";
        public static string AntiForgeryTokenExpired = "Oops! The Antiforgery token is invalid.";
        public static string ConfirmPasswordNotMatch = "Confirm password does not match with password.";
        public static string ResetPasswordEmailSent = "An email has been sent to you, please check your inbox.";
        public static string UserAccountNotFound = "User account not found.";
        public static string WrongUserNameOrPassword = "Invalid login credentials";
        public static string WrongCurrentPassword = "Invalid Current Password";
        public static string AccountLocked = "This account has been locked.";
        public static string AccountHardLocked = "You have tried too many times allowed. Please contact Administrator for support.";
        public static string IpLoginLocked = "You have been locked out due to too many invalid login attempts, please try again later.";
        public static string ErrorPageTitle = "There was an error";
        public static string PasswordComplexityNotMatch = "Input password does not meet the policy";
        public static string VerificationIncorrect = "The code you entered is incorrect.";
        public static string AccessDenied = "You don't have permission to access this resource.";
        public static string AccountNotSupported = "Account is not supported with this login type.";
        public static string InvalidCredentials = "Invalid credentials";
    }
}
