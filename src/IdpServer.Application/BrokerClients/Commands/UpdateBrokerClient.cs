using IdpServer.Application.BrokerClient.Model;
using MediatR;

namespace IdpServer.Application.BrokerClient.Command
{
    public class UpdateBrokerClient : IRequest<BrokerClientDto>
    {
        public string Id { get; set; }
        public string UpdatedBy { get; set; }
        public int? ExpiredDays { get; set; }
        public int PasswordLength { get; set; } = 30;
    }
}
