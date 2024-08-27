using System;
using AHI.Infrastructure.SharedKernel.Model;
using MediatR;

namespace IdpServer.Application.User.Command
{
    public class RestoreUser : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }
        public RestoreUser(Guid id)
        {
            Id = id;
        }
    }
}