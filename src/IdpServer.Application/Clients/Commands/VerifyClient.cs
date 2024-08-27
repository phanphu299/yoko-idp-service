using MediatR;
using AHI.Infrastructure.SharedKernel.Model;

namespace IdpServer.Application.Client.Command
{
    public class VerifyClient : IRequest<BaseResponse>
    {
        public string Data { get; set; }
    }
}
