using AHI.Infrastructure.Repository.Generic;

namespace IdpServer.Application.Repository.Abstraction
{
    public interface IBrokerClientRepository : IRepository<Domain.Entity.BrokerClient, string>
    {
    }
}
