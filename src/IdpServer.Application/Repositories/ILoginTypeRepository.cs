using System.Collections.Generic;
using System.Threading.Tasks;
using AHI.Infrastructure.Repository.Generic;
using IdpServer.Application.User.Model;

namespace IdpServer.Application.Repository.Abstraction
{
    public interface ILoginTypeRepository : IRepository<Domain.Entity.LoginType, string>
    {
        Task<List<LoginTypeDto>> GetLoginTypesAsync();
    }
}
