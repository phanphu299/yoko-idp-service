using System.Collections.Generic;
using System.Threading.Tasks;

namespace Function.Service.Abstraction
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string typeCode, IDictionary<string, object> customField = null);
    }
}