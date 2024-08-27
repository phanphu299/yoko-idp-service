using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdpServer.Application.Service.Abstraction
{
    public interface IEmailService
    {
        bool IsValid(string input);
        Task SendEmailAsync(string email, string typeCode, IDictionary<string, object> customField = null);
    }
}
