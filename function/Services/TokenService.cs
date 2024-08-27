using System;
using System.Threading.Tasks;
using Dapper;
using Function.Service.Abstraction;
using Microsoft.Extensions.Configuration;
using Function.Model;
using Microsoft.Data.SqlClient;
using AHI.Infrastructure.MultiTenancy.Abstraction;

namespace Function.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ITenantContext _tenantContext;
        private readonly IEmailService _emailService;

        public TokenService(IConfiguration configuration,
                            ITenantContext tenantContext,
                            IEmailService emailService)
        {
            _configuration = configuration;
            _tenantContext = tenantContext;
            _emailService = emailService;
        }

        public async Task<UserTokenDto> GenerateTokenAsync(string userName, string tokenType, string redirectUrl = "", int tokenLenght = 32, double expiredInSeconds = 86400)
        {
            var deleteTokenCommand = "UPDATE user_tokens SET deleted = 1 WHERE user_name = @userName AND token_type = @tokenType";
            var token = new UserTokenDto()
            {
                UserName = userName,
                RedirectUrl = redirectUrl,
                CreatedDate = DateTime.UtcNow,
                ExpiredDate = DateTime.UtcNow.AddSeconds(expiredInSeconds),
                TokenKey = Guid.NewGuid().ToString("N").Substring(0, tokenLenght),
                TokenType = tokenType
            };
            var createTokenCommand = @"INSERT INTO user_tokens (user_name,token_key,token_type,created_date,expired_date,redirect_url,deleted)
                                        VALUES (@userName,@tokenKey,@tokenType,@createdDate,@expiredDate,@redirectUrl,0)";
            using (var connection = new SqlConnection(_configuration["ConnectionStrings:Default"]))
            {
                await connection.ExecuteAsync(deleteTokenCommand, new { userName = userName, tokenType = tokenType }, commandTimeout: 600);
                await connection.ExecuteAsync(createTokenCommand,
                                                new
                                                {
                                                    userName = token.UserName,
                                                    tokenKey = token.TokenKey,
                                                    tokenType = token.TokenType,
                                                    createdDate = token.CreatedDate,
                                                    expiredDate = token.ExpiredDate,
                                                    redirectUrl = token.RedirectUrl
                                                },
                                                commandTimeout: 600);
                await connection.CloseAsync();
            }
            return token;
        }
    }
}
