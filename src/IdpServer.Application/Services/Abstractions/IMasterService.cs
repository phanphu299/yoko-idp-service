using System.Collections.Generic;
using System.Threading.Tasks;
using IdpServer.Application.Model;

namespace IdpServer.Application.Service.Abstraction
{
    public interface IMasterService
    {
        Task<IEnumerable<TenantDto>> GetAllTenantsAsync(bool migrated = true, bool deleted = false);
        Task<IEnumerable<SubscriptionDto>> GetAllSubscriptionsAsync(bool migrated = true, bool deleted = false);
        Task<IEnumerable<ProjectDto>> GetAllProjectsAsync(bool migrated = true, bool deleted = false);
        Task<IEnumerable<User.Model.DateTimeFormatDto>> GetAllDateTimeFormatAsync();
    }
}
