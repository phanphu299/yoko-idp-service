using System.Threading.Tasks;
using IdpServer.Application.Constant;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdpServer.Application.Event;
using AHI.Infrastructure.Audit.Service.Abstraction;
using AHI.Infrastructure.UserContext.Abstraction;
using AHI.Infrastructure.Audit.Constant;
using IdpServer.Application.Service.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using System;

namespace IdpServer.Application.Service
{
    public class AuthenticationEventSink : IEventSink
    {
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContext _userContext;
        private readonly IAccessControlService _accessControlService;
        private readonly ILoggerAdapter<AuthenticationEventSink> _logger;

        public AuthenticationEventSink(IAuditLogService auditLogService, IAccessControlService accessControlService, IUserContext userContext, ILoggerAdapter<AuthenticationEventSink> logger)
        {
            _auditLogService = auditLogService;
            _userContext = userContext;
            _accessControlService = accessControlService;
            _logger = logger;
        }

        public virtual async Task PersistAsync(IdentityServer4.Events.Event evt)
        {
            if (evt is AHIUserLoginSuccessEvent userLoginSuccessEvent)
            {
                _logger.LogInformation($"{userLoginSuccessEvent.CorrelationId} Start PersistAsync AHIUserLoginSuccessEvent");
                _userContext.SetUpn(userLoginSuccessEvent.Username);
                await SetApplicationName(userLoginSuccessEvent.ApplicationId, userLoginSuccessEvent.CorrelationId);
                await _auditLogService.SendLogAsync(ActivityEntityAction.CREDENTIAL, ActivitiesLogEventAction.Login, ActionStatus.Success, userLoginSuccessEvent.Username, userLoginSuccessEvent.DisplayName);
                _logger.LogInformation($"{userLoginSuccessEvent.CorrelationId} End PersistAsync AHIUserLoginSuccessEvent");
            }
            else if (evt is AHIUserLoginFailureEvent userLoginFailureEvent)
            {
                _userContext.SetUpn(userLoginFailureEvent.Username);
                await SetApplicationName(userLoginFailureEvent.ApplicationId, Guid.NewGuid());
                await _auditLogService.SendLogAsync(ActivityEntityAction.CREDENTIAL, ActivitiesLogEventAction.Login, ActionStatus.Fail, userLoginFailureEvent.Username, userLoginFailureEvent.DisplayName);
            }
            else if (evt is AHIUserLogoutSuccessEvent userLogoutSuccessEvent)
            {
                _userContext.SetUpn(userLogoutSuccessEvent.SubjectId);
                await SetApplicationName(userLogoutSuccessEvent.ApplicationId, Guid.NewGuid());
                await _auditLogService.SendLogAsync(ActivityEntityAction.CREDENTIAL, ActivitiesLogEventAction.Logout, ActionStatus.Success, userLogoutSuccessEvent.SubjectId, userLogoutSuccessEvent.DisplayName);
            }
        }

        private async Task SetApplicationName(string applicationId, Guid correlationId)
        {
            var applicationName = await _accessControlService.GetApplicationNameAsync(applicationId, correlationId);
            _logger.LogDebug($"{correlationId} ApplicationId: {applicationId} - ApplicationName: {applicationName}");
            _auditLogService.SetPropertyName(applicationName);
        }
    }
}
