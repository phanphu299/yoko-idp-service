using System;
using System.Threading.Tasks;
using IdpServer.Application.Enum;
using IdpServer.Application.Model;
using IdpServer.Application.User.Command;
using AHI.Infrastructure.Service.Abstraction;
using System.Collections.Generic;
using AHI.Infrastructure.SharedKernel.Model;
using IdpServer.Application.User.Model;

namespace IdpServer.Application.Service.Abstraction
{
    public interface IUserService : ISearchService<Domain.Entity.User, string, SearchUser, UserDto>
    {
        Task<UserDto> FindByUpnAsync(string userName, bool ignoreQueryFilters = false);
        Task<UserDto> FindByIdAsync(Guid userId, bool ignoreQueryFilters = false);
        Task RequestSetPasswordAsync(UserDto user, TokenTypeEnum tokenType, string redirectUrl, bool forceResetPassword = false);
        Task<(bool Result, string Message)> IsValidPasswordAsync(UserDto user, string newPassword, string currentPassword = "");
        Task<Guid> CreateUserAsync(CreateUser user);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<bool> RemoveUserAsync(Guid userId);
        Task<bool> RevokeUserSessionAsync(string userName, string tenantId, string subscriptionId, Guid? correlationId);
        Task RemoveUserBySubscriptionIdAsync(Guid subscriptionId);
        Task RestoreUserAsync(Guid userId);
        Task<UserDto> UpdateUserBasicInfoAsync(UpdateUserInfo updateUserInfo);
        Task PartialUpdateAsync(PartialUpdateUser command);
        Task UpdateMFAAsync(Guid userId, bool mfa, bool setupMfa);
        Task<IDictionary<string, string>> GetPasswordPolicyAsync(string tenantId, string subscriptionId);
        Task<ChangeUserPasswordDto> ChangePasswordAsync(string upn, string loginTypeCode, string newPassword, string currentPassword = "", bool sendEmail = true);
    }
}
