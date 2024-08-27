using IdpServer.Application.SharedKernel;
using MediatR;

namespace IdpServer.Application.User.Command
{
    public class SetUserDefaultPage : IRequest<BaseResponse>
    {
        public string Upn { get; set; }
        public string DefaultPage { get; set; }
    }
}