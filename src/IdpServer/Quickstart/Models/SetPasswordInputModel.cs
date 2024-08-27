using System.ComponentModel.DataAnnotations;
using IdpServer.Application.Enum;
using System.Collections.Generic;
using IdpServer.Application.User.Model;

namespace IdentityServer4.Quickstart.Model
{
    public class SetPasswordInputModel
    {
        [CustomRequired]
        [Display(Name = "COMMON.FIELD.EMAIL")]
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LoginTypeCode { get; set; }
        [CustomRequired]
        [Display(Name = "COMMON.FIELD.PASSWORD")]
        public string NewPassword { get; set; }
        [CustomRequired]
        [Display(Name = "PAGE.SET_PASSWORD.FIELD.CONFIRM_PASSWORD")]
        public string ConfirmNewPassword { get; set; }
        public string SetPasswordToken { get; set; }
        public TokenTypeEnum TokenType { get; set; }
        public string ReturnUrl { get; set; }
        public IDictionary<string, string> PasswordValidationRules { get; set; }
    }
}
