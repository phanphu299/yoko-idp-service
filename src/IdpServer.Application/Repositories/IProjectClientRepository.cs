using System;
using System.Threading.Tasks;
using AHI.Infrastructure.Repository.Generic;

namespace IdpServer.Application.Repository.Abstraction
{
    public interface IProjectClientRepository : IRepository<Domain.Entity.ProjectClient, Guid>
    {
        Task<Domain.Entity.ProjectClient> FindByClientIdAsync(int clientId);
    }
}
