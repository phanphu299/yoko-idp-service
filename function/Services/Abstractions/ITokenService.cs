using System.Threading.Tasks;
using Function.Model;

namespace Function.Service.Abstraction
{
    public interface ITokenService
    {
        Task<UserTokenDto> GenerateTokenAsync(string userName, string tokenType, string redirectUrl = "", int tokenLenght = 32, double expiredInSeconds = 86400);
    }
}