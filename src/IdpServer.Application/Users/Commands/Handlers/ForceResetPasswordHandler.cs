using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AHI.Infrastructure.Exception;
using IdpServer.Application.Constant;
using IdpServer.Application.Service.Abstraction;
using IdpServer.Application.SharedKernel;
using MediatR;
using Microsoft.Extensions.Configuration;
using AHI.Infrastructure.SharedKernel.Abstraction;
using IdpServer.Application.Event;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.Audit.Service.Abstraction;
using AHI.Infrastructure.Audit.Constant;
using AHI.Infrastructure.UserContext.Abstraction;
using IdpServer.Application.Extension;
using AHI.Infrastructure.MultiTenancy.Extension;
using System;

namespace IdpServer.Application.User.Command.Handler
{
    public class ForceResetPasswordByIdHandler : IRequestHandler<ForceResetPassword, BaseResponse>
    {
        private readonly IUserService _service;
        private readonly ILoggerAdapter<ForceResetPasswordByIdHandler> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserInfoService _userInfoService;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContext _userContext;
        private readonly ITenantContext _tenantContext;
        private readonly IAccessControlService _accessControlService;

        public ForceResetPasswordByIdHandler(
                IUserService service,
                ILoggerAdapter<ForceResetPasswordByIdHandler> logger,
                IUserInfoService userInfoService,
                IAuditLogService auditLogService,
                IUserContext userContext,
                ITenantContext tenantContext,
                IConfiguration configuration,
                IAccessControlService accessControlService)
        {
            _service = service;
            _logger = logger;
            _configuration = configuration;
            _userInfoService = userInfoService;
            _auditLogService = auditLogService;
            _userContext = userContext;
            _tenantContext = tenantContext;
            _accessControlService = accessControlService;
        }

        public async Task<BaseResponse> Handle(ForceResetPassword request, CancellationToken cancellationToken)
        {
            _tenantContext.RetrieveFromString(request.TenantId, request.SubscriptionId);
            await SetApplicationName(ApplicationInformation.USER_MANAGEMENT_APPLICATION_ID, Guid.NewGuid());
            var user = await _service.FindByUpnAsync(request.Upn, true);
            if (user == null)
            {
                throw new EntityNotFoundException(MessageConstants.ENTITY_NOT_FOUND);
            }
            _tenantContext.RetrieveFromString(user.TenantId, user.SubscriptionId);
            _userContext.SetUpn(request.RequestBy);
            if (user.Deleted)
            {
                await _auditLogService.SendLogAsync(ActivityEntityAction.CREDENTIAL, ActivitiesLogEventAction.Reset_Password, ActionStatus.Fail, user.Upn, user.GetDisplayName());
                throw new EntityNotFoundException(MessageConstants.ENTITY_NOT_FOUND);
            }
            var userInfo = await _userInfoService.GetUserInfoAsync(user.Upn);
            if (userInfo.IsLocked)
            {
                await _auditLogService.SendLogAsync(ActivityEntityAction.CREDENTIAL, ActivitiesLogEventAction.Reset_Password, ActionStatus.Fail, user.Upn, user.GetDisplayName());
                throw new GenericCommonException(MessageConstants.ACCOUNT_LOCKED);
            }
            if (UserTypes.VALID_LOCAL_LOGIN_USER_TYPES.Contains(user.UserTypeCode))
            {
                try
                {
                    var returnUrl = _configuration["DefaultEmail:RedirectUrl"];
                    await _service.RequestSetPasswordAsync(user, Enum.TokenTypeEnum.ResetPassword, returnUrl, forceResetPassword: true);
                }
                catch
                {
                    //Expected exception and won't show error, already has error log in EmailService
                }
            }
            await _auditLogService.SendLogAsync(ActivityEntityAction.CREDENTIAL, ActivitiesLogEventAction.Reset_Password, ActionStatus.Success, user.Upn, user.GetDisplayName());
            return BaseResponse.Success;
        }

        private async Task SetApplicationName(string applicationId, Guid correlationId)
        {
            var applicationName = await _accessControlService.GetApplicationNameAsync(applicationId, correlationId);
            _logger.LogDebug($"{correlationId} ApplicationId: {applicationId} - ApplicationName: {applicationName}");
            _auditLogService.SetPropertyName(applicationName);
        }
    }
}
