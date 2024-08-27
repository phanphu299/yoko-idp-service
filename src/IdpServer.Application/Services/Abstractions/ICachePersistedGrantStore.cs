using IdentityServer4.Stores;
using System.Threading.Tasks;

namespace IdpServer.Application.Service.Abstraction
{
    public interface ICachePersistedGrantStore : IPersistedGrantStore
    {
        Task RemoveAllAsync(string subjectId);
    }
}
