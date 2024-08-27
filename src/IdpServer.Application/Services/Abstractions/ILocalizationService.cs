using System.Threading.Tasks;
using System.Collections.Generic;
using IdpServer.Application.Model;

namespace IdpServer.Application.Service.Abstraction
{
    public interface ILocalizationService
    {
        Task<IEnumerable<LanguageDto>> GetLanguagesAsync();
    }
}
