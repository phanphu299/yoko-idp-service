using AHI.Infrastructure.Repository.Generic;

namespace IdpServer.Application.Repository.Abstraction
{
    public interface ITimezoneRepository : IRepository<Domain.Entity.Timezone, int>
    {
    }
}
