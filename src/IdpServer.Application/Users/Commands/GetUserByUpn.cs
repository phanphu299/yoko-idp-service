using IdpServer.Application.Model;
using MediatR;

namespace IdpServer.Application.User.Command
{
    public class GetUserByUpn : IRequest<UserDto>
    {
        public string Upn { get; set; }
        public bool ignoreQueryFilters { get; set; } = false;
        public GetUserByUpn(string upn)
        {
            Upn = upn;
        }
    }
}