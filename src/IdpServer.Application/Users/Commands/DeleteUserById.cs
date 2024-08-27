using System;
using MediatR;

namespace IdpServer.Application.User.Command
{
    public class DeleteUserById : IRequest<bool>
    {
        public Guid Id { get; set; }
        public DeleteUserById(Guid id)
        {
            Id = id;
        }
    }
}