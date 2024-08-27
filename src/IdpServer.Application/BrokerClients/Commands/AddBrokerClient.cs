using IdpServer.Application.BrokerClient.Model;
using MediatR;

namespace IdpServer.Application.BrokerClient.Command
{
    public class AddBrokerClient : IRequest<BrokerClientDto>
    {
        public string CreatedBy { get; set; }
        public int ExpiredDays { get; set; } = 365;
        public string TenantId { get; set; }
        public string ProjectId { get; set; }
        public string SubscriptionId { get; set; }
        public int PasswordLength { get; set; } = 30;
    }
}
