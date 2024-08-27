using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AHI.Infrastructure.Exception.Filter;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.MultiTenancy.Middleware;
using AHI.Infrastructure.UserContext;
using AHI.Infrastructure.UserContext.Extension;
using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdpServer.Application.Extension;
using IdpServer.Application.Model;
using IdpServer.Application.Service;
using IdpServer.Customized;
using IdpServer.Localization;
using IdpServer.Model;
using IdpServer.Persistence.Extension;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Hosting;
using Prometheus;
using Prometheus.SystemMetrics;
using StackExchange.Redis;
using static AHI.Infrastructure.SharedKernel.Extension.Constant;
using IClientStore = IdentityServer4.Stores.IClientStore;
using IdpServer.Customized.Dummy;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.CookiePolicy;
using AHI.Infrastructure.Audit.Extension;
using IdpServer.Application.Service.Abstraction;

namespace IdpServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment;
        const string defaultCulture = "en-US";

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorization();
            services.AddControllersWithViews(option =>
            {
                option.ExceptionHandling();
            })
            .AddNewtonsoftJson(option =>
            {
                option.SerializerSettings.NullValueHandling = JsonSerializerSetting.NullValueHandling;
                option.SerializerSettings.DateFormatString = JsonSerializerSetting.DateFormatString;
                option.SerializerSettings.ReferenceLoopHandling = JsonSerializerSetting.ReferenceLoopHandling;
                option.SerializerSettings.DateParseHandling = JsonSerializerSetting.DateParseHandling;
            })
           .AddViewLocalization();
            var redisConfigOptions = new ConfigurationOptions
            {
                EndPoints = { Configuration["Redis:Host"] },
                Password = Configuration["Redis:Password"],
                Ssl = bool.Parse(Configuration["Redis:Ssl"])
            };
            services.AddStackExchangeRedisCache(config =>
            {
                config.ConfigurationOptions = redisConfigOptions;
            });

            services.AddSession(options =>
            {
                options.Cookie.Name = ".AssetHealthInsights.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(15);
            });

            services.AddMultiTenantService();
            services.AddUserContextService();
            services.AddPersistenceService();
            services.AddNotification();
            services.AddApplicationServices();
            services.AddSingleton<IPasswordHasher<UserDto>, PasswordHasher<UserDto>>();
            services.AddTransient(typeof(IEventSink), typeof(AuthenticationEventSink));
            
            services.AddScoped<IUserStore<UserDto>, DummyUserStore<UserDto>>();
            services.AddSingleton<ILookupNormalizer, DummyLookupNormalizer>();
            services.AddSingleton<IdentityErrorDescriber>();
            services.AddSingleton<AuthenticatorTokenProvider<UserDto>>();
            services.AddScoped<UserManager<UserDto>>(service =>
            {
                var userStore = service.GetRequiredService<IUserStore<UserDto>>();
                var option = service.GetRequiredService<IOptions<IdentityOptions>>();
                var passwordHasher = service.GetRequiredService<IPasswordHasher<UserDto>>();
                var userValidators = service.GetRequiredService<IEnumerable<IUserValidator<UserDto>>>();
                var passwordValidators = service.GetRequiredService<IEnumerable<IPasswordValidator<UserDto>>>();
                var keyNormalizer = service.GetRequiredService<ILookupNormalizer>();
                var errors = service.GetRequiredService<IdentityErrorDescriber>();
                var logger = service.GetRequiredService<ILogger<UserManager<UserDto>>>();
                var tokenValidator = service.GetRequiredService<AuthenticatorTokenProvider<UserDto>>();
                var userManager = new UserManager<UserDto>(userStore, option, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, service, logger);
                userManager.RegisterTokenProvider(TokenOptions.DefaultAuthenticatorProvider, tokenValidator);
                return userManager;
            });
            var connectionString = Configuration["ConnectionStrings:Default"];
            var publicOrigin = Configuration["PublicOrigin"];
            var idpBuilder = services.AddIdentityServer(option =>
            {
                if (!string.IsNullOrEmpty(publicOrigin))
                {
                    option.PublicOrigin = publicOrigin;
                }
                option.IssuerUri = "idp";
                option.Events = new IdentityServer4.Configuration.EventsOptions()
                {
                    RaiseErrorEvents = true,
                    RaiseFailureEvents = true,
                    RaiseInformationEvents = true,
                    RaiseSuccessEvents = true
                };
                // https://github.com/IdentityServer/IdentityServer4/issues/4950
                // by default, the length is 300
                option.InputLengthRestrictions.Scope = System.Convert.ToInt32(Configuration["ScopeMaxLength"] ?? "1000");
            })
                // add credentials
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString);
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString);
                    options.EnableTokenCleanup = true;
                    options.TokenCleanupInterval = 3600; // interval in seconds (default is 3600)
                })
                .AddProfileService<CustomizedProfileService>()
                .AddExtensionGrantValidator<UserSwitchTenantGrantValidator>();
            idpBuilder.AddClientStore<CacheClientStore>();
            idpBuilder.AddPersistedGrantStore<CachePersistedGrantStore>();
            idpBuilder.AddResourceStore<MemoryCacheResourceStore>();
            idpBuilder.AddConfigurationStore();
            idpBuilder.Services.AddTransient<ICachePersistedGrantStore, CachePersistedGrantStore>();
            idpBuilder.Services.AddTransient<IClientStore, CacheClientStore>();
            idpBuilder.Services.AddTransient<IResourceStore, MemoryCacheResourceStore>();

            X509Certificate2 certificate = new X509Certificate2(Path.Combine("AppData", Configuration["Certificate:Name"]), Configuration["Certificate:Password"]);
            idpBuilder.AddSigningCredential(certificate);

            
            var redis = ConnectionMultiplexer.Connect(redisConfigOptions);
            services.AddDataProtection()
                .SetApplicationName("idp")
                .PersistKeysToStackExchangeRedis(redis, "idp-data-protection")
                .ProtectKeysWithCertificate(certificate);

            var externalProviders = new List<ExternalProviderConfig>();

            Configuration.GetSection("Providers").Bind(externalProviders);

            var builder = services.AddAuthentication();
            builder.AddIdentityServerAuthentication("oidc",
                jwtTokenOption =>
                {
                    jwtTokenOption.Authority = Configuration["Authentication:Authority"];
                    jwtTokenOption.RequireHttpsMetadata = Configuration["Authentication:Authority"].StartsWith("https");
                    jwtTokenOption.TokenValidationParameters.ValidateAudience = false;
                    jwtTokenOption.ClaimsIssuer = Configuration["Authentication:Issuer"];
                }, referenceTokenOption =>
                {
                    referenceTokenOption.IntrospectionEndpoint = Configuration["Authentication:IntrospectionEndpoint"];
                    referenceTokenOption.ClientId = Configuration["Authentication:ApiScopeName"];
                    referenceTokenOption.ClientSecret = Configuration["Authentication:ApiScopeSecret"];
                    referenceTokenOption.ClaimsIssuer = Configuration["Authentication:Issuer"];
                    referenceTokenOption.SaveToken = true;
                });
            foreach (var provider in externalProviders)
            {
                if (provider.Name.StartsWith("aad-", ignoreCase: true, CultureInfo.InvariantCulture))
                {
                    builder.AddOpenIdConnect(provider.Name, provider.Description, options =>
                                                    {
                                                        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                                                        options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                                                        options.ClientId = provider.ClientId;
                                                        options.Authority = provider.Authority;
                                                        options.ClientSecret = provider.ClientSecret;
                                                        options.ResponseType = "code";
                                                        options.RequireHttpsMetadata = true;
                                                        options.CallbackPath = $"/signin-{provider.Name}";
                                                        options.SignedOutCallbackPath = $"/signout-callback-{provider.Name}";
                                                        options.RemoteSignOutPath = $"/signout-{provider.Name}";
                                                        options.Events.OnRemoteFailure = HandleRemoteFailureAsync;
                                                    });
                }
                else if (provider.Name.StartsWith("google-", ignoreCase: true, CultureInfo.InvariantCulture))
                {
                    builder.AddGoogle(options =>
                                           {
                                               options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                                               options.ClientId = provider.ClientId;
                                               options.ClientSecret = provider.ClientSecret;
                                               options.Events.OnRemoteFailure = HandleRemoteFailureAsync;
                                           });
                }
                else if (provider.Name.StartsWith("facebook-", ignoreCase: true, CultureInfo.InvariantCulture))
                {
                    builder.AddFacebook(options =>
                                           {
                                               options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                                               options.ClientId = provider.ClientId;
                                               options.ClientSecret = provider.ClientSecret;
                                               options.Events.OnRemoteFailure = HandleRemoteFailureAsync;
                                           });
                }
            }
            services.AddMemoryCache();
            services.AddSystemMetrics();

            services.AddLocalization();
            services.AddSingleton<TranslationService>();
            services.AddSingleton<LocalizationMiddleware>();
            services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            var cookieSecurePolicy = Convert.ToBoolean(Configuration["CookieSecurePolicy:Enabled"] ?? "true");
            var option = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Lax,
                HttpOnly = HttpOnlyPolicy.Always
            };
            if (cookieSecurePolicy)
            {
                // enable cookie policy for https
                option.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                option.Secure = CookieSecurePolicy.Always;
                option.HttpOnly = HttpOnlyPolicy.None;
            }
            app.UseCookiePolicy(option);
            app.UseRouting();
            app.UseHttpMetrics();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseWhen(
                context => context.Request.Path.HasValue && context.Request.Path.Value.StartsWith("/idp")
                && !context.Request.Path.Value.StartsWith("/idp/clients/info")
                && !context.Request.Path.Value.StartsWith("/idp/brokerclients")
                && !context.Request.Path.Value.StartsWith("/idp/users")
                && !context.Request.Path.Value.StartsWith("/metrics")
                && !(context.Request.Query.TryGetValue("excludeUserContext", out var excludeUserContext)
                    && excludeUserContext.Count == 1 && excludeUserContext[0] == "true"),
                builder =>
                {
                    builder.UseMiddleware<MultiTenancyMiddleware>();
                    builder.UseMiddleware<UserContextMiddleware>();
                }
            );
            //https://github.com/IdentityServer/IdentityServer4/issues/860
            app.UseForwardedHeaders(new ForwardedHeadersOptions()
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto
            });
            app.Use((context, next) =>
            {
                if (context.Request.Host.Host.EndsWith("azurewebsites.net") || context.Request.Host.Host.EndsWith("apps.yokogawa.com"))
                {
                    // this is request from FE
                    context.Request.Scheme = "https";
                }
                return next();
            });
            app.UseIdentityServer();
            app.UseMiddleware<LocalizationMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapMetrics();
                endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}");
            });
        }

        private Task HandleRemoteFailureAsync(RemoteFailureContext context)
        {
            var returnUrl = context.Properties.Items["returnUrl"] ?? Configuration["DefaultEmail:RedirectUrl"] ?? "~/";
            context.Response.Redirect($"/Account/Login?ReturnUrl={System.Web.HttpUtility.UrlEncode(returnUrl)}");
            context.HandleResponse();
            return Task.CompletedTask;
        }
    }
}
