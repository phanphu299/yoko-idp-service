using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AHI.Infrastructure.Repository.Generic;

namespace IdpServer.Application.Repository.Abstraction
{
    public interface IClientRepository : IRepository<Domain.Entity.Client, int>
    {
        Task<Domain.Entity.Client> FindByClientIdAsync(Guid clientId);
        Task<Domain.Entity.Client> FindByClientNameAsync(string name);
        //IQueryable<Domain.Entity.Client> GetAllClients();
        Task<bool> UpdateClientRedirectUrisAsync(int id, IEnumerable<string> redirectUris);
        Task<bool> UpdateClientPostLogoutRedirectUrisAsync(int id, IEnumerable<string> redirectUris);
        Task RemoveClientRedirectUrisAsync(int id);
        Task RemoveClientPostLogoutRedirectUrisAsync(int id);
        Task RemoveClientAsync(int id);
        Task RetrieveAsync(IEnumerable<Domain.Entity.Client> input);
        Task<Domain.Entity.Client> AddWithSaveChangeAsync(Domain.Entity.Client e);
        IQueryable<Domain.Entity.Client> AsQueryableTag(IQueryable<Domain.Entity.Client> query);
    }
}