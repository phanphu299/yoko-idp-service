using System;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Function.Model;
using Function.Service.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.Cache.Abstraction;

namespace Function.Service
{
    public class BrokerClientsService : IBrokerClientsService
    {
        private readonly IConfiguration _configuration;
        private readonly ICache _cache;

        public BrokerClientsService(IConfiguration configuration, ICache cache)
        {
            _configuration = configuration;
            _cache = cache;
        }

        public async Task<AuthenticationResultDto> AuthenticateAsync(AuthenticateBrokerClientRequest request)
        {
            var key = $"idp_broker_client_{request.Username.CalculateMd5Hash()}_{request.Password.CalculateMd5Hash()}_auth";
            var authInfo = await _cache.GetAsync<AuthenticationResultDto>(key);

            if (authInfo == null)
            {
                authInfo = new AuthenticationResultDto()
                {
                    IsSuperUser = false,
                    Result = "deny"
                };
                var connectionString = _configuration["ConnectionStrings:Default"];
                using (var connection = new SqlConnection(connectionString))
                {
                    var query = @"SELECT expired_utc
                                FROM [broker_clients] WITH(NOLOCK) 
                                WHERE [deleted] = 0 
                                AND [id] = @Username
                                AND [password] = @Password
                                AND [expired_utc] >= @UtcNow;";
                    await connection.OpenAsync();
                    var expiredDate = await connection.QueryFirstOrDefaultAsync<DateTime?>(query, new { request.Username, request.Password, UtcNow = DateTime.UtcNow });
                    await connection.CloseAsync();

                    TimeSpan timespan = TimeSpan.MaxValue;
                    if (expiredDate.HasValue)
                    {
                        authInfo.Result = "allow";
                        timespan = expiredDate.Value - DateTime.UtcNow;
                    }

                    await _cache.StoreAsync(key, authInfo, timespan);
                }
            }
            return authInfo;
        }
    }
}
