using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using IdpServer.Application.DbConnections;

namespace IdpServer.Persistence.DbConnections
{
    public class ReadDbConnectionFactory : IReadDbConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private readonly ITenantContext _tenantContext;
        private readonly ILoggerAdapter<ReadDbConnectionFactory> _logger;

        public ReadDbConnectionFactory(IConfiguration configuration, ITenantContext tenantContext, ILoggerAdapter<ReadDbConnectionFactory> logger)
        {
            _configuration = configuration;
            _tenantContext = tenantContext;
            _logger = logger;
        }

        public IDbConnection CreateConnection()
        {
            var connectionString = _configuration["ConnectionStrings:ReadOnly"];
            if (string.IsNullOrEmpty(connectionString))
                connectionString = _configuration["ConnectionStrings:Default"];

            var con = new SqlConnection(connectionString);
            try
            {
                con.Open();
                return con;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ReadDbConnectionFactory OpenFail. SubscriptionId={_tenantContext.SubscriptionId}");
                throw;
            }
        }
    }
}