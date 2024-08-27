using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using IdpServer.Application.User.Model;

namespace IdentityServer4.Quickstart.Model
{
    public class ChangePasswordInputModel
    {
        [CustomRequired]
        [Display(Name = "COMMON.FIELD.EMAIL")]
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LoginTypeCode { get; set; }
        [CustomRequired]
        [Display(Name = "PAGE.CHANGE_PASSWORD.FIELD.CURRENT_PASSWORD")]
        public string CurrentPassword { get; set; }
        [CustomRequired]
        [Display(Name = "PAGE.CHANGE_PASSWORD.FIELD.NEW_PASSWORD")]
        public string NewPassword { get; set; }
        [CustomRequired]
        [Display(Name = "PAGE.CHANGE_PASSWORD.FIELD.CONFIRM_NEW_PASSWORD")]
        public string ConfirmNewPassword { get; set; }
        [CustomRequired]
        public string Token { get; set; }
        public string ReturnUrl { get; set; }
        public IDictionary<string, string> PasswordValidationRules { get; set; }
    }
}
