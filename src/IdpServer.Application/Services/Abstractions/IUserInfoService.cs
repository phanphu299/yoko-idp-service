using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdpServer.Application.User.Model;

namespace IdpServer.Application.Service.Abstraction
{
    public interface IUserInfoService
    {
        Task<UserInfoDto> GetUserInfoAsync(string upn);
        Task ForceUpdateUserInfoAsync(Guid userId, string upn);
        Task<IEnumerable<User.Model.SubscriptionDto>> GetUserSubscriptionsAsync(Guid userId);
    }
}