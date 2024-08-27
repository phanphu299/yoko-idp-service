namespace IdentityServer4.Quickstart.Model
{
    public class SetPasswordSuccessInputModel
    {
        public string Username { get; set; }
        public string ReturnUrl { get; set; }
        public int AutoRedirectTime { get; set; } = 5;
    }
}