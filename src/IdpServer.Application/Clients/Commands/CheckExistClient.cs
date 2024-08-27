using System.Collections.Generic;
using MediatR;
using IdpServer.Application.SharedKernel;

namespace IdpServer.Application.Client.Command
{
    public class CheckExistClient : IRequest<BaseResponse>
    {
        public IEnumerable<string> Ids { get; set; }

        public CheckExistClient(IEnumerable<string> ids)
        {
            Ids = ids;
        }
    }
}
