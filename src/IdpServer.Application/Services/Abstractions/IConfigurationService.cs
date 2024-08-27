using System.Collections.Generic;
using System.Threading.Tasks;
using IdpServer.Application.Model;

namespace IdpServer.Application.Service.Abstraction
{
    public interface IConfigurationService
    {
        Task<string> GetValueAsync(string key, string defaultValue);
        Task<IEnumerable<SystemConfigDto>> GetConfigsAsync(string keys);
    }
}
