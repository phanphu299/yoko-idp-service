using System;
using System.Threading.Tasks;
using AHI.Infrastructure.Repository.Generic;

namespace IdpServer.Application.Repository.Abstraction
{
    public interface IUserRepository : IRepository<Domain.Entity.User, string>
    {
        //Task<IdpServer.Domain.Entity.User> FindUserByUpnAsync(string userName);
        Task<IdpServer.Domain.Entity.User> FindUserByUserIdAsync(Guid userId, bool ignoreQueryFilters = false);
        //Task<IdpServer.Domain.Entity.User> FindUserByUpnAndProviderAsync(string userName, string loginProvider);
    }
}