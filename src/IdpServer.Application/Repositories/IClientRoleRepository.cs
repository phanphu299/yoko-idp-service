using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AHI.Infrastructure.Repository.Generic;

namespace IdpServer.Application.Repository.Abstraction
{
    public interface IClientRoleRepository : IRepository<Domain.Entity.ClientRole, Guid>
    {
        Task<IEnumerable<Domain.Entity.ClientRole>> GetByClientIdAsync(Guid clientId);
    }
}
