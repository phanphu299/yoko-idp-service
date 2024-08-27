using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using IdpServer.Application.Service.Abstraction;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using IdpServer.Application.Event;
using IdpServer.Application.Constant;

namespace IdpServer.Customized
{
    public class UserSwitchTenantGrantValidator : IExtensionGrantValidator
    {
        private readonly ITokenValidator _tokenValidator;
        private readonly IDomainEventDispatcher _domainDispatcher;
        private readonly IUserInfoService _userInfoService;
        public UserSwitchTenantGrantValidator(ITokenValidator tokenValidator,
                                    IDomainEventDispatcher domainEventDispatcher,
                                    IUserInfoService userInfoService)
        {
            _tokenValidator = tokenValidator;
            _domainDispatcher = domainEventDispatcher;
            _userInfoService = userInfoService;
        }

        public string GrantType => "user_switch_tenant";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            GrantValidationResult grantResult = new GrantValidationResult();
            var tenantIdString = context.Request.Raw.Get("tenantId");
            var subscriptionIdString = context.Request.Raw.Get("subscriptionId");
            var projectIdString = context.Request.Raw.Get("projectId");
            var applicationIdString = context.Request.Raw.Get("applicationId") ?? ApplicationInformation.ASSET_MANAGEMENT_APPLICATION_ID;
            var userToken = context.Request.Raw.Get("token");
            if (string.IsNullOrEmpty(userToken))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }
            var result = await _tokenValidator.ValidateAccessTokenAsync(userToken);
            if (result.IsError)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }
            var upn = result.Claims.FirstOrDefault(x => x.Type == "upn").Value;
            var userInfo = await _userInfoService.GetUserInfoAsync(upn);

            var tenantId = userInfo.Subscriptions.Select(x => x.TenantId).FirstOrDefault(t => string.Equals(t, tenantIdString, StringComparison.InvariantCultureIgnoreCase));
            tenantId = string.IsNullOrEmpty(tenantId) ? userInfo?.HomeTenantId : tenantId;
            if (string.IsNullOrEmpty(tenantId))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ApiMessages.TENANT_ID_INVALID);
                return;
            }

            var subscriptionId = userInfo.Subscriptions.Where(x => x.TenantId == tenantId).Select(x => x.Id).FirstOrDefault(x => string.Equals(x, subscriptionIdString, StringComparison.InvariantCultureIgnoreCase));
            subscriptionId = string.IsNullOrEmpty(subscriptionId) ? userInfo?.HomeSubscriptionId : subscriptionId;
            if (string.IsNullOrEmpty(subscriptionId))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ApiMessages.SUBSCRIPTION_ID_INVALID);
                return;
            }
            string projectId = null;
            if (!string.IsNullOrEmpty(projectIdString))
            {
                var applicationId = Guid.Parse(applicationIdString);
                var projectIds = userInfo.Subscriptions.Where(x => x.Id == subscriptionId).SelectMany(x => x.Applications.Where(x => x.Id == applicationId)).SelectMany(x => x.Projects).Select(x => x.Id);
                projectId = projectIds.FirstOrDefault(x => string.Equals(x.ToString(), projectIdString, StringComparison.InvariantCultureIgnoreCase));
                if (!projectIds.Any() || !string.IsNullOrEmpty(projectId))
                {
                    grantResult = new GrantValidationResult(
                                                subject: userInfo.Upn,
                                                "oidc",
                                                IdentityServer4.Quickstart.UI.AccountController.IssueUserClaims(userInfo.Upn, $"{userInfo.FirstName} {userInfo.LastName}".Trim(), tenantId, subscriptionId, projectIdString, applicationId: applicationIdString.ToLower()));
                }
                else
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ApiMessages.PROJECT_ID_INVALID);
                    return;
                }
            }
            else
            {
                grantResult = new GrantValidationResult(
                                                               subject: userInfo.Upn,
                                                               "oidc",
                                                               IdentityServer4.Quickstart.UI.AccountController.IssueUserClaims(userInfo.Upn, $"{userInfo.FirstName} {userInfo.LastName}".Trim(), tenantId, subscriptionId));
            }
            context.Result = grantResult;
            var userAccessEvent = new UserAccessEvent(userInfo.Upn, tenantId, subscriptionId, projectIdString);
            await _domainDispatcher.SendAsync(userAccessEvent);
        }
    }
}
