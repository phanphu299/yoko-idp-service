using AHI.Infrastructure.Repository.Generic;
using AHI.Infrastructure.Service.Tag.Service.Abstraction;
using IdpServer.Domain.Entity;

namespace IdpServer.Application.Repository.Abstraction
{
    public interface IIdpUnitOfWork : IUnitOfWork
    {
        IBrokerClientRepository BrokerClients { get; }
        IUserRepository Users { get; }
        ITimezoneRepository Timezones { get; }
        IPersistedGrantRepository PersistedGrants { get; }
        IClientRepository Clients { get; }
        IEntityTagRepository<EntityTagDb> EntityTags { get; }
    }
}