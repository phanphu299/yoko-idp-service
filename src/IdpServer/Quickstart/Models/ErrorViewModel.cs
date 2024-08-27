using IdentityServer4.Models;

namespace IdentityServer4.Quickstart.Model
{
    public class ErrorViewModel 
    {
        public string ErrorId { get; set; }
        public string ErrorTitle { get; set; }
        public string ErrorMessage { get; set; }
        public string EmailAddress { get; set; }
        public int AutoRedirectTime { get; set; } = 0;
        public string ReturnUrl { get; set; }
        public ErrorMessage Error { get; set; }
    }
}