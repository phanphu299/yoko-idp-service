using System;
using IdpServer.Application.SharedKernel;
using MediatR;

namespace IdpServer.Application.User.Command
{
    public class SetUserMFA : IRequest<BaseResponse>
    {
        public Guid UserId { get; set; }
        public bool MFA { get; set; }
    }
}