using IdpServer.Application.User.Command;
using IdpServer.Application.User.Model;
using AHI.Infrastructure.Service.Abstraction;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IdpServer.Application.Service.Abstraction
{
    public interface ILoginTypeService : ISearchService<Domain.Entity.LoginType, string, SearchLoginType, LoginTypeDto>
    {
        Task<List<LoginTypeDto>> GetLoginTypesAsync();
    }
}
