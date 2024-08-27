using System;
using AHI.Infrastructure.SharedKernel.Model;
using MediatR;

namespace IdpServer.Application.User.Command
{
    public class ChangePassword : IRequest<BaseResponse>
    {
        public Guid UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}