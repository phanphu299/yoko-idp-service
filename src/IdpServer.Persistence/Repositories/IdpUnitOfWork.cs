using AHI.Infrastructure.Repository;
using AHI.Infrastructure.Service.Tag.Service.Abstraction;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Domain.Entity;
using IdpServer.Persistence.Context;

namespace IdpServer.Persistence.Repository
{
    public class IdpUnitOfWork : BaseUnitOfWork, IIdpUnitOfWork
    {
        public IUserRepository Users { get; private set; }
        public IPersistedGrantRepository PersistedGrants { get; private set; }
        public ITimezoneRepository Timezones { get; private set; }
        public IBrokerClientRepository BrokerClients { get; private set; }
        public IEntityTagRepository<EntityTagDb> EntityTags { get; private set; }
        public IClientRepository Clients { get; private set; }

        public IdpUnitOfWork(IdpDbContext context,
                            IBrokerClientRepository brokerClientRepository,
                            IUserRepository userRepository,
                            ITimezoneRepository timezoneRepository,
                            IPersistedGrantRepository persistedGrantRepository,
                            IClientRepository clients,
                            IEntityTagRepository<EntityTagDb> entityTags)
                            : base(context)
        {
            Users = userRepository;
            PersistedGrants = persistedGrantRepository;
            Timezones = timezoneRepository;
            BrokerClients = brokerClientRepository;
            EntityTags = entityTags;
            Clients = clients;
        }
    }
}