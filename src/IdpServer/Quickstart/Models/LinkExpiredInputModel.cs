using IdpServer.Application.Enum;

namespace IdentityServer4.Quickstart.Model
{
    public class LinkExpiredInputModel
    {
        public string Username { get; set; }
        public bool Result { get; set; } = false;
        public TokenTypeEnum TokenType { get; set; }
        public string ReturnUrl { get; set; }
    }
}