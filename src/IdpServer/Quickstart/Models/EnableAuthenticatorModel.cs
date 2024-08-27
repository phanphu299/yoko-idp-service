using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IdentityServer4.Quickstart.Model
{
    public class EnableAuthenticatorModel
    {
        public string Username { get; set; }
        public string ReturnUrl { get; set; }
        [CustomRequired]
        [Display(Name = "PAGE.VERIFICATION.FIELD.VERFICATION_CODE")]
        public string Token { get; set; }
        [BindNever]
        public string SharedKey { get; set; }

        [BindNever]
        public string AuthenticatorUri { get; set; }
    }
}
