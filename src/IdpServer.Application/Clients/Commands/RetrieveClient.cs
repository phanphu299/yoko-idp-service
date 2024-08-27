using MediatR;
using AHI.Infrastructure.SharedKernel.Model;

namespace IdpServer.Application.Client.Command
{
    public class RetrieveClient : IRequest<BaseResponse>
    {
        public string Data { get; set; }
        public string Upn { get; set; }
    }
}
