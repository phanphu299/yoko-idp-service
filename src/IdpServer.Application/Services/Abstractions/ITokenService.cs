using System.Threading.Tasks;
using IdpServer.Application.Enum;
using IdpServer.Application.Model;

namespace IdpServer.Application.Service.Abstraction
{
    public interface ITokenService
    {
        Task<UserTokenDto> GenerateTokenAsync(string userName, TokenTypeEnum tokenType, string redirectUrl = "", int tokenLenght = 32, double expiredInSeconds = 900);
        Task<UserTokenDto> GetUserTokenAsync(string userName, string tokenKey, TokenTypeEnum tokenType);
        Task<UserTokenDto> GetLatestTokenByTypeAsync(string userName, TokenTypeEnum tokenType);
        Task DeleteTokenByTypeAsync(string userName, TokenTypeEnum tokenType);
        //Task DeleteTokenAsync(string userName, string tokenKey);
    }
}
