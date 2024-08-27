using IdpServer.Application.SharedKernel;
using MediatR;

namespace IdpServer.Application.User.Command
{
    public class ForceResetPassword : IRequest<BaseResponse>
    {
        public string Upn { get; set; }
        public string RequestBy { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
    }
}