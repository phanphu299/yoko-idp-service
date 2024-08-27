using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using AHI.Infrastructure.Exception;
using AHI.Infrastructure.Exception.Helper;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.Service;
using AHI.Infrastructure.SharedKernel.Abstraction;
using IdpServer.Application.Constant;
using IdpServer.Application.Enum;
using IdpServer.Application.Event;
using IdpServer.Application.Model;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Application.Service.Abstraction;
using IdpServer.Application.User.Command;
using IdpServer.Application.User.Model;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using AHI.Infrastructure.Audit.Service.Abstraction;
using AHI.Infrastructure.UserContext.Abstraction;
using AHI.Infrastructure.Audit.Constant;
using Newtonsoft.Json;
using IdpServer.Application.Extension;
using AHI.Infrastructure.Audit.Model;
using AHI.Infrastructure.MultiTenancy.Extension;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Net;

namespace IdpServer.Application.Service
{
    public class UserService : BaseSearchService<Domain.Entity.User, string, SearchUser, UserDto>, IUserService
    {
        private readonly IIdpUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly ITenantContext _tenantContext;
        private readonly ICachePersistedGrantStore _persistedGrantStore;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILoggerAdapter<UserService> _logger;
        private readonly IMasterService _masterService;
        private readonly IAuditLogService _auditLogService;
        private readonly IUserContext _userContext;
        private readonly INotificationService _notificationService;
        private readonly ISAPCDCService _sapCDCService;
        private readonly IPasswordHasher<UserDto> _hasher;
        private readonly IConfigurationService _configurationService;

        public UserService(
                            IServiceProvider serviceProvider,
                            IIdpUnitOfWork unitOfWork,
                            IEmailService emailService,
                            IConfiguration configuration,
                            ITokenService tokenService,
                            ITenantContext tenantContext,
                            IHttpClientFactory httpClientFactory,
                            ICachePersistedGrantStore persistedGrantStore,
                            ILoggerAdapter<UserService> logger,
                            IAuditLogService auditLogService,
                            IUserContext userContext,
                            INotificationService notificationService,
                            IMasterService masterService,
                            IConfigurationService configurationService,
                            ISAPCDCService sapCDCService,
                             IPasswordHasher<UserDto> hasher)
                            : base(UserDto.Create, serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailService = emailService;
            _tokenService = tokenService;
            _tenantContext = tenantContext;
            _persistedGrantStore = persistedGrantStore;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _masterService = masterService;
            _auditLogService = auditLogService;
            _userContext = userContext;
            _notificationService = notificationService;
            _hasher = hasher;
            _configurationService = configurationService;
            _sapCDCService = sapCDCService;
        }

        public async Task<UserDto> FindByUpnAsync(string userName, bool ignoreQueryFilters = false)
        {
            Guid correlationId = Guid.NewGuid();
            _logger.LogInformation($"{correlationId} FindByUpnAsync - Step 1: Start");
            var entity = (Domain.Entity.User)null;
            var query = _unitOfWork.Users.AsQueryable().AsNoTracking().Where(u => u.Id == userName);
            if (ignoreQueryFilters)
                entity = await query.IgnoreQueryFilters().FirstOrDefaultAsync();
            else
                entity = await query.FirstOrDefaultAsync();

            _logger.LogInformation($"{correlationId} FindByUpnAsync - Step 2: End");
            return UserDto.Create(entity);
        }

        public async Task<UserDto> FindByIdAsync(Guid userId, bool ignoreQueryFilters = false)
        {
            var entity = await _unitOfWork.Users.FindUserByUserIdAsync(userId, ignoreQueryFilters);
            return UserDto.Create(entity);
        }

        public async Task RequestSetPasswordAsync(UserDto user, TokenTypeEnum tokenType, string redirectUrl, bool forceResetPassword = false)
        {
            await _tokenService.DeleteTokenByTypeAsync(user.Upn, TokenTypeEnum.SetPassword);
            await _tokenService.DeleteTokenByTypeAsync(user.Upn, TokenTypeEnum.ResetPassword);
            var token = await _tokenService.GenerateTokenAsync(user.Upn, tokenType, redirectUrl);
            string accessUrl = $"{_configuration["DefaultEmail:LoginWebHost"]}/Account/SetPassword?token={HttpUtility.UrlEncode(token.TokenKey)}&username={HttpUtility.UrlEncode(user.Upn)}&type={HttpUtility.UrlEncode(tokenType.ToString())}";
            var customField = new Dictionary<string, object>()
                            {
                                { "Email", user.Upn },
                                { "AccessUrl", accessUrl }
                            };
            _tenantContext.RetrieveFromString(user.TenantId, user.SubscriptionId);
            if (forceResetPassword)
            {
                await _unitOfWork.BeginTransactionAsync();
                var entity = await _unitOfWork.Users.AsQueryable().FirstOrDefaultAsync(u => u.Id == user.Upn);
                entity.RequiredChangePassword = true;
                await _unitOfWork.Users.UpdateAsync(entity.Id, entity);
                await _unitOfWork.CommitAsync();
            }
            switch (tokenType)
            {
                case TokenTypeEnum.ResetPassword:
                    await _emailService.SendEmailAsync(user.Upn, EmailTypeCode.RESET_PASSWORD, customField);
                    break;

                case TokenTypeEnum.SetPassword:
                    await _emailService.SendEmailAsync(user.Upn, EmailTypeCode.WELCOME, customField);
                    break;

                default:
                    break;
            }
        }

        public async Task<(bool Result, string Message)> IsValidPasswordAsync(UserDto user, string newPassword, string currentPassword = "")
        {
            // validate password complexity
            var passwordPolicies = await GetPasswordPolicyAsync(user.TenantId, user.SubscriptionId);

            var passwordLengthEnable = Boolean.Parse(passwordPolicies.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_LENGTH_ENABLED).Value);
            var minLength = int.Parse(passwordPolicies.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_LENGTH_MIN).Value);
            var maxLength = int.Parse(passwordPolicies.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_LENGTH_MAX).Value);
            var complexityEnable = Boolean.Parse(passwordPolicies.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_COMPLEXITY_ENABLED).Value);
            var minLowerCaseChar = int.Parse(passwordPolicies.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_COMPLEXITY_LOWERCASE_MIN).Value);
            var minUpperCaseChar = int.Parse(passwordPolicies.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_COMPLEXITY_UPPERCASE_MIN).Value);
            var minNumericChar = int.Parse(passwordPolicies.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_COMPLEXITY_NUMERIC_CHAR_MIN).Value);
            var minSpecialChar = int.Parse(passwordPolicies.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_COMPLEXITY_SPECIAL_CHAR_MIN).Value);
            var passwordHistoryEnabled = Boolean.Parse(passwordPolicies.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_HISTORY_ENABLED).Value);
            var passwordHistoryCount = int.Parse(passwordPolicies.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_HISTORY_NUMBER).Value);
            var passwordHistoryLetterChange = int.Parse(passwordPolicies.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_HISTORY_LETTER_CHANGE_NUMBER).Value);
            StringBuilder regexBuilder = new StringBuilder("^");

            bool complexityCheck = true;
            if (complexityEnable)
            {
                int complexityRuleCount = 0;
                int currentCount = 0;
                if (minLowerCaseChar > -1)
                {
                    complexityRuleCount++;
                    if (newPassword.Where(c => char.IsLower(c)).Count() >= minLowerCaseChar)
                    {
                        currentCount++;
                    }
                }
                if (minUpperCaseChar > -1)
                {
                    complexityRuleCount++;
                    if (newPassword.Where(c => char.IsUpper(c)).Count() >= minUpperCaseChar)
                    {
                        currentCount++;
                    }
                }
                if (minNumericChar > -1)
                {
                    complexityRuleCount++;
                    if (newPassword.Where(c => char.IsNumber(c)).Count() >= minNumericChar)
                    {
                        currentCount++;
                    }
                }
                if (minSpecialChar > -1)
                {
                    complexityRuleCount++;
                    if (newPassword.Where(c => !char.IsLetterOrDigit(c)).Count() >= minSpecialChar)
                    {
                        currentCount++;
                    }
                }
                // BA defined the 3 / 4 rule count
                if (complexityRuleCount >= 3)
                {
                    if (currentCount < 3)
                        complexityCheck = false;
                }
                else if (currentCount < complexityRuleCount)
                {
                    complexityCheck = false;
                }
            }

            if (!complexityCheck)
            {
                return (false, MessageConstants.PASSWORD_POLICY_COMPLEXITY_NOT_MATCH);
            }

            if (passwordLengthEnable && (newPassword.Length < minLength || newPassword.Length > maxLength))
            {
                return (false, MessageConstants.PASSWORD_POLICY_LENGTH_NOT_MATCH);
            }

            if (!string.IsNullOrEmpty(user.FirstName) && newPassword.ToLowerInvariant().Contains(user.FirstName.ToLowerInvariant()) ||
                !string.IsNullOrEmpty(user.LastName) && newPassword.ToLowerInvariant().Contains(user.LastName.ToLowerInvariant()) ||
                newPassword.ToLowerInvariant().Contains(user.Upn.ToLowerInvariant()))
            {
                return (false, MessageConstants.PASSWORD_POLICY_PASS_MATCH_NAME);
            }

            // validate password
            if (passwordHistoryEnabled)
            {
                if (passwordHistoryCount > -1)
                {
                    var entity = await _unitOfWork.Users.AsQueryable().Include(x => x.PasswordHistories).FirstAsync(u => u.Id == user.Upn);
                    foreach (var history in entity.PasswordHistories.OrderByDescending(x => x.CreatedDate).Take(passwordHistoryCount))
                    {
                        var verify = _hasher.VerifyHashedPassword(user, history.Password, newPassword);
                        if (verify == PasswordVerificationResult.Success) // Password is the same as old password
                        {
                            return (false, MessageConstants.PASSWORD_POLICY_HISTORY_FAILED);
                        }
                    }
                }
                if (passwordHistoryLetterChange > -1 && !string.IsNullOrEmpty(currentPassword))
                {
                    int differentCharsCount = 0;
                    if (newPassword.Equals(currentPassword))
                    {
                        return (false, MessageConstants.PASSWORD_POLICY_COMPLEXITY_NOT_MATCH);
                    }
                    for (int i = 0; i < newPassword.Length; i++)
                    {
                        if (i >= currentPassword.Length || newPassword[i] != currentPassword[i])
                        {
                            differentCharsCount++;
                        }
                    }
                    if (differentCharsCount < passwordHistoryLetterChange)
                    {
                        return (false, MessageConstants.PASSWORD_POLICY_COMPLEXITY_NOT_MATCH);
                    }
                }
            }
            return (true, "success");
        }

        public async Task<IDictionary<string, string>> GetPasswordPolicyAsync(string tenantId, string subscriptionId)
        {
            var result = new Dictionary<string, string> { };
            string[] keys = { SystemConfigKeys.ACCOUNT_LOCKOUT_ENABLED,
                                SystemConfigKeys.ACCOUNT_LOCKOUT_ATTEMPT,
                                SystemConfigKeys.ACCOUNT_LOCKOUT_DURATION,
                                SystemConfigKeys.PASSWORD_COMPLEXITY_ENABLED,
                                SystemConfigKeys.PASSWORD_COMPLEXITY_LOWERCASE_MIN,
                                SystemConfigKeys.PASSWORD_COMPLEXITY_UPPERCASE_MIN,
                                SystemConfigKeys.PASSWORD_COMPLEXITY_NUMERIC_CHAR_MIN,
                                SystemConfigKeys.PASSWORD_COMPLEXITY_SPECIAL_CHAR_MIN,
                                SystemConfigKeys.PASSWORD_EXPIRATION_ENABLED,
                                SystemConfigKeys.PASSWORD_EXPIRATION_DAY,
                                SystemConfigKeys.PASSWORD_HISTORY_ENABLED,
                                SystemConfigKeys.PASSWORD_HISTORY_LETTER_CHANGE_NUMBER,
                                SystemConfigKeys.PASSWORD_HISTORY_NUMBER,
                                SystemConfigKeys.PASSWORD_LENGTH_ENABLED,
                                SystemConfigKeys.PASSWORD_LENGTH_MIN,
                                SystemConfigKeys.PASSWORD_LENGTH_MAX };

            _tenantContext.RetrieveFromString(tenantId, subscriptionId);
            var systemConfigs = await _configurationService.GetConfigsAsync(string.Join(',', keys));

            result.Add(SystemConfigKeys.ACCOUNT_LOCKOUT_ENABLED, systemConfigs.FirstOrDefault(x => x.Key == SystemConfigKeys.ACCOUNT_LOCKOUT_ENABLED)?.Value ?? DefaultOptions.ACCOUNT_LOCKOUT_ENABLED);
            result.Add(SystemConfigKeys.ACCOUNT_LOCKOUT_ATTEMPT, systemConfigs.FirstOrDefault(x => x.Key == SystemConfigKeys.ACCOUNT_LOCKOUT_ATTEMPT)?.Value ?? DefaultOptions.ACCOUNT_LOCKOUT_ATTEMPT);
            result.Add(SystemConfigKeys.ACCOUNT_LOCKOUT_DURATION, systemConfigs.FirstOrDefault(x => x.Key == SystemConfigKeys.ACCOUNT_LOCKOUT_DURATION)?.Value ?? DefaultOptions.ACCOUNT_LOCKOUT_DURATION);
            result.Add(SystemConfigKeys.PASSWORD_COMPLEXITY_ENABLED, systemConfigs.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_COMPLEXITY_ENABLED)?.Value ?? DefaultOptions.PASSWORD_COMPLEXITY_ENABLED);
            result.Add(SystemConfigKeys.PASSWORD_COMPLEXITY_LOWERCASE_MIN, systemConfigs.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_COMPLEXITY_LOWERCASE_MIN)?.Value ?? DefaultOptions.PASSWORD_COMPLEXITY_LOWERCASE_MIN);
            result.Add(SystemConfigKeys.PASSWORD_COMPLEXITY_UPPERCASE_MIN, systemConfigs.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_COMPLEXITY_UPPERCASE_MIN)?.Value ?? DefaultOptions.PASSWORD_COMPLEXITY_UPPERCASE_MIN);
            result.Add(SystemConfigKeys.PASSWORD_COMPLEXITY_NUMERIC_CHAR_MIN, systemConfigs.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_COMPLEXITY_NUMERIC_CHAR_MIN)?.Value ?? DefaultOptions.PASSWORD_COMPLEXITY_NUMERIC_CHAR_MIN);
            result.Add(SystemConfigKeys.PASSWORD_COMPLEXITY_SPECIAL_CHAR_MIN, systemConfigs.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_COMPLEXITY_SPECIAL_CHAR_MIN)?.Value ?? DefaultOptions.PASSWORD_COMPLEXITY_SPECIAL_CHAR_MIN);
            result.Add(SystemConfigKeys.PASSWORD_EXPIRATION_ENABLED, systemConfigs.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_EXPIRATION_ENABLED)?.Value ?? DefaultOptions.PASSWORD_EXPIRATION_ENABLED);
            result.Add(SystemConfigKeys.PASSWORD_EXPIRATION_DAY, systemConfigs.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_EXPIRATION_DAY)?.Value ?? DefaultOptions.PASSWORD_EXPIRATION_DAY);
            result.Add(SystemConfigKeys.PASSWORD_HISTORY_ENABLED, systemConfigs.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_HISTORY_ENABLED)?.Value ?? DefaultOptions.PASSWORD_HISTORY_ENABLED);
            result.Add(SystemConfigKeys.PASSWORD_HISTORY_LETTER_CHANGE_NUMBER, systemConfigs.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_HISTORY_LETTER_CHANGE_NUMBER)?.Value ?? DefaultOptions.PASSWORD_HISTORY_LETTER_CHANGE_NUMBER);
            result.Add(SystemConfigKeys.PASSWORD_HISTORY_NUMBER, systemConfigs.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_HISTORY_NUMBER)?.Value ?? DefaultOptions.PASSWORD_HISTORY_NUMBER);
            result.Add(SystemConfigKeys.PASSWORD_LENGTH_ENABLED, systemConfigs.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_LENGTH_ENABLED)?.Value ?? DefaultOptions.PASSWORD_LENGTH_ENABLED);
            result.Add(SystemConfigKeys.PASSWORD_LENGTH_MIN, systemConfigs.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_LENGTH_MIN)?.Value ?? DefaultOptions.PASSWORD_LENGTH_MIN);
            result.Add(SystemConfigKeys.PASSWORD_LENGTH_MAX, systemConfigs.FirstOrDefault(x => x.Key == SystemConfigKeys.PASSWORD_LENGTH_MAX)?.Value ?? DefaultOptions.PASSWORD_LENGTH_MAX);

            return result;
        }

        public async Task<Guid> CreateUserAsync(CreateUser user)
        {
            var entityToCreate = CreateUser.Create(user);
            var requestSendEmail = user.UserTypeCode == UserTypes.LOCAL ? true : false;
            var entity = await _unitOfWork.Users.AsQueryable().IgnoreQueryFilters().Where(x => x.Id == user.Upn).FirstOrDefaultAsync();
            _tenantContext.RetrieveFromString(user.TenantId, user.SubscriptionId);
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (entity != null)
                {
                    requestSendEmail = false;
                    if (user.UserTypeCode == UserTypes.LOCAL)
                    {
                        if (entity.UserTypeCode == UserTypes.LOCAL)
                        {
                            // can not create 2 local users, the valid scenario is: 1 local, the other one is guest
                            if (entity.SubscriptionId != user.SubscriptionId || entity.TenantId != user.TenantId)
                            {
                                throw EntityValidationExceptionHelper.GenerateException(nameof(user.Upn), ExceptionErrorCode.DetailCode.ERROR_VALIDATION_DUPLICATED);
                            }
                        }
                        else
                        {
                            // promote from azure AD or Guest to local account
                            entity.UserTypeCode = user.UserTypeCode;
                            entity.SubscriptionId = user.SubscriptionId;
                            entity.TenantId = user.TenantId;
                            entity.MFA = user.MFA;
                            entity.Deleted = false; //Restore account
                            entity.LoginTypeCode = user.LoginTypeCode;
                            if (string.IsNullOrEmpty(entity.Password))
                                requestSendEmail = true;
                        }
                    }
                }
                else
                {
                    entity = await _unitOfWork.Users.AddAsync(entityToCreate);
                }

                if (user.LoginTypeCode == LoginTypes.SAP_CDC)
                {
                    try
                    {
                        await AddSAPCDCAsync(user, entity.UserId);
                    }
                    catch
                    {
                        throw new SystemCallServiceException(ExceptionErrorCode.ERROR_SYSTEM_CALL_SERVICE);
                    }
                }
                await _unitOfWork.CommitAsync();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Payload: {JsonConvert.SerializeObject(user)}");
                await _unitOfWork.RollbackAsync();
                throw;
            }

            if (requestSendEmail && (user.LoginTypeCode == LoginTypes.AHI_LOCAL || user.LoginTypeCode == LoginTypes.SAP_CDC))
            {
                var token = await _tokenService.GenerateTokenAsync(user.Upn, TokenTypeEnum.SetPassword, _configuration["DefaultEmail:RedirectUrl"]);
                var accessUrl = $"{_configuration["DefaultEmail:LoginWebHost"]}/Account/SetPassword?token={HttpUtility.UrlEncode(token.TokenKey)}&username={HttpUtility.UrlEncode(user.Upn)}&type={HttpUtility.UrlEncode(TokenTypeEnum.SetPassword.ToString())}";
                var customField = new Dictionary<string, object>()
                {
                    { "Email", user.Upn },
                    { "AccessUrl", accessUrl }
                };
                await _emailService.SendEmailAsync(user.Upn, EmailTypeCode.WELCOME, customField);
            }
            return entity.UserId;
        }

        private async Task AddSAPCDCAsync(CreateUser user, Guid userId)
        {
            var addRequest = new SAPCDC.Model.AddAccountRequest(user, _tenantContext, userId);
            var addResponse = await _sapCDCService.AddAsync(addRequest);
            if (addResponse.StatusCode != (int)System.Net.HttpStatusCode.PartialContent)
            {
                if (addResponse.ValidationErrors != null && addResponse.ValidationErrors.Any(x => x.ErrorCode == SAPCDCConstants.ERROR_CODE_EMAIL_ALREADY_EXSITS))
                {
                    var updateRequest = new SAPCDC.Model.UpdateAccountRequest(user, _tenantContext, userId);
                    await ProcessUpdateSAPCDCAsync(updateRequest);
                }
                else
                {
                    throw new SystemCallServiceException(message: addResponse.ErrorMessages);
                }
            }
        }

        private async Task ProcessUpdateSAPCDCAsync(SAPCDC.Model.UpdateAccountRequest request)
        {
            var searchResult = await _sapCDCService.SearchAsync(request.Profile.Email, pageSize: 1);
            if (!searchResult.Results.Any())
            {
                throw new EntityNotFoundException();
            }
            request.UId = searchResult.Results.First().UId;
            await _sapCDCService.UpdateAsync(request);
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.AsQueryable().FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null)
            {
                await _unitOfWork.BeginTransactionAsync();
                user.Deleted = true;
                await _unitOfWork.CommitAsync();
                await RevokeUserSessionAsync(user.Id, user.TenantId, user.SubscriptionId, Guid.NewGuid());
            }
            return user != null;
        }

        public async Task<bool> RevokeUserSessionAsync(string userName, string tenantId, string subscriptionId, Guid? correlationId)
        {
            _logger.LogInformation($"{correlationId} RevokeUserSessionAsync: {userName}");
            _tenantContext.RetrieveFromString(tenantId, subscriptionId);
            await _notificationService.SendNotifyAsync(NotificationConstants.USER_SESSION_ENDPOINT, new UpnNotificationMessage(NotificationConstants.USER_SESSION_TYPE, userName));
            await _persistedGrantStore.RemoveAllAsync(userName);
            return true;
        }

        public async Task<bool> RemoveUserAsync(Guid userId)
        {
            var result = false;
            var user = await _unitOfWork.Users.AsQueryable().IgnoreQueryFilters().FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null)
            {
                await _unitOfWork.BeginTransactionAsync();
                result = await _unitOfWork.Users.RemoveAsync(user.Id);
                await _unitOfWork.CommitAsync();
                await RevokeUserSessionAsync(user.Id, user.TenantId, user.SubscriptionId, Guid.NewGuid());
            }
            return result;
        }

        public async Task RestoreUserAsync(Guid userId)
        {
            await _unitOfWork.BeginTransactionAsync();
            var user = await _unitOfWork.Users.AsQueryable().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null)
            {
                throw new EntityNotFoundException(MessageConstants.ENTITY_NOT_FOUND);
            }
            user.Deleted = false;
            await _unitOfWork.CommitAsync();
        }

        public async Task<UserDto> UpdateUserBasicInfoAsync(UpdateUserInfo updateUserInfo)
        {
            await _unitOfWork.BeginTransactionAsync();
            var user = await _unitOfWork.Users.AsQueryable().FirstOrDefaultAsync(u => u.UserId == updateUserInfo.Id);
            if (user == null)
            {
                throw new EntityNotFoundException(MessageConstants.ENTITY_NOT_FOUND);
            }

            user.Avatar = updateUserInfo.Avatar;
            user.DateTimeFormat = updateUserInfo.DateTimeFormat;
            user.DisplayDateTimeFormat = updateUserInfo.DisplayDateTimeFormat;
            user.TimezoneId = updateUserInfo.TimeZoneId;
            user.FirstName = updateUserInfo.FirstName;
            user.LastName = updateUserInfo.LastName;
            user.LoginTypeCode = updateUserInfo.LoginTypeCode;
            await _unitOfWork.Users.UpdateAsync(user.Id, user);
            await _unitOfWork.CommitAsync();

            if ((updateUserInfo.LoginTypeCode == LoginTypes.AHI_LOCAL || user.LoginTypeCode == LoginTypes.SAP_CDC) && string.IsNullOrEmpty(user.Password))
            {
                _tenantContext.RetrieveFromString(user.TenantId, user.SubscriptionId);
                var token = await _tokenService.GenerateTokenAsync(user.Id, TokenTypeEnum.SetPassword, _configuration["DefaultEmail:RedirectUrl"]);
                var accessUrl = $"{_configuration["DefaultEmail:LoginWebHost"]}/Account/SetPassword?token={HttpUtility.UrlEncode(token.TokenKey)}&username={HttpUtility.UrlEncode(user.Id)}&type={HttpUtility.UrlEncode(TokenTypeEnum.SetPassword.ToString())}";
                var customField = new Dictionary<string, object>()
                {
                    { "Email", user.Id },
                    { "AccessUrl", accessUrl }
                };
                await _emailService.SendEmailAsync(user.Id, EmailTypeCode.WELCOME, customField);
            }

            return UserDto.Create(user);
        }

        public async Task PartialUpdateAsync(PartialUpdateUser command)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var operation in command.JsonPatchDocument.Operations)
                {
                    switch (operation.op)
                    {
                        case "update/profile":
                            await UpdateUserProfileAsync(command.Id, operation);
                            break;

                        case "update/language":
                            await UpdateUserPreferLanguageAsync(command.Id, operation);
                            break;

                        case "update/name":
                            await UpdateUserDisplayNameAsync(command.Id, operation);
                            break;

                        default:
                            throw new EntityInvalidException("Operation is not valid");
                    }
                }
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        //App Portal - Profile page
        private async Task UpdateUserProfileAsync(Guid id, Operation operation)
        {
            var userProfile = JObject.FromObject(operation.value).ToObject<UserPatchDto>();
            var user = await _unitOfWork.Users.AsQueryable().FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
                throw new EntityNotFoundException(MessageConstants.ENTITY_NOT_FOUND);

            user.Avatar = userProfile.Avatar;

            if (userProfile.DateTimeFormatId > 0)
            {
                var timezoneFormats = await _masterService.GetAllDateTimeFormatAsync();
                var format = timezoneFormats.FirstOrDefault(x => x.Id == userProfile.DateTimeFormatId);
                if (format != null)
                {
                    user.DateTimeFormat = format.Format;
                    user.DisplayDateTimeFormat = format.DisplayFormat;
                }
            }

            if (userProfile.TimezoneId > 0)
                user.TimezoneId = userProfile.TimezoneId;
        }

        //All apps - Change Language
        private async Task UpdateUserPreferLanguageAsync(Guid id, Operation operation)
        {
            var user = await _unitOfWork.Users.AsQueryable().FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
                throw new EntityNotFoundException(MessageConstants.ENTITY_NOT_FOUND);

            var userProfile = JObject.FromObject(operation.value).ToObject<UserPatchDto>();
            if (!string.IsNullOrWhiteSpace(userProfile.LanguageCode))
                user.LanguageCode = userProfile.LanguageCode;

            var httpClient = _httpClientFactory.CreateClient(HttpClients.USER_FUNCTION);
            await httpClient.PostAsync($"fnc/usr/users/info?forceUpdate=true&showDialog=false",
                 new StringContent(
                     JsonConvert.SerializeObject(new
                     {
                         upn = user.Email,
                         notificationType = "user/profile"
                     }),
                     System.Text.Encoding.UTF8, "application/json"
                 )
            );
        }

        private async Task UpdateUserDisplayNameAsync(Guid id, Operation operation)
        {
            var user = await _unitOfWork.Users.AsQueryable().FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
                throw new EntityNotFoundException(MessageConstants.ENTITY_NOT_FOUND);

            var userProfile = JObject.FromObject(operation.value).ToObject<UserPatchDto>();
            user.FirstName = userProfile.FirstName;
            user.LastName = userProfile.LastName;
        }

        public async Task UpdateMFAAsync(Guid userId, bool mfa, bool setupMfa)
        {
            var user = await _unitOfWork.Users.AsQueryable().IgnoreQueryFilters().Where(x => x.UserId == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new EntityNotFoundException(MessageConstants.ENTITY_NOT_FOUND);
            }
            if (user.UserTypeCode == UserTypes.LOCAL)
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    user.MFA = mfa;
                    user.SetupMFA = setupMfa;
                    await _unitOfWork.Users.UpdateAsync(user.Id, user);
                    await _unitOfWork.CommitAsync();
                }
                catch
                {
                    await _unitOfWork.RollbackAsync();
                    throw;
                }
            }
        }

        protected override IQueryable<Domain.Entity.User> BuildFilter(SearchUser criteria, IQueryable<Domain.Entity.User> query)
        {
            var filtered = base.BuildFilter(criteria, query);
            if (criteria.Deleted)
            {
                filtered = filtered.IgnoreQueryFilters().Where(x => x.Deleted);
            }
            return filtered;
        }

        protected override Type GetDbType()
        {
            return typeof(IUserRepository);
        }

        public async Task RemoveUserBySubscriptionIdAsync(Guid subscriptionId)
        {
            var subscriptionString = subscriptionId.ToString();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var userIds = await _unitOfWork.Users.AsQueryable().AsNoTracking().Where(x => x.SubscriptionId == subscriptionString).Select(x => x.Id).ToListAsync();
                foreach (var userId in userIds)
                {
                    await _unitOfWork.Users.RemoveAsync(userId);
                }
                await _unitOfWork.CommitAsync();
            }
            catch (System.Exception exc)
            {
                _logger.LogError(exc, $"SubscriptionId: {subscriptionId}");
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<ChangeUserPasswordDto> ChangePasswordAsync(string upn, string loginTypeCode, string newPassword, string currentPassword = "", bool sendEmail = true)
        {
            var user = await FindByUpnAsync(upn);
            var result = new ChangeUserPasswordDto(false, MessageConstants.OPERATION_INVALID, new GenericProcessFailedException(MessageConstants.OPERATION_INVALID));
            if (user == null)
            {
                return new ChangeUserPasswordDto(false, MessageConstants.ENTITY_NOT_FOUND, new EntityNotFoundException());
            }
            switch (user.LoginTypeCode)
            {
                case LoginTypes.AHI_LOCAL:
                    result = await ChangeAHILocalPasswordAsync(user, newPassword, currentPassword);
                    break;
                case LoginTypes.SAP_CDC:
                    result = await ChangeSAPCDCPasswordAsync(user, newPassword, currentPassword);
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (!result.Result)
                return result;

            if (sendEmail)
            {
                var customField = new Dictionary<string, object>()
                        {
                            { "Email", user.Upn }
                        };
                _tenantContext.RetrieveFromString(user.TenantId, user.SubscriptionId);
                await _emailService.SendEmailAsync(user.Upn, EmailTypeCode.CHANGE_PASSWORD_SUCCESS, customField);
            }

            _userContext.SetUpn(user.Upn);
            await _auditLogService.SendLogAsync(ActivityEntityAction.CREDENTIAL, ActivitiesLogEventAction.Change_Password, ActionStatus.Success, user.Upn, user.GetDisplayName());

            //Fire and forget revoke user session.
            _ = Task.Run(async () => await RevokeUserSessionAsync(user.Upn, user.TenantId, user.SubscriptionId, Guid.NewGuid()));

            return result;
        }

        private async Task<ChangeUserPasswordDto> ChangeAHILocalPasswordAsync(UserDto user, string newPassword, string currentPassword = "")
        {
            if (!string.IsNullOrEmpty(currentPassword))
            {
                var verificationResult = _hasher.VerifyHashedPassword(user, user.Password, currentPassword);

                if (verificationResult != PasswordVerificationResult.Success)
                {
                    return new ChangeUserPasswordDto(false,
                                                        MessageConstants.ERROR_VALIDATION_WRONG_CURRENT_PASSWORD,
                                                        EntityValidationExceptionHelper.GenerateException(fieldName: nameof(currentPassword),
                                                                                                            fieldErrorMessage: MessageConstants.ERROR_VALIDATION_WRONG_CURRENT_PASSWORD));
                }
            }

            var validationResult = await IsValidPasswordAsync(user, newPassword, currentPassword);
            if (!validationResult.Result)
            {
                return new ChangeUserPasswordDto(false,
                                                    validationResult.Message,
                                                    EntityValidationExceptionHelper.GenerateException(fieldName: nameof(newPassword),
                                                                                                        fieldErrorMessage: validationResult.Message));
            }
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var entity = await _unitOfWork.Users.AsQueryable().Include(x => x.PasswordHistories).FirstAsync(u => u.Id == user.Upn);
                entity.Password = _hasher.HashPassword(user, newPassword);
                entity.PasswordHistories.Add(new Domain.Entity.PasswordHistory()
                {
                    Upn = entity.Id,
                    Password = entity.Password,
                    CreatedDate = DateTime.UtcNow
                });
                entity.RequiredChangePassword = false;
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            return new ChangeUserPasswordDto(true, MessageConstants.SUCCESS);
        }

        private async Task<ChangeUserPasswordDto> ChangeSAPCDCPasswordAsync(UserDto user, string newPassword, string currentPassword = "")
        {
            if (!string.IsNullOrEmpty(currentPassword))
            {
                var loginResult = await _sapCDCService.LoginAsync(user.Upn, currentPassword);
                if (loginResult.StatusCode != (int)HttpStatusCode.OK)
                {
                    return new ChangeUserPasswordDto(false,
                                                    MessageConstants.ERROR_VALIDATION_WRONG_CURRENT_PASSWORD,
                                                    EntityValidationExceptionHelper.GenerateException(fieldName: nameof(currentPassword),
                                                                                                        fieldErrorMessage: MessageConstants.ERROR_VALIDATION_WRONG_CURRENT_PASSWORD));
                }
            }
            var response = await _sapCDCService.ResetPasswordByUpnAsync(user.Upn, newPassword);
            if (response.StatusCode != (int)HttpStatusCode.OK && response.ErrorCode == SAPCDCConstants.ERROR_CODE_PASSWORD_INVALID)
            {
                return new ChangeUserPasswordDto(false,
                                                    MessageConstants.PASSWORD_POLICY_COMPLEXITY_NOT_MATCH,
                                                    EntityValidationExceptionHelper.GenerateException(fieldName: nameof(currentPassword),
                                                                                                        fieldErrorMessage: MessageConstants.PASSWORD_POLICY_COMPLEXITY_NOT_MATCH));
            }
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var entity = await _unitOfWork.Users.AsQueryable().FirstOrDefaultAsync(u => u.Id == user.Upn);
                entity.RequiredChangePassword = false;
                await _unitOfWork.Users.UpdateAsync(entity.Id, entity);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            return new ChangeUserPasswordDto(true, MessageConstants.SUCCESS);
        }
    }
}