using System;
using System.Linq;
using System.Threading.Tasks;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Domain.Entity;
using IdpServer.Persistence.Context;
using AHI.Infrastructure.Repository.Generic;
using Microsoft.EntityFrameworkCore;

namespace IdpServer.Persistence.Repository
{
    public class UserTokenRepository : GenericRepository<UserToken, int>, IUserTokenRepository
    {
        private readonly IdpDbContext _dbContext;
        public UserTokenRepository(IdpDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteTokenAsync(string userName, string tokenKey)
        {
            var tokens = await _dbContext.UserTokens.Where(x => x.TokenKey == tokenKey && x.UserName == userName).ToListAsync();
            if (tokens.Any())
            {
                _dbContext.UserTokens.RemoveRange(tokens);
                await _dbContext.SaveChangesAsync();
            }
        }

        public Task<UserToken> FindByTokenKeyAsync(string tokenKey)
        {
            return _dbContext.UserTokens.Where(x => x.TokenKey == tokenKey).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<UserToken> GenerateTokenAsync(string userName, string tokenType, string redirectUrl = "", int tokenLenght = 32, double expiredInSeconds = 86400)
        {
            var undeletedTokens = _dbContext.UserTokens.Where(x => x.UserName == userName && x.TokenType == tokenType && !x.Deleted);
            foreach (var item in undeletedTokens)
            {
                item.Deleted = true;
            }
            _dbContext.UserTokens.UpdateRange(undeletedTokens);
            var token = new UserToken()
            {
                UserName = userName,
                RedirectUrl = redirectUrl,
                CreatedDate = DateTime.UtcNow,
                ExpiredDate = DateTime.UtcNow.AddSeconds(expiredInSeconds),
                TokenKey = Guid.NewGuid().ToString("N").Substring(0, tokenLenght),
                TokenType = tokenType
            };
            await _dbContext.UserTokens.AddAsync(token);
            await _dbContext.SaveChangesAsync();
            return token;
        }

        public Task<UserToken> GetLatestTokenByTypeAsync(string userName, string tokenType)
        {
            return _dbContext.UserTokens.Where(x => x.TokenType == tokenType && x.UserName == userName).AsNoTracking().OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        }

        public Task DeleteTokenByTypeAsync(string userName, string tokenType)
        {
            var undeletedTokens = _dbContext.UserTokens.Where(x => x.UserName == userName && x.TokenType == tokenType && !x.Deleted);
            foreach (var item in undeletedTokens)
            {
                item.Deleted = true;
            }
            _dbContext.UserTokens.UpdateRange(undeletedTokens);
            return _dbContext.SaveChangesAsync();
        }

        public async Task<UserToken> GetUserTokenAsync(string userName, string tokenKey, string tokenType)
        {
            var token = await _dbContext.UserTokens.Where(x => x.TokenKey == tokenKey
                                                && x.UserName == userName
                                                && x.TokenType == tokenType
                                                && x.ExpiredDate > DateTime.UtcNow).FirstOrDefaultAsync();
            if (token != null)
            {
                token.ClickCount = token.ClickCount + 1;
                if (token.ClickCount > token.MaxClickCount)
                {
                    token.Deleted = true;
                }
                await _dbContext.SaveChangesAsync();
            }

            if (token != null && !token.Deleted)
            {
                return token;
            }
            return null;
        }

        protected override void Update(UserToken requestObject, UserToken targetObject)
        {
            targetObject.Deleted = requestObject.Deleted;
        }
    }
}
