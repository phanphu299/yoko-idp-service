using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.Model
{
    public class VerificationLoginInputModel
    {
        public string Username { get; set; }
        [CustomRequired]
        [Display(Name = "PAGE.VERIFICATION.FIELD.VERFICATION_CODE")]
        public string Token { get; set; }
        public string ReturnUrl { get; set; }
    }
}
