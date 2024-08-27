using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AHI.Infrastructure.Audit.Extension;
using AHI.Infrastructure.Bus.ServiceBus.Extension;
using AHI.Infrastructure.Cache.Redis.Extension;
using AHI.Infrastructure.MultiTenancy.Http.Handler;
using AHI.Infrastructure.Service.Extension;
using AHI.Infrastructure.SystemContext.Enum;
using AHI.Infrastructure.Validation.Extension;
using IdpServer.Application.Client.Command;
using IdpServer.Application.Client.Model;
using IdpServer.Application.Constant;
using IdpServer.Application.Delegating.Handler;
using IdpServer.Application.Handler;
using IdpServer.Application.Pipeline;
using IdpServer.Application.Service;
using IdpServer.Application.Service.Abstraction;
using IdpServer.Application.User.Command;
using MediatR;
using Prometheus;
using System.Net.Http;
using System.Net;
using AHI.Infrastructure.Service.Tag.Extension;
using AHI.Infrastructure.Service.Tag.Enum;

namespace IdpServer.Application.Extension
{
    public static class ApplicationExtension
    {
        const string SERVICE_NAME = "identity-service";
        public static void AddApplicationServices(this IServiceCollection serviceCollection)
        {
            // Add the fluent validator
            serviceCollection.AddApplicationValidator();
            serviceCollection.AddMediatR(typeof(ApplicationExtension).GetTypeInfo().Assembly);
            serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            serviceCollection.AddTransient<SAPCDCAuthenticationHandler>();
            serviceCollection.AddFrameworkServices();
            serviceCollection.AddDynamicValidation();
            serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddScoped<IEmailService, EmailService>();
            serviceCollection.AddScoped<ITokenService, TokenService>();
            serviceCollection.AddScoped<IMasterService, MasterService>();
            serviceCollection.AddScoped<IClientService, ClientService>();
            serviceCollection.AddScoped<ILoginTypeService, LoginTypeService>();
            serviceCollection.AddScoped<IBrokerClientService, BrokerClientService>();
            serviceCollection.AddScoped<IConfigurationService, ConfigurationService>();
            serviceCollection.AddScoped<IAccessControlService, AccessControlService>();
            serviceCollection.AddScoped<ISAPCDCService, SAPCDCService>();

            serviceCollection.AddHttpClient(HttpClients.NOTIFICATION_SERVICE, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration["Api:Notification"]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>().UseHttpClientMetrics();

            serviceCollection.AddHttpClient(HttpClients.MASTER_FUNCTION, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration["Function:Master"]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>().UseHttpClientMetrics();

            serviceCollection.AddHttpClient(HttpClients.USER_FUNCTION, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration["Function:User"]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>();

            serviceCollection.AddHttpClient(HttpClients.ACCESS_CONTROL_SERVICE, (service, config) =>
           {
               var configuration = service.GetRequiredService<IConfiguration>();
               config.BaseAddress = new System.Uri(configuration["Api:AccessControl"]);
               config.Timeout = TimeSpan.FromSeconds(5);
           }).AddHttpMessageHandler<ClientCrendetialAuthentication>().UseHttpClientMetrics();

            serviceCollection.AddHttpClient(HttpClients.CONFIGURATION_SERVICE, (service, config) =>
             {
                 var configuration = service.GetRequiredService<IConfiguration>();
                 config.BaseAddress = new System.Uri(configuration["Api:Configuration"]);
             }).AddHttpMessageHandler<ClientCrendetialAuthentication>().UseHttpClientMetrics();

            serviceCollection.AddHttpClient(HttpClients.MASTER_SERVICE, (service, config) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                config.BaseAddress = new System.Uri(configuration["Api:Master"]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>().UseHttpClientMetrics();

            serviceCollection.AddHttpClient(HttpClients.LOCALIZATION_SERVICE, (service, config) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                config.BaseAddress = new System.Uri(configuration["Api:Localization"]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>().UseHttpClientMetrics();

            serviceCollection.AddHttpClient(HttpClients.SAP_CDC, (service, config) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                config.BaseAddress = new System.Uri(configuration["SAP:Endpoint"]);
            }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            }).AddHttpMessageHandler<Delegating.Handler.SAPCDCAuthenticationHandler>().UseHttpClientMetrics();

            serviceCollection.AddRabbitMQ(SERVICE_NAME);
            serviceCollection.AddRedisCache();
            serviceCollection.AddScoped<IUserInfoService, UserInfoService>();
            serviceCollection.AddSingleton<ILocalizationService, LocalizationService>();

            serviceCollection.AddAuditLogService(AppLevel.SUBSCRIPTION);

            serviceCollection.AddEntityTagService(DatabaseType.SqlServer);
        }

        public static void AddApplicationValidator(this IServiceCollection serviceCollection)
        {
            // All the validator object should be added into DI
            serviceCollection.AddScoped<FluentValidation.IValidator<AddClient>, AddClientValidation>();
            serviceCollection.AddScoped<FluentValidation.IValidator<UpdateClient>, UpdateClientValidation>();
            serviceCollection.AddSingleton<FluentValidation.IValidator<CreateUser>, CreateUserValidation>();
            serviceCollection.AddSingleton<FluentValidation.IValidator<ArchiveClientDto>, ArchiveClientDtoValidation>();
            serviceCollection.AddSingleton<FluentValidation.IValidator<ChangePassword>, ChangePasswordValidation>();
        }
    }
}