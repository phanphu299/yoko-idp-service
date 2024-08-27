using System;
using System.Threading.Tasks;

namespace IdpServer.Application.Service.Abstraction
{
    public interface IAccessControlService
    {
        Task<string> GetApplicationNameAsync(string applicationId, Guid correlationId);
    }
}