using System;
using IdpServer.Application.Model;
using MediatR;

namespace IdpServer.Application.User.Command
{
    public class GetUserById : IRequest<UserDto>
    {
        public Guid Id { get; set; }
        public bool IgnoreQueryFilters { get; set; }
        public GetUserById(Guid id, bool ignoreQueryFilters = false)
        {
            Id = id;
            IgnoreQueryFilters = ignoreQueryFilters;
        }
    }
}