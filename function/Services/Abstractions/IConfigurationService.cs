using System.Collections.Generic;
using System.Threading.Tasks;
using Function.Model;

namespace Function.Service.Abstraction
{
    public interface IConfigurationService
    {
        Task<string> GetValueAsync(string key, string defaultValue);
        Task<IEnumerable<SystemConfigDto>> GetConfigsAsync(string keys);
    }
}