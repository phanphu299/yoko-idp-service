using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Persistence.Repository;
using IdpServer.Persistence.Context;
using IdpServer.Application.DbConnections;
using IdpServer.Persistence.DbConnections;
using AHI.Infrastructure.Service.Tag.Extension;

namespace IdpServer.Persistence.Extension
{
    public static class PersistenceExtensions
    {
        public static void AddPersistenceService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContextPool<IdpDbContext>((service, option) =>
            {
                var configuration = service.GetService<IConfiguration>();
                var connectionString = configuration["ConnectionStrings:Default"];
                option.UseSqlServer(connectionString);
            });
            // add other services like repository, application services
            serviceCollection.AddScoped<IUserRepository, UserRepository>();
            serviceCollection.AddScoped<IUserTokenRepository, UserTokenRepository>();
            serviceCollection.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            serviceCollection.AddScoped<IClientRoleRepository, ClientRoleRepository>();
            serviceCollection.AddScoped<IProjectClientRepository, ProjectClientRepository>();
            serviceCollection.AddScoped<IClientRepository, ClientRepository>();
            serviceCollection.AddScoped<IIdpUnitOfWork, IdpUnitOfWork>();
            serviceCollection.AddScoped<ITimezoneRepository, TimezoneRepository>();
            serviceCollection.AddScoped<ILoginTypeRepository, LoginTypeRepository>();
            serviceCollection.AddScoped<IBrokerClientRepository, BrokerClientRepository>();
            serviceCollection.AddScoped<IPersistedGrantRepository, PersistedGrantRepository>();

            serviceCollection.AddScoped<IDbConnectionFactory, WriteDbConnectionFactory>();
            serviceCollection.AddScoped<IWriteDbConnectionFactory, WriteDbConnectionFactory>();
            serviceCollection.AddScoped<IReadDbConnectionFactory, ReadDbConnectionFactory>();
            serviceCollection.AddScoped<IDbConnectionResolver, DbConnectionResolver>();
            serviceCollection.AddEntityTagRepository(typeof(IdpDbContext));
        }
    }
}
