using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.Model
{
    public class ForgotPasswordInputModel
    {
        [CustomRequired]
        [Display(Name = "COMMON.FIELD.EMAIL")]
        public string Username { get; set; }
        public bool Result { get; set; } = false;
        public string ReturnUrl { get; set; }
    }
}
