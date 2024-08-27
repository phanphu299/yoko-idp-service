using System.Threading.Tasks;
using Function.Model;

namespace Function.Service.Abstraction
{
    public interface IBrokerClientsService
    {
        Task<AuthenticationResultDto> AuthenticateAsync(AuthenticateBrokerClientRequest request);
    }
}