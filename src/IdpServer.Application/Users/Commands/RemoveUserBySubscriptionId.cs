using System;
using MediatR;

namespace IdpServer.Application.User.Command
{
    public class RemoveUserBySubscriptionId : IRequest<bool>
    {
        public Guid Id { get; set; }
        public RemoveUserBySubscriptionId(Guid id)
        {
            Id = id;
        }
    }
}