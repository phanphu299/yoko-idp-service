using MediatR;

namespace IdpServer.Application.User.Command
{
    public class RevokeUserSessionByUpn : IRequest<bool>
    {
        public string Upn { get; set; }
        public RevokeUserSessionByUpn(string upn)
        {
            Upn = upn;
        }
    }
}