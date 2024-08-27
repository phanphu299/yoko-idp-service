using System;
using MediatR;

namespace IdpServer.Application.User.Command
{
    public class RemoveUserById : IRequest<bool>
    {
        public Guid Id { get; set; }
        public RemoveUserById(Guid id)
        {
            Id = id;
        }
    }
}