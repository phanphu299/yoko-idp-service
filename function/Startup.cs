using Function.Service;
using Function.Service.Abstraction;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using AHI.Infrastructure.Bus.ServiceBus.Extension;
using Microsoft.Extensions.Configuration;
using System;
using AHI.Infrastructure.MultiTenancy.Http.Handler;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.OpenTelemetry;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;
using OpenTelemetry.Logs;
using Function.Constant;
using AHI.Infrastructure.Cache.Redis.Extension;
using AHI.Infrastructure.Service.Tag.Extension;

[assembly: FunctionsStartup(typeof(Identity.Function.Startup))]
namespace Identity.Function
{
    public class Startup : FunctionsStartup
    {
        const string SERVICE_NAME = "identity-function";
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient(HttpClients.NOTIFICATION_SERVICE, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration["Api:Notification"]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>();
            builder.Services.AddHttpClient(HttpClients.CONFIGURATION_SERVICE, (service, client) =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(configuration["Api:Configuration"]);
            }).AddHttpMessageHandler<ClientCrendetialAuthentication>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IBrokerClientsService, BrokerClientsService>();
            builder.Services.AddScoped<IConfigurationService, ConfigurationService>();

            builder.Services.AddRabbitMQ(SERVICE_NAME);
            builder.Services.AddMultiTenantService();
            builder.Services.AddRedisCache();
            // builder.Services.AddApplicationInsightsTelemetry();

            // for debugging purpose
            builder.Services.AddOtelTracingService(SERVICE_NAME, typeof(Startup).Assembly.GetName().Version.ToString());
            // for production, no need to output to console.
            // will adapt with open telemetry collector in the future.
            //serviceCollection.AddOtelTracingService("project-service", typeof(ApplicationExtension).Assembly.GetName().Version.ToString());
            builder.Services.AddLogging(builder =>
            {
                builder.AddOpenTelemetry(option =>
               {
                   option.SetResourceBuilder(
                   ResourceBuilder.CreateDefault()
                       .AddService(SERVICE_NAME, typeof(Startup).Assembly.GetName().Version.ToString()));
                   //option.AddConsoleExporter();
                   option.AddOtlpExporter(oltp =>
                   {
                       oltp.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                   });
               });
            });
            builder.Services.AddEntityTagService(AHI.Infrastructure.Service.Tag.Enum.DatabaseType.SqlServer);
        }
    }
}