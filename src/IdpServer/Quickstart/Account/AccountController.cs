// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using AHI.Infrastructure.Exception;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Quickstart.Model;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdpServer.Application.Constant;
using IdpServer.Application.Enum;
using IdpServer.Application.Model;
using IdpServer.Application.Service.Abstraction;
using IdpServer.Constant;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using AHI.Infrastructure.Cache.Abstraction;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using ITokenService = IdpServer.Application.Service.Abstraction.ITokenService;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using System.Globalization;
using System.Net;
using AHI.Infrastructure.MultiTenancy.Extension;
using IdpServer.Application.Event;
using Microsoft.EntityFrameworkCore;
using AHI.Infrastructure.Audit.Service.Abstraction;
using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.UserContext.Abstraction;
using IdpServer.Application.Extension;
using AHI.Infrastructure.SharedKernel.Extension;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Threading;
using System.Net.Http;

namespace IdentityServer4.Quickstart.UI
{
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identity server for validation and context retrieval
    /// </summary>
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IConfiguration _configuration;
        private readonly IEventService _events;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly IMasterService _masterService;
        private readonly ITenantContext _tenantContext;
        private readonly IPasswordHasher<UserDto> _hasher;
        private readonly ILoggerAdapter<AccountController> _logger;
        private readonly ICache _cache;
        private readonly IAntiforgery _antiforgery;
        private readonly IUserInfoService _userInfoService;
        private readonly IStringLocalizer<AccountController> _localizer;
        private readonly UserManager<UserDto> _userManager;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContext _userContext;
        private readonly ISAPCDCService _sapCDCService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        public AccountController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IConfiguration configuration,
            IEventService events,
            IUserService userService,
            IEmailService emailService,
            ITokenService tokenService,
            IMasterService masterService,
            ITenantContext tenantContext,
            ICache cache,
            ILoggerAdapter<AccountController> logger,
            IPasswordHasher<UserDto> hasher,
            IAntiforgery antiforgery,
            IUserInfoService userInfoService,
            IStringLocalizer<AccountController> localizer,
            IAuditLogService auditLogService,
            IUserContext userContext,
            ISAPCDCService sapCDCService,
            UserManager<UserDto> userManager,
            IHttpClientFactory httpClientFactory,
            IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _userService = userService;
            _emailService = emailService;
            _hasher = hasher;
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _tokenService = tokenService;
            _masterService = masterService;
            _tenantContext = tenantContext;
            _cache = cache;
            _logger = logger;
            _antiforgery = antiforgery;
            _userInfoService = userInfoService;
            _localizer = localizer;
            _userManager = userManager;
            _auditLogService = auditLogService;
            _userContext = userContext;
            _sapCDCService = sapCDCService;
            _scopeFactory = scopeFactory;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        [ImportModelState]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page
            var vm = await BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("Challenge", "External", new { provider = vm.ExternalLoginScheme, returnUrl });
            }
            var lockModel = HttpContext.Session.Get(ClientProperties.LockedOut)?.Deserialize<LockedOutModel>();
            if (lockModel != null && lockModel.IsLocked && (lockModel.ExpiredTime > DateTime.UtcNow))
            {
                vm.LockedOut = lockModel.IsLocked;
                ModelState.AddModelError(string.Empty, _localizer["ERROR.IP_LOGIN_LOCKED", lockModel.Duration]);
            }
            else
            {
                HttpContext.Session.Remove(ClientProperties.LockedOut);
            }
            return View(vm);
        }

        [HttpPost]
        [ExportModelState]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            Guid correlationId = Guid.NewGuid();
            
            _logger.LogInformation($"{correlationId} Login - Step 1: validate reCaptcha");
            if (!await IsValidRecaptchaAsync(HttpContext, correlationId))
            {
                _logger.LogError("reCaptcha validation failed");
                return RedirectToAction(nameof(Login), new { returnUrl = model.ReturnUrl });
            }
            _logger.LogInformation($"{correlationId} Login - Step 2: _interaction.GetAuthorizationContextAsync");
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            if (!string.IsNullOrEmpty(model.Username))
            {
                model.Username = model.Username.Trim();
            }
            var loginIpAddress = GetRequestIp();

            if (ModelState.IsValid)
            {
                _logger.LogInformation($"{correlationId} Login - Step 3: ModelState.IsValid");
                if (!_emailService.IsValid(model.Username))
                {
                    ModelState.AddModelError("Username", _localizer["ERROR.VALIDATION.INVALID_EMAIL"]);
                }
                else
                {
                    _logger.LogInformation($"{correlationId} Login - Step 3.1: _cache.GetStringAsync(loginAttemptCacheKey)");
                    var loginAttemptCacheKey = $"idp_attempt_{loginIpAddress}_{model.Username}";
                    var loginAttemptString = await _cache.GetStringAsync(loginAttemptCacheKey);
                    _logger.LogInformation($"_cache.GetStringAsync: key:{loginAttemptCacheKey}, value:{loginAttemptString}");
                    var loginAttempt = 0;
                    if (!string.IsNullOrEmpty(loginAttemptString))
                    {
                        loginAttempt = Convert.ToInt32(loginAttemptString);
                    }
                    loginAttempt++;

                    _logger.LogInformation($"{correlationId} Login - Step 3.2: userService.FindByUpnAsync(model.Username)");
                    var user = await _userService.FindByUpnAsync(model.Username);
                    if (user != null)
                    {
                        _logger.LogInformation($"{correlationId} Login - Step 3.3: _userService.GetPasswordPolicyAsync");
                        _tenantContext.RetrieveFromString(user.TenantId, user.SubscriptionId);
                        var policies = await _userService.GetPasswordPolicyAsync(user.TenantId, user.SubscriptionId);
                        var lockoutEnabled = Boolean.Parse(policies.FirstOrDefault(x => x.Key == SystemConfigKeys.ACCOUNT_LOCKOUT_ENABLED).Value);
                        var lockoutDuration = int.Parse(policies.FirstOrDefault(x => x.Key == SystemConfigKeys.ACCOUNT_LOCKOUT_DURATION).Value);
                        var lockoutAttempt = int.Parse(policies.FirstOrDefault(x => x.Key == SystemConfigKeys.ACCOUNT_LOCKOUT_ATTEMPT).Value);

                        if (lockoutEnabled && lockoutDuration > 0 && lockoutAttempt > 0
                            && (loginAttempt > lockoutAttempt)
                            && user.LoginTypeCode == LoginTypes.AHI_LOCAL)
                        {
                            _logger.LogInformation($"{correlationId} Login - Step 3.5: RaiseLoginFailureEventAsync(model.Username, user.GetDisplayName()");
                            // just temporary lock the request
                            await RaiseLoginFailureEventAsync(model.Username, user.GetDisplayName(), $"{user.Id} IP login locked", context?.ClientId);
                            ModelState.AddModelError(string.Empty, _localizer["ERROR.IP_LOGIN_LOCKED", lockoutDuration]);
                            model.LockedOut = true;
                            //var viewModel = await BuildLoginViewModelAsync(model);
                            return RedirectToAction(nameof(Login), new { returnUrl = model.ReturnUrl });
                        }

                        _logger.LogInformation($"{correlationId} Login - Step 3.6: _userInfoService.GetUserInfoAsync(user.Upn)");
                        var verificationResult = PasswordVerificationResult.Failed;
                        switch (user.LoginTypeCode)
                        {
                            case LoginTypes.AHI_LOCAL:
                                if (user.Password != null)
                                    verificationResult = _hasher.VerifyHashedPassword(user, user.Password, model.Password);
                                break;
                            case LoginTypes.SAP_CDC:
                                var response = await _sapCDCService.LoginAsync(model.Username, model.Password);
                                if (response.StatusCode == (int)HttpStatusCode.OK)
                                {
                                    verificationResult = PasswordVerificationResult.Success;
                                }
                                break;
                            default:
                                break;
                        }

                        if (verificationResult == PasswordVerificationResult.Success)
                        {
                            _logger.LogInformation($"{correlationId} Login - Step 3.6.1: verificationResult == PasswordVerificationResult.Success");
                            await _cache.DeleteAsync(loginAttemptCacheKey);
                            HttpContext.Session.Remove(ClientProperties.LockedOut);
                            //pre caching user info
                            _ = Task.Run(async () => await _userInfoService.GetUserInfoAsync(user.Upn));

                            if (user.RequiredChangePassword)
                            {
                                if (user.LoginTypeCode == LoginTypes.AHI_LOCAL)
                                {
                                    _logger.LogInformation($"{correlationId} Login - Step 3.6.1.1: _tokenService.GenerateTokenAsync");
                                    var changePassToken = await _tokenService.GenerateTokenAsync(user.Upn, TokenTypeEnum.ChangePassword, model.ReturnUrl);

                                    return RedirectToAction("ChangePassword", new
                                    {
                                        username = user.Upn,
                                        token = changePassToken.TokenKey,
                                        returnUrl = model.ReturnUrl
                                    });
                                }
                                else if (user.LoginTypeCode == LoginTypes.SAP_CDC)
                                {
                                    _logger.LogInformation($"{correlationId} Login - Step 3.6.1.2: _tokenService.GenerateTokenAsync");
                                    var setPassToken = await _tokenService.GenerateTokenAsync(user.Upn, TokenTypeEnum.ResetPassword, model.ReturnUrl);

                                    return RedirectToAction("SetPassword", new
                                    {
                                        username = user.Upn,
                                        token = setPassToken.TokenKey,
                                        type = TokenTypeEnum.ResetPassword
                                    });
                                }
                            }

                            if (user.MFA)
                            {
                                _logger.LogInformation($"{correlationId} Login - Step 3.6.1.3: user.MFA");
                                if (user.SetupMFA)
                                {
                                    return RedirectToAction(nameof(Verification), new { username = user.Upn, returnUrl = model.ReturnUrl });
                                }
                                else
                                {
                                    return RedirectToAction(nameof(EnableAuthenticator), new { username = user.Upn, returnUrl = model.ReturnUrl });
                                }
                            }

                            // For case delete user subscription
                            _logger.LogInformation($"{correlationId} Login - Step 3.6.1.4: _masterService.GetAllSubscriptionsAsync");
                            var allSub = await _masterService.GetAllSubscriptionsAsync();
                            var userSubscriptions = await _userInfoService.GetUserSubscriptionsAsync(user.UserId);
                            if (!allSub.Any(x => x.Id == user.SubscriptionId && x.TenantResourceId == user.TenantId))
                            {
                                ModelState.AddModelError(string.Empty, _localizer["ERROR.VALIDATION.INVALID_LOGIN_INFO"]);
                                return RedirectToAction(nameof(Login), new { returnUrl = model.ReturnUrl });
                            }

                            if (userSubscriptions.Select(x => x.TenantId).Distinct().Count() > 1)
                            {
                                return RedirectToAction(nameof(SelectTenant), new { username = user.Upn, returnUrl = model.ReturnUrl });
                            }
                            else
                            {
                                _logger.LogInformation($"{correlationId} Login - Step 3.6.1.5: Pre RaiseLoginSuccessEventAsync");
                                var userSubscription = userSubscriptions.FirstOrDefault(x => x.Id == user.SubscriptionId);
                                if (userSubscription == null)
                                {
                                    // need to fallback into default first subscription
                                    userSubscription = userSubscriptions.FirstOrDefault();
                                }
                                var tenantId = userSubscription == null ? user.TenantId : userSubscription.TenantId;
                                var subscriptionId = userSubscription == null ? user.SubscriptionId : userSubscription.Id;
                                _tenantContext.RetrieveFromString(tenantId, subscriptionId);

                                _logger.LogInformation($"{correlationId} Login - Step 3.6.1.5.1: RaiseLoginSuccessEventAsync");
                                await RaiseLoginSuccessEventAsync(context?.ClientId, user, correlationId);

                                _logger.LogInformation($"{correlationId} Login - Step 3.6.1.6: PostLoginSuccessAsyncFireAndForget");
                                PostLoginSuccessAsyncFireAndForget(user, userSubscriptions, _tenantContext);
                                AuthenticationProperties props = null;

                                _logger.LogInformation($"{correlationId} Login - Step 3.6.1.7: HttpContext.SignInAsync");
                                await HttpContext.SignInAsync(user.Upn, props, IssueUserClaims(user.Upn, $"{user.FirstName} {user.LastName}".Trim(), tenantId, subscriptionId, null, user.DefaultPage));

                                _logger.LogInformation($"{correlationId} Login - Step 3.6.1.8: UpdateUserLanguageAsync");
                                await UpdateUserLanguageAsync(user.Id);

                                string returnUrl = await BuildRedirectUrlAsync(model.ReturnUrl);
                                _logger.LogInformation($"{correlationId} Login - Step 3.6.1.9: this.LoadingPage, returnUrl: {returnUrl}");

                                return this.LoadingPage("Redirect", returnUrl);
                            }
                        }
                        else
                        {
                            _logger.LogInformation($"{correlationId} Login - Step 3.6.2: _cache.StoreStringAsync(loginAttemptCacheKey, loginAttempt.ToString()");
                            await _cache.StoreStringAsync(loginAttemptCacheKey, loginAttempt.ToString(), TimeSpan.FromMinutes(lockoutDuration));
                            
                            if (lockoutEnabled && lockoutDuration > 0 && lockoutAttempt > 0 && user.LoginTypeCode == LoginTypes.AHI_LOCAL)
                            {
                                if ((loginAttempt + 1) > lockoutAttempt)
                                {
                                    // just temporary lock the request
                                    _logger.LogInformation($"{correlationId} Login - Step 3.6.2.1: RaiseLoginFailureEventAsync");
                                    await RaiseLoginFailureEventAsync(model.Username, user.GetDisplayName(), $"{user.Id} IP login locked", context?.ClientId);
                                    ModelState.AddModelError(string.Empty, _localizer["ERROR.IP_LOGIN_LOCKED", lockoutDuration]);
                                    model.LockedOut = true;
                                    _logger.LogInformation($"{correlationId} Login - Step 3.6.2.2: BuildLoginViewModelAsync");
                                    var lockModel = new LockedOutModel()
                                    {
                                        Duration = lockoutDuration,
                                        IsLocked = true,
                                        ExpiredTime = DateTime.UtcNow.AddMinutes(lockoutDuration)
                                    };
                                    HttpContext.Session.Set(ClientProperties.LockedOut, lockModel.Serialize());
                                }
                                else
                                {
                                    await RaiseLoginFailureEventAsync(model.Username, user?.GetDisplayName(), ViewMessages.InvalidCredentials, context?.ClientId);
                                    ModelState.AddModelError(string.Empty, _localizer["ERROR.VALIDATION.INVALID_LOGIN_INFO_WITH_LOGIN_ATTEMPT_REMAINING", lockoutAttempt - loginAttempt]);
                                } 
                            }
                            else
                            {
                                await RaiseLoginFailureEventAsync(model.Username, user?.GetDisplayName(), ViewMessages.InvalidCredentials, context?.ClientId);
                                ModelState.AddModelError(string.Empty, _localizer["ERROR.VALIDATION.INVALID_LOGIN_INFO"]);
                                await _cache.DeleteAsync(loginAttemptCacheKey);
                            }
                            // var viewModel = await BuildLoginViewModelAsync(model);
                            // return View(viewModel);
                            return RedirectToAction(nameof(Login), new { returnUrl = model.ReturnUrl });
                        }
                    }

                    await RaiseLoginFailureEventAsync(model.Username, user?.GetDisplayName(), ViewMessages.InvalidCredentials, context?.ClientId);
                    ModelState.AddModelError(string.Empty, _localizer["ERROR.VALIDATION.ACCOUNT_NOT_FOUND"]);
                }
            }
            // something went wrong, show form with error

            _logger.LogInformation($"{correlationId} Login - Step 4: Validation error - return view");
            // var vm = await BuildLoginViewModelAsync(model);
            // return View(vm);
            return RedirectToAction(nameof(Login), new { returnUrl = model.ReturnUrl });
        }

        [HttpGet]
        [ImportModelState]
        public IActionResult ForgotPassword(string returnUrl, string userName = "", bool result = false )
        {
            var viewModel = new ForgotPasswordInputModel()
            {
                ReturnUrl = returnUrl,
                Username = userName,
                Result = result
            };

            return View(viewModel);
        }

        [HttpPost]
        [ExportModelState]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordInputModel model, string button)
        {
            if (string.Equals(button, "Cancel", StringComparison.InvariantCultureIgnoreCase))
            {
                return Redirect(await BuildRedirectUrlAsync(model.ReturnUrl));
            }

            if (ModelState.IsValid)
            {
                //Forgot password logics here
                model.Username = model.Username.Trim();
                if (!_emailService.IsValid(model.Username))
                {
                    ModelState.AddModelError("Username", _localizer["ERROR.VALIDATION.INVALID_EMAIL"]);
                    return RedirectToAction(nameof(ForgotPassword), new { returnUrl = model.ReturnUrl });
                }
                // fixing the security issue
                // https://dev.azure.com/thanhtrungbui/yokogawa-internal/_workitems/edit/7302/
                // never show the error message
                var user = await _userService.FindByUpnAsync(model.Username);
                if (user != null)
                {
                    var userInfo = await _userInfoService.GetUserInfoAsync(model.Username);
                    if (!userInfo.IsLocked && UserTypes.VALID_LOCAL_LOGIN_USER_TYPES.Contains(user.UserTypeCode))
                    {
                        try
                        {
                            // US 8771: If login_type_code != "ahi-local"
                            // Return view "An email has been sent to you, please check your inbox." but don't send any email to user.
                            if (user.LoginTypeCode == LoginTypes.AHI_LOCAL || user.LoginTypeCode == LoginTypes.SAP_CDC)
                            {
                                await _userService.RequestSetPasswordAsync(user, TokenTypeEnum.ResetPassword, model.ReturnUrl);
                                _userContext.SetUpn(user.Upn);
                                await _auditLogService.SendLogAsync(ActivityEntityAction.CREDENTIAL, ActivitiesLogEventAction.Forgot_Password, ActionStatus.Success, user.Upn, user.GetDisplayName());
                            }
                        }
                        catch (GenericProcessFailedException)
                        {
                            //Expected exception and won't show error, already has error log in EmailService
                        }
                    }
                }
                model.Result = true;
            }
            return RedirectToAction(nameof(ForgotPassword), new { returnUrl = model.ReturnUrl, userName = model.Username, result = model.Result });
        }

        [HttpGet]
        [ImportModelState]
        public IActionResult LinkExpired(LinkExpiredInputModel inputModel)
        {
            return View(inputModel);
        }

        [HttpPost]
        [ExportModelState]
        public async Task<IActionResult> LinkExpired(LinkExpiredInputModel model, string button)
        {
            if (ModelState.IsValid)
            {
                model.Username = model.Username.Trim();
                var user = await _userService.FindByUpnAsync(model.Username);
                //US 8771: if login_type != ahi-ad then don't send email
                if (user != null && UserTypes.VALID_LOCAL_LOGIN_USER_TYPES.Contains(user.UserTypeCode))
                {
                    var userInfo = await _userInfoService.GetUserInfoAsync(model.Username);
                    if (!userInfo.IsLocked)
                    {
                        try
                        {
                            _userContext.SetUpn(user.Upn);
                            switch (model.TokenType)
                            {
                                case TokenTypeEnum.SetPassword:
                                    await _userService.RequestSetPasswordAsync(user, model.TokenType, model.ReturnUrl);
                                    await _auditLogService.SendLogAsync(ActivityEntityAction.CREDENTIAL, ActivitiesLogEventAction.Resend_Set_Password, ActionStatus.Success, user.Upn, user.GetDisplayName());
                                    break;
                                case TokenTypeEnum.ResetPassword:
                                    await _userService.RequestSetPasswordAsync(user, model.TokenType, model.ReturnUrl);
                                    await _auditLogService.SendLogAsync(ActivityEntityAction.CREDENTIAL, ActivitiesLogEventAction.Resend_Reset_Password, ActionStatus.Success, user.Upn, user.GetDisplayName());
                                    break;
                                default:
                                    break;
                            }

                        }
                        catch (GenericProcessFailedException)
                        {
                            //Expected exception and won't show error, already has error log in EmailService
                        }
                    }
                }
                model.Result = true;
            }
            return RedirectToAction(nameof(LinkExpired), model);
        }

        [HttpGet]
        [ImportModelState]
        public async Task<IActionResult> SetPassword(string token, string username, string type)
        {
            var tokenType = type.ToEnum<TokenTypeEnum>();
            var model = new SetPasswordInputModel()
            {
                Username = username,
                TokenType = tokenType
            };
            var tokenObject = await _tokenService.GetUserTokenAsync(username, token, tokenType);
            var returnUrl = tokenObject?.RedirectUrl ?? _configuration["DefaultEmail:RedirectUrl"];
            if (tokenObject == null)
            {
                var expiredModel = new LinkExpiredInputModel()
                {
                    Result = false,
                    ReturnUrl = returnUrl,
                    Username = username,
                    TokenType = tokenType
                };
                return RedirectToAction("LinkExpired", expiredModel);
            }

            model.SetPasswordToken = tokenObject.TokenKey;
            model.ReturnUrl = returnUrl;
            var user = await _userService.FindByUpnAsync(username);
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.LoginTypeCode = user.LoginTypeCode;
            model.PasswordValidationRules = await _userService.GetPasswordPolicyAsync(user.TenantId, user.SubscriptionId);
            return View(model);
        }

        [HttpPost]
        [ExportModelState]
        public async Task<IActionResult> SetPassword(SetPasswordInputModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.FindByUpnAsync(model.Username);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, _localizer["ERROR.ACCOUNT_NOT_FOUND", model.Username]);
                    return RedirectToAction(nameof(SetPassword), new { token = model.SetPasswordToken, userName = model.Username, type = model.TokenType });
                }
                var tokenObject = await _tokenService.GetUserTokenAsync(model.Username, model.SetPasswordToken, model.TokenType);
                await _tokenService.DeleteTokenByTypeAsync(model.Username, model.TokenType);
                var returnUrl = tokenObject?.RedirectUrl ?? _configuration["DefaultEmail:RedirectUrl"];
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.LoginTypeCode = user.LoginTypeCode;
                model.PasswordValidationRules = await _userService.GetPasswordPolicyAsync(user.TenantId, user.SubscriptionId);
                if (tokenObject == null)
                {
                    var expiredModel = new LinkExpiredInputModel()
                    {
                        Result = false,
                        ReturnUrl = returnUrl,
                        Username = model.Username,
                        TokenType = model.TokenType
                    };
                    return RedirectToAction("LinkExpired", expiredModel);
                }
                else
                {
                    if (model.NewPassword != model.ConfirmNewPassword)
                    {
                        var setPasswordToken = await _tokenService.GenerateTokenAsync(model.Username, model.TokenType, returnUrl);
                        model.SetPasswordToken = setPasswordToken.TokenKey;
                        ModelState.AddModelError("ConfirmNewPassword", _localizer["ERROR.VALIDATION.PASSWORDS_NOT_MATCH"]);
                        return RedirectToAction(nameof(SetPassword), new { token = model.SetPasswordToken, userName = model.Username, type = model.TokenType });
                    }
                    var sendUpdateEmail = (model.TokenType == TokenTypeEnum.SetPassword) ? false : true;
                    var changePasswordResult = await _userService.ChangePasswordAsync(model.Username, user.LoginTypeCode, model.NewPassword, sendEmail: sendUpdateEmail);
                    if (!changePasswordResult.Result)
                    {
                        var setPasswordToken = await _tokenService.GenerateTokenAsync(model.Username, model.TokenType, returnUrl);
                        model.SetPasswordToken = setPasswordToken.TokenKey;
                        var validationMessage = _localizer[changePasswordResult.Message];
                        if (changePasswordResult.Message == MessageConstants.PASSWORD_POLICY_HISTORY_FAILED)
                        {
                            validationMessage = _localizer[changePasswordResult.Message, model.PasswordValidationRules.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_HISTORY_NUMBER).Value];
                        }
                        ModelState.AddModelError("NewPassword", validationMessage);
                        return RedirectToAction(nameof(SetPassword), new { token = model.SetPasswordToken, userName = model.Username, type = model.TokenType });
                    }
                    _ = await _userInfoService.GetUserInfoAsync(model.Username);
                    return RedirectToAction("SetPasswordSuccess", new { username = model.Username, returnUrl = model.ReturnUrl });
                }
            }
            return RedirectToAction(nameof(SetPassword), new { token = model.SetPasswordToken, userName = model.Username, type = model.TokenType });
        }

        [HttpGet]
        [ImportModelState]
        public async Task<IActionResult> ChangePassword(string username, string token, string returnUrl)
        {
            var model = new ChangePasswordInputModel()
            {
                Username = username,
                Token = token,
                ReturnUrl = returnUrl
            };
            var tokenObject = await _tokenService.GetUserTokenAsync(model.Username, model.Token, TokenTypeEnum.ChangePassword);
            model.ReturnUrl = tokenObject?.RedirectUrl ?? model.ReturnUrl ?? "";
            if (tokenObject == null)
            {
                var vModel = new ErrorViewModel()
                {
                    EmailAddress = model.Username,
                    ErrorTitle = _localizer["PAGE.ERROR.TITLE"],
                    ErrorMessage = _localizer["MESSAGE.TOKEN_EXPIRED"],
                    AutoRedirectTime = 15,
                    ReturnUrl = model.ReturnUrl
                };
                return RedirectToAction("Error", "Account", vModel);
            }
            var user = await _userService.FindByUpnAsync(username);
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.LoginTypeCode = user.LoginTypeCode;
            model.PasswordValidationRules = await _userService.GetPasswordPolicyAsync(user.TenantId, user.SubscriptionId);
            return View(model);
        }

        [HttpPost]
        [ExportModelState]
        public async Task<IActionResult> ChangePassword(ChangePasswordInputModel model, string button)
        {
            if (ModelState.IsValid)
            {
                var tokenObject = await _tokenService.GetUserTokenAsync(model.Username, model.Token, TokenTypeEnum.ChangePassword);
                var returnUrl = tokenObject?.RedirectUrl ?? _configuration["DefaultEmail:RedirectUrl"];
                if (tokenObject == null)
                {
                    var vModel = new ErrorViewModel()
                    {
                        EmailAddress = model.Username,
                        ErrorTitle = _localizer["PAGE.ERROR.TITLE"],
                        ErrorMessage = _localizer["MESSAGE.TOKEN_EXPIRED"],
                        AutoRedirectTime = 15,
                        ReturnUrl = model.ReturnUrl
                    };
                    return RedirectToAction("Error", "Account", vModel);
                }
                else
                {
                    var userInfo = await _userInfoService.GetUserInfoAsync(model.Username);
                    var user = await _userService.FindByUpnAsync(model.Username);
                    var tokenType = TokenTypeEnum.ChangePassword;
                    model.FirstName = user.FirstName;
                    model.LastName = user.LastName;
                    model.LoginTypeCode = user.LoginTypeCode;
                    model.PasswordValidationRules = await _userService.GetPasswordPolicyAsync(user.TenantId, user.SubscriptionId);
                    if (model.NewPassword != model.ConfirmNewPassword)
                    {
                        var token = await _tokenService.GenerateTokenAsync(model.Username, tokenType, model.ReturnUrl);
                        model.Token = token.TokenKey;
                        ModelState.AddModelError("ConfirmNewPassword", _localizer["ERROR.VALIDATION.PASSWORDS_NOT_MATCH"]);
                        return RedirectToAction("ChangePassword", new { username = model.Username, token = model.Token, returnUrl = model.ReturnUrl });
                    }
                    var changePasswordResult = await _userService.ChangePasswordAsync(model.Username, user.LoginTypeCode, model.NewPassword, model.CurrentPassword, sendEmail: true);
                    if (!changePasswordResult.Result)
                    {
                        var token = await _tokenService.GenerateTokenAsync(model.Username, tokenType, model.ReturnUrl);
                        model.Token = token.TokenKey;
                        var validationMessage = _localizer[changePasswordResult.Message];
                        if (changePasswordResult.Message == MessageConstants.ERROR_VALIDATION_WRONG_CURRENT_PASSWORD)
                        {
                            ModelState.AddModelError("CurrentPassword", validationMessage);
                        }
                        else if (changePasswordResult.Message == MessageConstants.PASSWORD_POLICY_HISTORY_FAILED)
                        {
                            validationMessage = _localizer[changePasswordResult.Message, model.PasswordValidationRules.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_HISTORY_NUMBER).Value];
                            ModelState.AddModelError("NewPassword", validationMessage);
                        }
                        else
                        {
                            ModelState.AddModelError("NewPassword", validationMessage);
                        }
                        return RedirectToAction("ChangePassword", new { username = model.Username, token = model.Token, returnUrl = model.ReturnUrl });
                    }
                    return RedirectToAction("SetPasswordSuccess", new { username = model.Username, returnUrl = model.ReturnUrl });
                }
            }
            return RedirectToAction("ChangePassword", new { username = model.Username, token = model.Token, returnUrl = model.ReturnUrl });
        }

        [HttpGet]
        public IActionResult SetPasswordSuccess(string username, string returnUrl)
        {
            var viewModel = new SetPasswordSuccessInputModel()
            {
                Username = username,
                ReturnUrl = returnUrl,
                AutoRedirectTime = 5
            };
            return View("SetPasswordSuccess", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SetPasswordSuccess(SetPasswordSuccessInputModel model)
        {
            // var user = await _userService.FindByUpnAsync(model.Username);
            // // US 8771: if link valid & login_type_code = "ahi-ad"
            // // Save password
            // // Return View Access Denied (Unauthorized)
            // if (user.LoginTypeCode != LoginTypes.AHI_LOCAL && user.LoginTypeCode != LoginTypes.SAP_CDC)
            // {
            //     var viewModel = new AccessDeniedViewModel()
            //     {
            //         ReturnUrl = model.ReturnUrl,
            //         Message = _localizer["ERROR.ACCOUNT_NOT_SUPPORTED"]
            //     };
            //     return RedirectToAction("AccessDenied", "Account", viewModel);
            // }
            // var userInfo = await _userInfoService.GetUserInfoAsync(model.Username);
            // if (userInfo.IsLocked)
            // {
            //     var viewModel = new AccessDeniedViewModel()
            //     {
            //         ReturnUrl = model.ReturnUrl,
            //         Message = _localizer["ERROR.ACCESS_DENIED"]
            //     };
            //     return RedirectToAction("AccessDenied", "Account", viewModel);
            // }
            // if (userInfo.Subscriptions.Select(x => x.TenantId).Distinct().Count() > 1)
            // {
            //     return RedirectToAction("SelectTenant", new { username = user.Upn, returnUrl = model.ReturnUrl });
            // }
            // else
            // {
            //     var subscriptionId = user.SubscriptionId;
            //     await HttpContext.SignInAsync(user.Upn, user.FirstName, IssueUserClaims(user.Upn, $"{user.FirstName} {user.LastName}".Trim(), user.TenantId, subscriptionId, null, user.DefaultPage));
            //     var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            //     _tenantContext.RetrieveFromString(user.TenantId, user.SubscriptionId);
            //     await RaiseLoginSuccessEventAsync(context?.ClientId, userInfo, Guid.NewGuid());
            //     PostLoginSuccessAsyncFireAndForget(userInfo, _tenantContext);
            //     return this.LoadingPage("Redirect", await BuildRedirectUrlAsync(model.ReturnUrl));
            // }

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [ImportModelState]
        public async Task<IActionResult> Verification(string username, string returnUrl)
        {
            _ = await _userService.FindByUpnAsync(username);
            var viewModel = new VerificationLoginInputModel()
            {
                Username = username,
                ReturnUrl = returnUrl
            };
            return View(viewModel);
        }

        [HttpPost]
        [ExportModelState]
        public async Task<IActionResult> Verification(VerificationLoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                var (is2faTokenValid, user) = await ValidateMFAAsync(model.Username, model.Token);

                if (!is2faTokenValid)
                {
                    ModelState.AddModelError(string.Empty, _localizer["ERROR.VALIDATION.INVALID_VERIFICATION_CODE"]);
                    return RedirectToAction(nameof(Verification), new { username = model.Username, returnUrl = model.ReturnUrl });
                }
                else
                {
                    return await PostVerificationAsync(user, model.ReturnUrl);
                }
            }
            return RedirectToAction(nameof(Verification), new { username = model.Username, returnUrl = model.ReturnUrl });
        }


        [HttpGet]
        [ImportModelState]
        public async Task<IActionResult> EnableAuthenticator(string username, string returnUrl)
        {
            var user = await _userService.FindByUpnAsync(username);
            var viewModel = new EnableAuthenticatorModel()
            {
                Username = username,
                ReturnUrl = returnUrl
            };
            await LoadSharedKeyAndQrCodeUriAsync(user, viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [ExportModelState]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorModel model)
        {
            var (is2faTokenValid, user) = await ValidateMFAAsync(model.Username, model.Token);
            if (!is2faTokenValid)
            {
                ModelState.AddModelError(string.Empty, _localizer["ERROR.VALIDATION.INVALID_VERIFICATION_CODE"]);
                await LoadSharedKeyAndQrCodeUriAsync(user, model);
                return RedirectToAction(nameof(EnableAuthenticator), new { username = model.Username, returnUrl = model.ReturnUrl});
            }
            else
            {
                // update the setup MFA to true, no need to show the mfa screen anymore
                await _userService.UpdateMFAAsync(user.Id, user.MFA, true);
                return await PostVerificationAsync(user, model.ReturnUrl);
            }
        }

        [HttpGet]
        [ImportModelState]
        public async Task<IActionResult> SelectTenant(string username, string returnUrl)
        {
            var viewModel = new SelectTenantInputModel()
            {
                Username = username,
                ReturnUrl = returnUrl
            };
            var user = await _userService.FindByUpnAsync(username);
            if (user == null)
            {
                var vModel = new ErrorViewModel()
                {
                    EmailAddress = username,
                    ErrorTitle = _localizer["PAGE.ERROR.TITLE"],
                    ErrorMessage = _localizer["ERROR.ACCOUNT_NOT_FOUND", username],
                    AutoRedirectTime = 15,
                    ReturnUrl = returnUrl
                };
                return RedirectToAction("Error", "Account", vModel);
            }
            var tenantList = new List<TenantDto>();
            tenantList.Add(new TenantDto() { Id = null, Name = _localizer["PAGE.SELECT_TENANT.FIELD.TENANT.PLACEHOLDER"] });

            var allTenants = await _masterService.GetAllTenantsAsync();
            foreach (var tenant in allTenants)
            {
                var userInfo = await _userInfoService.GetUserInfoAsync(user.Upn);
                if (userInfo.Subscriptions.Select(x => x.TenantId).Any(x => x == tenant.ResourceId))
                {
                    tenantList.Add(tenant);
                }
            }
            viewModel.Tenants = new SelectList(tenantList, nameof(TenantDto.ResourceId), nameof(TenantDto.Name));
            return View(viewModel);
        }

        [HttpPost]
        [ExportModelState]
        public async Task<IActionResult> SelectTenant(SelectTenantInputModel model)
        {
            AuthenticationProperties props = null;
            var user = await _userService.FindByUpnAsync(model.Username);
            var userSubscription = await _userInfoService.GetUserSubscriptionsAsync(user.UserId);

            var firstSubscription = userSubscription.First(x => x.TenantId == model.SelectedTenant);
            var subscriptionId = firstSubscription.Id;
            await HttpContext.SignInAsync(user.Upn,
                                            props,
                                            IssueUserClaims(user.Upn, $"{user.FirstName} {user.LastName}".Trim(), model.SelectedTenant, subscriptionId));

            await UpdateUserLanguageAsync(user.Id);
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            _tenantContext.RetrieveFromString(model.SelectedTenant, subscriptionId);
            await RaiseLoginSuccessEventAsync(context?.ClientId, user, Guid.NewGuid());
            PostLoginSuccessAsyncFireAndForget(user, userSubscription, _tenantContext);
            return this.LoadingPage("Redirect", await BuildRedirectUrlAsync(model.ReturnUrl));
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            Guid correlationId = Guid.NewGuid();
            _logger.LogInformation($"{correlationId} Logout (Get) - Step 1: Start: {logoutId}");

            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(logoutId);
            if (!string.IsNullOrEmpty(vm.SubjectId))
            {
                // revoke all sessions
                var user = await _userService.FindByUpnAsync(vm.SubjectId, true);
                if (user != null)
                {
                    _logger.LogInformation($"{correlationId} Logout (Get) - Step 2: RevokeUserSessionAsync {user.Upn}");
                    RevokeUserSessionFireAndForget(correlationId, user, _tenantContext);
                }
            }

            if (!vm.ShowLogoutPrompt)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.

                vm.CorrelationId = correlationId;
                _logger.LogInformation($"{correlationId} Logout (Get) - Step 3: End ShowLogoutPrompt");
                return await Logout(vm);
            }

            _logger.LogInformation($"{correlationId} Logout (Get) - Step 3: End");
            return View(vm);
        }

        /// <summary>
        /// Handle logout page post back
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            Guid correlationId = model.CorrelationId;

            if (correlationId == Guid.Empty)
            {
                correlationId = Guid.NewGuid();
                _logger.LogInformation($"{correlationId} Logout - Step 0: Start Post Logout from client");
            }

            // build a model so the logged out page knows what to display
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId, correlationId);
            _logger.LogInformation($"{correlationId} Logout - Step 1: Logout context: {vm.ToJson()}");

            if (User?.Identity.IsAuthenticated == true)
            {
                _logger.LogInformation($"{correlationId} Logout - Step 2: User?.Identity.IsAuthenticated");
                if (string.IsNullOrEmpty(_tenantContext.TenantId) || string.IsNullOrEmpty(_tenantContext.SubscriptionId))
                {
                    var userClaims = User?.Claims?.ToList() ?? new List<Claim>();
                    var tenantId = userClaims.FirstOrDefault(x => string.Equals(x.Type, "tenantId", StringComparison.InvariantCultureIgnoreCase))?.Value;
                    var subscriptionId = userClaims.FirstOrDefault(x => string.Equals(x.Type, "subscriptionId", StringComparison.InvariantCultureIgnoreCase))?.Value;
                    _tenantContext.RetrieveFromString(tenantId, subscriptionId);
                }

                _logger.LogInformation($"{correlationId} Logout - Step 3: HttpContext.SignOutAsync");
                // delete local authentication cookie
                await HttpContext.SignOutAsync();

                // raise the logout event
                _logger.LogInformation($"{correlationId} Logout - Step 4: _clientStore.FindClientByIdAsync");
                var clientInfo = await _clientStore.FindClientByIdAsync(vm.ClientId);
                var applicationId = "";
                clientInfo?.Properties.TryGetValue(ClientProperties.ApplicationId, out applicationId);

                _logger.LogInformation($"{correlationId} Logout - Step 5: _userService.FindByUpnAsync");
                var user = await _userService.FindByUpnAsync(User.GetSubjectId());

                _logger.LogInformation($"{correlationId} Logout - Step 6: RaiseLogoutSuccessEventAsync");
                await RaiseLogoutSuccessEventAsync(User.GetSubjectId(), user?.GetDisplayName(), applicationId);
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignOut)
            {
                _logger.LogInformation($"{correlationId} Logout - Step 7: vm.TriggerExternalSignOut");
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            _logger.LogInformation($"{correlationId} Logout - Step 8: End");
            return View("LoggedOut", vm);
        }

        [HttpGet]
        public IActionResult AccessDenied(AccessDeniedViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AccessDenied(AccessDeniedViewModel model, string button = "Ok")
        {
            return this.LoadingPage("Redirect", await BuildRedirectUrlAsync(model.ReturnUrl));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResendTokenAsync(string email, string tokenType, string returnUrl)
        {
            var user = await _userService.FindByUpnAsync(email);

            var tokenTypeEnum = tokenType.ToEnum<TokenTypeEnum>();
            var timeLimit = _configuration.GetValue("Thresholds:TokenResendTimeLimit", "60");
            var token = await _tokenService.GetLatestTokenByTypeAsync(email, tokenType.ToEnum<TokenTypeEnum>());
            var timeLeft = double.Parse(timeLimit);
            if (token != null)
            {
                timeLeft = double.Parse(timeLimit) - (DateTime.UtcNow - token.CreatedDate).TotalSeconds;
                if (timeLeft > 0)
                {
                    return Json(new
                    {
                        TimeLeft = timeLeft
                    });
                }
                else
                {
                    timeLeft = double.Parse(timeLimit);

                }
            }
            try
            {
                switch (tokenTypeEnum)
                {
                    case TokenTypeEnum.ResetPassword:
                        await _userService.RequestSetPasswordAsync(user, tokenTypeEnum, returnUrl);
                        _userContext.SetUpn(user.Upn);
                        await _auditLogService.SendLogAsync(ActivityEntityAction.CREDENTIAL, ActivitiesLogEventAction.Resend_Reset_Password, ActionStatus.Success, user.Upn, user.GetDisplayName());
                        break;
                    default:
                        break;
                }
            }
            catch (GenericProcessFailedException exc)
            {
                _logger.LogError(exc, exc.Message);
            }
            return Json(new
            {
                TimeLeft = timeLeft
            });
        }

        [HttpGet]
        public IActionResult Error(ErrorViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Error(ErrorViewModel model, string button = "Ok")
        {
            return this.LoadingPage("Redirect", await BuildRedirectUrlAsync(model.ReturnUrl));
        }

        [HttpPost]
        public IActionResult GetTranslations(string[] keys)
        {
            var results = new Dictionary<string, string>();
            foreach (var key in keys)
            {
                results.Add(key, _localizer[key]);
            }

            return Json(new
            {
                translations = results
            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeLanguage(string language, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(7) }
            );
            return LocalRedirect(await BuildRedirectUrlAsync(returnUrl));
        }

        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        public static Claim[] IssueUserClaims(string upn, string name, string tenantString, string subscriptionString, string projectString = null, string defaultPage = null, string applicationId = null)
        {
            var claims = new List<Claim>()
            {
                 new Claim("upn",upn),
                 new Claim(ClaimTypes.Name,name),
                 new Claim("tenantId", tenantString),
                 new Claim("subscriptionId", subscriptionString)
            };
            if (!string.IsNullOrEmpty(projectString))
            {
                claims.Add(new Claim("projectId", projectString));
            };
            if (!string.IsNullOrEmpty(defaultPage))
            {
                claims.Add(new Claim("defaultPage", defaultPage));
            };
            if (!string.IsNullOrEmpty(applicationId))
            {
                claims.Add(new Claim("applicationId", applicationId));
            };
            return claims.ToArray();
        }

        private string GetRequestIp()
        {
            Request.Headers.TryGetValue("X-Forwarded-For", out var headerValues);
            var ipAddress = headerValues.FirstOrDefault()?.Split(",")?.FirstOrDefault();
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
            if (string.IsNullOrEmpty(ipAddress))
            {
                Request.Headers.TryGetValue("REMOTE_ADDR", out headerValues);
                ipAddress = headerValues.FirstOrDefault();
            }
            return ipAddress;
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            ViewBag.ReturnUrl = HttpUtility.UrlEncode(returnUrl);
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServer4.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new LoginViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null ||
                            (x.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
                )
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.LockedOut = model.LockedOut;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            vm.SubjectId = context?.SubjectId;
            vm.ClientId = context?.ClientId;
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }
            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        private async Task<bool> IsValidRecaptchaAsync(HttpContext request, Guid correlationId)
        {
            var secretKey = _configuration["ApplicationKeys:reCaptchaV2:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                return true;
            }
            var result = false;
            var response = request.Request.Form["g-recaptcha-response"];
            _logger.LogInformation($"{correlationId} Validate Recaptcha Input\r\nToken{response}");
            if (!string.IsNullOrEmpty(response))
            {
                var client = _httpClientFactory.CreateClient(HttpClients.RECAPTCHA_SERVICE);
                var requestContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("secret", secretKey),
                    new KeyValuePair<string, string>("response", response)
                });

                var apiResponse = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", requestContent);
                if (apiResponse.IsSuccessStatusCode)
                {
                    var content = await apiResponse.Content.ReadAsByteArrayAsync();
                    var captchaResponse = content.Deserialize<RecaptchaResponse>();
                    _logger.LogInformation($"{correlationId} Validate Recaptcha Result\r\n{JsonConvert.SerializeObject(captchaResponse)}");
                    if (captchaResponse.Success)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId, Guid correlationId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated sign out)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);
            string serializedLogout = logout != null ? JsonConvert.SerializeObject(logout) : "";
            _logger.LogInformation($"{correlationId} BuildLoggedOutViewModelAsync logoutId: {logoutId} {serializedLogout}");

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri ?? _configuration["DefaultEmail:RedirectUrl"],
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId,
                ClientId = logout?.ClientId
            };

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }

        private Task UpdateUserLanguageAsync(Guid userId)
        {
            var patchDocument = new JsonPatchDocument();
            var op = new Operation("update/language", "/", string.Empty, new
            {
                languageCode = CultureInfo.CurrentCulture.Name
            });
            patchDocument.Operations.Add(op);

            return Task.CompletedTask;
        }

        private async Task<string> BuildRedirectUrlAsync(string redirectUrl)
        {
            if (await IsAuthorizedUrlAsync(redirectUrl))
            {
                return redirectUrl;
            }
            else
            {
                return _configuration["DefaultEmail:RedirectUrl"] ?? "~/";
            }
        }

        private async Task<bool> IsAuthorizedUrlAsync(string redirectUrl)
        {
            if (string.IsNullOrEmpty(redirectUrl))
            {
                return false;
            }
            if (Url.IsLocalUrl(redirectUrl))
            {
                return true;
            }
            var cacheKey = $"idp_redirect_{redirectUrl}";
            var cacheResult = await _cache.GetStringAsync(cacheKey);
            _logger.LogInformation($"_cache.GetStringAsync: key:{cacheKey}, value:{cacheResult}");
            if (!string.IsNullOrEmpty(cacheResult))
            {
                return Convert.ToBoolean(cacheResult);
            }

            var result = false;
            try
            {
                var redirectUri = new Uri(redirectUrl); // can throw exception when redirectUrl is not a valid url
                var redirectHost = redirectUri.Host;
                var context = await _interaction.GetAuthorizationContextAsync(redirectUrl);
                if (context != null && await _clientStore.IsPkceClientAsync(context.ClientId))
                {
                    return true;
                }
                var clientInfo = await _clientStore.FindClientByIdAsync(context.ClientId);
                result = clientInfo.RedirectUris.Any(s => s.Contains(redirectHost));
                await _cache.StoreStringAsync(cacheKey, result.ToString(), TimeSpan.FromDays(1));
            }
            catch
            {
                result = false;
            }
            return result;
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(UserDto user, EnableAuthenticatorModel model)
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            model.SharedKey = FormatKey(unformattedKey);
            model.AuthenticatorUri = GenerateQrCodeUri(user.Email, unformattedKey);
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                "AHI Identity",
                WebUtility.UrlEncode(email),
                unformattedKey);
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new System.Text.StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private async Task<(bool, UserDto)> ValidateMFAAsync(string username, string token)
        {
            // Strip spaces and hyphens
            var user = await _userService.FindByUpnAsync(username);
            var mfa = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, token);
            return (mfa, user);
        }

        private void PostLoginSuccessAsyncFireAndForget(UserDto user, IEnumerable<IdpServer.Application.User.Model.SubscriptionDto> userSubscriptions,
            ITenantContext tenantContextSource)
        {
            Task.Run(async () => await PostLoginSuccessAsync(user, userSubscriptions, tenantContextSource));
        }

        private async Task PostLoginSuccessAsync(
            UserDto user,
            IEnumerable<IdpServer.Application.User.Model.SubscriptionDto> userSubscriptions,
            ITenantContext tenantContextSource)
        {
            var scope = _scopeFactory.CreateScope();
            var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
            tenantContext.CopyFrom(tenantContextSource);
            ICache cache = scope.ServiceProvider.GetRequiredService<ICache>();
            ILoggerAdapter<AccountController> logger = scope.ServiceProvider.GetRequiredService<ILoggerAdapter<AccountController>>();
            IMasterService masterService = scope.ServiceProvider.GetRequiredService<IMasterService>();

            try
            {
                logger.LogInformation("Start Try/Catch PostLoginSuccessAsync.");
                string upn = user.Upn;
                var enableConsent = Convert.ToBoolean(_configuration["EnableConsent"] ?? "false");

                if (enableConsent)
                {
                    foreach (var subscription in userSubscriptions)
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

                    var defaultKey = string.Format(CacheKey.REQUIRED_CONSENT, user.TenantId, user.SubscriptionId, null, upn);
                    await cache.StoreStringAsync(defaultKey, "1", TimeSpan.FromDays(1));
                    var tenantKey = string.Format(CacheKey.REQUIRED_CONSENT, user.TenantId, null, null, upn);
                    await cache.StoreStringAsync(tenantKey, "1", TimeSpan.FromDays(1));
                }

                logger.LogInformation("End Try/Catch PostLoginSuccessAsync.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "PostLoginSuccessAsync failed.");
            }
        }

        private async Task RaiseLoginSuccessEventAsync(
            string clientId,
            UserDto user,
            Guid correlationId)
        {
            _logger.LogInformation($"{correlationId} Start RaiseLoginSuccessEventAsync.");
            try
            {
                string upn = user.Upn;
                string userId = user.Id.ToString();
                string firstName = user.FirstName;
                string displayName = user.GetDisplayName();
                var clientInfo = await _clientStore.FindClientByIdAsync(clientId);
                var applicationId = "";
                clientInfo?.Properties.TryGetValue(ClientProperties.ApplicationId, out applicationId);
                await _events.RaiseAsync(new AHIUserLoginSuccessEvent(upn, userId, firstName, clientId: clientId, applicationId: applicationId, correlationId: correlationId)
                {
                    DisplayName = displayName
                });

                _logger.LogInformation($"{correlationId} End RaiseLoginSuccessEventAsync.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(RaiseLoginSuccessEventAsync)} {ex.Message}");
            }
        }

        private async Task RaiseLoginFailureEventAsync(string upn, string displayName, string error, string clientId)
        {
            try
            {
                var clientInfo = await _clientStore.FindClientByIdAsync(clientId);
                var applicationId = "";
                clientInfo?.Properties.TryGetValue(ClientProperties.ApplicationId, out applicationId);
                await _events.RaiseAsync(new AHIUserLoginFailureEvent(upn, error, clientId: clientId, applicationId: applicationId, displayName: displayName));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(RaiseLoginFailureEventAsync)} {ex.Message}");
            }
        }

        private async Task RaiseLogoutSuccessEventAsync(string subjectId, string displayName, string applicationId)
        {
            try
            {
                await _events.RaiseAsync(new AHIUserLogoutSuccessEvent(subjectId, displayName, applicationId: applicationId));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(RaiseLogoutSuccessEventAsync)} {ex.Message}");
            }
        }

        private async Task<IActionResult> PostVerificationAsync(UserDto user, string returnUrl)
        {
            var userSubscriptions = await _userInfoService.GetUserSubscriptionsAsync(user.UserId);

            if (userSubscriptions.Select(x => x.TenantId).Distinct().Count() > 1)
            {
                return RedirectToAction("SelectTenant", new { username = user.Upn, returnUrl = returnUrl });
            }
            AuthenticationProperties props = null;
            var subscriptionId = user.SubscriptionId;
            await HttpContext.SignInAsync(user.Upn, props, IssueUserClaims(user.Upn, $"{user.FirstName} {user.LastName}".Trim(), user.TenantId, subscriptionId, null, user.DefaultPage));
            PostLoginSuccessAsyncFireAndForget(user, userSubscriptions, _tenantContext);
            return this.LoadingPage("Redirect", await BuildRedirectUrlAsync(returnUrl));
        }

        private void RevokeUserSessionFireAndForget(Guid correlationId, UserDto user, ITenantContext tenantContextSource)
        {
            Task.Run(async () => await RevokeUserSessionAsync(correlationId, user, tenantContextSource));
        }

        private async Task RevokeUserSessionAsync(Guid correlationId,
            UserDto user,
            ITenantContext tenantContextSource)
        {
            var scope = _scopeFactory.CreateScope();
            ILoggerAdapter<AccountController> logger = scope.ServiceProvider.GetRequiredService<ILoggerAdapter<AccountController>>();
            IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            try
            {
                // Only revoke other user sessions when this session is logged-out successfully
                Thread.Sleep(2000);
                await userService.RevokeUserSessionAsync(user.Upn, user.TenantId, user.SubscriptionId, correlationId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "RevokeUserSessionAsync failed.");
            }
        }
    }
}