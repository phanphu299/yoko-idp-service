using System;
using System.Collections.Generic;
using IdpServer.Application.SharedKernel;
using MediatR;

namespace IdpServer.Application.Client.Command
{
    public class DeleteClient : IRequest<BaseResponse>
    {
        public IEnumerable<Guid> ClientIds { get; set; }
        public DeleteClient()
        {
            ClientIds = new List<Guid>();
        }
    }
}
