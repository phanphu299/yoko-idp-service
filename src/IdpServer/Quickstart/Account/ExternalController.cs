using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using AHI.Infrastructure.Exception;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.SharedKernel.Abstraction;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Quickstart.Model;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdpServer.Application.Constant;
using IdpServer.Application.Extension;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Application.Service.Abstraction;
using IdpServer.Constant;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace IdentityServer4.Quickstart.UI
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class ExternalController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly ILoggerAdapter<ExternalController> _logger;
        private readonly IEventService _events;
        private readonly IIdpUnitOfWork _unitOfWork;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IConfiguration _configuration;
        private readonly IUserInfoService _userInfoService;
        private readonly ITenantContext _tenantContext;
        private readonly IStringLocalizer<ExternalController> _localizer;
        private readonly AHI.Infrastructure.Cache.Abstraction.ICache _cache;
        private readonly IMasterService _masterService;
        private readonly IServiceScopeFactory _scopeFactory;

        public ExternalController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IEventService events,
            ILoggerAdapter<ExternalController> logger,
            IIdpUnitOfWork unitOfWork,
            IDomainEventDispatcher domainEventDispatcher,
            IAuthenticationSchemeProvider schemeProvider,
            IConfiguration configuration,
            IUserInfoService userInfoService,
            IHttpClientFactory httpClientFactory,
            ITenantContext tenantContext,
            IStringLocalizer<ExternalController> localizer,
            AHI.Infrastructure.Cache.Abstraction.ICache cache,
            IMasterService masterService,
            IServiceScopeFactory scopeFactory
            )
        {

            _interaction = interaction;
            _clientStore = clientStore;
            _logger = logger;
            _events = events;
            _unitOfWork = unitOfWork;
            _schemeProvider = schemeProvider;
            _configuration = configuration;
            _userInfoService = userInfoService;
            _localizer = localizer;
            _tenantContext = tenantContext;
            _cache = cache;
            _masterService = masterService;
            _tenantContext = tenantContext;
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public IActionResult Challenge(string provider, string returnUrl)
        {
            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            if (string.IsNullOrEmpty(returnUrl) || (Url.IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == false))
            {
                // user might have clicked on a malicious link - should be logged
                returnUrl = _configuration["DefaultEmail:RedirectUrl"] ?? "~/";
            }

            // start challenge and roundtrip the return URL and scheme
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback)),
                Items =
                    {
                        { "returnUrl", returnUrl },
                        { "scheme", provider },
                    }
            };

            return Challenge(props, provider);
        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            // read external identity from the temporary cookie
            var result = await HttpContext.AuthenticateAsync(IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                throw new GenericCommonException(MessageConstants.COMMON_ERROR_AUTHENTICATION);
            }

            _logger.LogDebug("External claims: {@claims}", result.Principal.Claims.Select(c => $"{c.Type}: {c.Value}"));

            // lookup our user and external provider info
            var (userFirstName, email, provider, claims) = await FindUserFromExternalProviderAsync(result);
            // retrieve return URL
            var returnUrl = result.Properties.Items["returnUrl"] ?? _configuration["DefaultEmail:RedirectUrl"] ?? "~/";
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            var userEntity = await _unitOfWork.Users.FindAsync(email);
            if (userEntity == null)
            {
                await HttpContext.SignOutAsync(IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme);
                var viewModel = new AccessDeniedViewModel()
                {
                    ReturnUrl = returnUrl,
                    Message = _localizer["ERROR.ACCESS_DENIED"]
                };
                return RedirectToAction("AccessDenied", "Account", viewModel);
            }
            var userInfo = await _userInfoService.GetUserInfoAsync(email);
            _tenantContext.RetrieveFromString(userEntity.TenantId, userEntity.SubscriptionId);
            if (userEntity.LoginTypeCode != LoginTypes.AHI_AZURE_AD)
            {
                await RaiseLoginFailureEventAsync(email, userEntity.GetDisplayName(), ViewMessages.InvalidCredentials, context?.ClientId);
                await HttpContext.SignOutAsync(IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme);
                var viewModel = new AccessDeniedViewModel()
                {
                    ReturnUrl = returnUrl,
                    Message = _localizer["ERROR.ACCOUNT_NOT_SUPPORTED"]
                };
                return RedirectToAction("AccessDenied", "Account", viewModel);
            }

            // update the user information
            if (userEntity != null && string.IsNullOrEmpty(userEntity.FirstName))
            {
                await _unitOfWork.BeginTransactionAsync();
                userEntity.FirstName = userFirstName;
                await _unitOfWork.CommitAsync();
                await _userInfoService.ForceUpdateUserInfoAsync(userEntity.UserId, userEntity.Id);

            }
            // this allows us to collect any additional claims or properties
            // for the specific protocols used and store them in the local auth cookie.
            // this is typically used to store data needed for sign out from those protocols.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            ProcessLoginCallbackForOidc(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            var subscriptionId = userEntity.SubscriptionId;
            await HttpContext.SignInAsync(userEntity.Id, localSignInProps, AccountController.IssueUserClaims(userEntity.Id, $"{userEntity.FirstName} {userEntity.LastName}".Trim(), userEntity.TenantId, subscriptionId, null, userEntity.DefaultPage));

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityServer4.IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // check if external login is in the context of an OIDC request
            await RaiseLoginSuccessEventAsync(context?.ClientId, userInfo);
            PostLoginSuccessAsyncFireAndForget(userInfo, _tenantContext);

            if (context != null)
            {
                if (await _clientStore.IsPkceClientAsync(context.ClientId))
                {
                    // if the client is PKCE then we assume it's native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage("Redirect", returnUrl);
                }
            }

            return Redirect(returnUrl);
        }

        private async Task<(string userFirstName, string email, string provider, IEnumerable<Claim> claims)> FindUserFromExternalProviderAsync(AuthenticateResult result)
        {
            var externalUser = result.Principal;

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userEmail = externalUser.FindFirst(ClaimTypes.Email) ??
                            externalUser.FindFirst(JwtClaimTypes.Email) ??
                            externalUser.FindFirst(ClaimTypes.Name) ??
                            externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                            externalUser.FindFirst(JwtClaimTypes.Name) ??
                            externalUser.FindFirst(JwtClaimTypes.Subject) ??
                              throw new EntityNotFoundException("Unknown userId");
            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userEmail);
            var provider = result.Properties.Items["scheme"];
            _ = await _schemeProvider.GetSchemeAsync(provider);

            // find external user
            var email = userEmail.Value;
            var userFirstName = externalUser.FindFirstValue(ClaimTypes.GivenName) ??
                                 externalUser.FindFirstValue(JwtClaimTypes.Name) ?? "";
            return (userFirstName, email, provider, claims);
        }


        private void ProcessLoginCallbackForOidc(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for sign out
            var id_token = externalResult.Properties.GetTokenValue("id_token");
            if (id_token != null)
            {
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = id_token } });
            }
        }

        private async Task RaiseLoginSuccessEventAsync(
                    string clientId,
                    IdpServer.Application.User.Model.UserInfoDto user)
        {
            string upn = user.Upn;
            string userId = user.Id.ToString();
            string firstName = user.FirstName;
            string displayName = user.GetDisplayName();
            var clientInfo = await _clientStore.FindClientByIdAsync(clientId);
            var applicationId = "";
            clientInfo?.Properties.TryGetValue(ClientProperties.ApplicationId, out applicationId);
            await _events.RaiseAsync(new AHIUserLoginSuccessEvent(upn, userId, firstName, clientId: clientId, applicationId: applicationId)
            {
                DisplayName = displayName
            });
        }

        private void PostLoginSuccessAsyncFireAndForget(IdpServer.Application.User.Model.UserInfoDto user,
                    ITenantContext tenantContextSource)
        {
            Task.Run(async () => await PostLoginSuccessAsync(user, tenantContextSource));
        }

        private async Task PostLoginSuccessAsync(
            IdpServer.Application.User.Model.UserInfoDto user,
            ITenantContext tenantContextSource)
        {
            var scope = _scopeFactory.CreateScope();
            var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
            tenantContext.CopyFrom(tenantContextSource);
            AHI.Infrastructure.Cache.Abstraction.ICache cache = scope.ServiceProvider.GetRequiredService<AHI.Infrastructure.Cache.Abstraction.ICache>();
            ILoggerAdapter<ExternalController> logger = scope.ServiceProvider.GetRequiredService<ILoggerAdapter<ExternalController>>();
            IMasterService masterService = scope.ServiceProvider.GetRequiredService<IMasterService>();

            try
            {
                string upn = user.Upn;
                var enableConsent = Convert.ToBoolean(_configuration["EnableConsent"] ?? "false");

                if (enableConsent)
                {
                    foreach (var subscription in user.Subscriptions)
                    {
                        var key = string.Format(CacheKey.REQUIRED_CONSENT, subscription.TenantId, subscription.Id, null, upn);
                        await cache.StoreStringAsync(key, "1", TimeSpan.FromDays(1));
                    }

                    var allProjects = await masterService.GetAllProjectsAsync();
                    foreach (var project in allProjects)
                    {
                        var key = string.Format(CacheKey.REQUIRED_CONSENT, project.TenantId, project.SubscriptionId, project.Id, upn);
                        await cache.StoreStringAsync(key, "1", TimeSpan.FromDays(1));
                    }

                    var defaultKey = string.Format(CacheKey.REQUIRED_CONSENT, user.HomeTenantId, user.HomeSubscriptionId, null, upn);
                    await cache.StoreStringAsync(defaultKey, "1", TimeSpan.FromDays(1));
                    var tenantKey = string.Format(CacheKey.REQUIRED_CONSENT, user.HomeTenantId, null, null, upn);
                    await cache.StoreStringAsync(tenantKey, "1", TimeSpan.FromDays(1));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RaiseLoginSuccessEventAsync failed.");
            }
        }

        private async Task RaiseLoginFailureEventAsync(string upn, string displayName, string error, string clientId)
        {
            var clientInfo = await _clientStore.FindClientByIdAsync(clientId);
            var applicationId = "";
            clientInfo?.Properties.TryGetValue(ClientProperties.ApplicationId, out applicationId);
            await _events.RaiseAsync(new AHIUserLoginFailureEvent(upn, error, clientId: clientId, applicationId: applicationId, displayName: displayName));
        }
    }
}
