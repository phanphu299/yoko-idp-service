using AHI.Infrastructure.Repository.Generic;
using System.Threading.Tasks;
namespace IdpServer.Application.Repository.Abstraction
{
    public interface IPersistedGrantRepository : IRepository<Domain.Entity.PersistedGrant, string>
    {
        Task RemoveAllAsync(string subjectId);
    }
}
