using System;
using IdpServer.Application.Model;
using MediatR;

namespace IdpServer.Application.User.Command
{
    public class GetUserDetails : IRequest<UserDto>
    {
        public Guid UserId { get; set; }
        public string Upn { get; set; }
        public bool IgnoreQueryFilters { get; set; }
    }
}