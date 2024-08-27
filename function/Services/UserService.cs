using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using Function.Constant;
using Function.Model;
using Function.Service.Abstraction;
using System.Linq;
using AHI.Infrastructure.MultiTenancy.Extension;

namespace Function.Service
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly ITenantContext _tenantContext;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly IConfigurationService _configurationService;
        private readonly IDomainEventDispatcher _domainDispatcher;

        public UserService(IConfiguration configuration,
                            ITenantContext tenantContext,
                            ITokenService tokenService,
                            IDomainEventDispatcher domainDispatcher,
                            IConfigurationService configurationService,
                            IEmailService emailService)
        {
            _configuration = configuration;
            _tenantContext = tenantContext;
            _emailService = emailService;
            _tokenService = tokenService;
            _domainDispatcher = domainDispatcher;
            _configurationService = configurationService;
        }

        public async Task ScanPasswordExpirationAsync()
        {
            var connectionString = _configuration["ConnectionStrings:Default"];
            IEnumerable<IdentityUserDto> users;
            var usersToExpires = new List<string>();
            var expireIn = int.Parse(_configuration["PasswordExpireIn"] ?? "14");
            string[] keys = {   SystemConfigKeys.PASSWORD_EXPIRATION_ENABLED,
                                    SystemConfigKeys.PASSWORD_EXPIRATION_DAY};
            using (var connection = new SqlConnection(connectionString))
            {
                Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
                users = await connection.QueryAsync<IdentityUserDto>(@"SELECT upn, tenant_id , subscription_id, 
                                                                        (SELECT TOP 1 created_utc FROM password_histories WITH (NOLOCK) WHERE upn = users.upn ORDER BY created_utc DESC) as LastPasswordChangeUtc 
                                                                        FROM users WITH (NOLOCK) 
                                                                        WHERE deleted = 0 and login_type_code = 'ahi-local' and [password] IS NOT NULL");
                await connection.CloseAsync();
            }

            var userGroups = users.GroupBy(x => new { x.TenantId, x.SubscriptionId });
            foreach (var userGroup in userGroups)
            {
                _tenantContext.RetrieveFromString(userGroup.Key.TenantId, userGroup.Key.SubscriptionId);
                var systemConfigs = await _configurationService.GetConfigsAsync(string.Join(',', keys));

                var expireEnabled = Boolean.Parse(systemConfigs.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_EXPIRATION_ENABLED)?.Value ?? DefaultOptions.PASSWORD_EXPIRATION_ENABLED);
                var expireDay = int.Parse(systemConfigs.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_EXPIRATION_DAY)?.Value ?? DefaultOptions.PASSWORD_EXPIRATION_DAY);

                if (expireEnabled)
                {
                    foreach (var user in userGroup.AsEnumerable())
                    {
                        if (user.LastPasswordChangeUtc.AddDays(expireDay) <= DateTime.UtcNow)
                        {
                            usersToExpires.Add(user.Upn);
                        }

                        if (user.LastPasswordChangeUtc.AddDays(expireDay).AddDays(-1 * expireIn).Date == DateTime.UtcNow.Date)
                        {
                            var customField = new Dictionary<string, object>()
                            {
                                { "Email", user.Upn },
                                { "PasswordExpireIn", expireIn }
                            };
                            _ = _emailService.SendEmailAsync(user.Upn, EmailTypeCodes.ChangePasswordRemind, customField);
                        }
                    }
                }
            }

            if (usersToExpires.Any())
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.ExecuteAsync(@"UPDATE users SET required_change_password = 1 WHERE upn IN @Users", new { Users = usersToExpires });
                    await connection.CloseAsync();
                }
            }
        }
    }
}
