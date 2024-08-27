using System.Threading.Tasks;
using IdpServer.Domain.Entity;
using AHI.Infrastructure.Repository.Generic;

namespace IdpServer.Application.Repository.Abstraction
{
    public interface IUserTokenRepository : IRepository<Domain.Entity.UserToken, int>
    {
        Task<UserToken> FindByTokenKeyAsync(string tokenKey);
        Task<UserToken> GenerateTokenAsync(string userName, string tokenType, string redirectUrl = "", int tokenLenght = 32, double expiredInSeconds = 86400);
        Task<UserToken> GetUserTokenAsync(string userName, string tokenKey, string tokenType);
        Task<UserToken> GetLatestTokenByTypeAsync(string userName, string tokenType);
        Task DeleteTokenByTypeAsync(string userName, string tokenType);
        Task DeleteTokenAsync(string userName, string tokenKey);
    }
}