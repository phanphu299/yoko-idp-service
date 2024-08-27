using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AHI.Infrastructure.Cache.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using IdpServer.Application.Constant;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Application.Service.Abstraction;
using IdpServer.Application.User.Model;
using Newtonsoft.Json;

namespace IdpServer.Application.Service
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICache _cache;
        private readonly IIdpUnitOfWork _unitOfWork;
        private readonly ILoggerAdapter<UserInfoService> _logger;
        public UserInfoService(IHttpClientFactory httpClientFactory, ICache cache, IIdpUnitOfWork unitOfWork, ILoggerAdapter<UserInfoService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task ForceUpdateUserInfoAsync(Guid userId, string upn)
        {
            // var key = $"idp_user_{userId}";
            // var userEntity = await _unitOfWork.Users.FindUserByUserIdAsync(userId);
            // if (userEntity == null)
            // {
            //     throw new EntityNotFoundException(MessageConstants.ENTITY_NOT_FOUND);
            // }
            // var userDto = new UserDto()
            // {
            //     Id = userEntity.UserId,
            //     Upn = userEntity.Id,
            //     UserId = userEntity.UserId,
            //     FirstName = userEntity.FirstName,
            //     LastName = userEntity.LastName,
            //     UserTypeCode = userEntity.UserTypeCode,
            //     TenantId = userEntity.TenantId,
            //     SubscriptionId = userEntity.SubscriptionId,
            //     MFA = userEntity.MFA,
            //     Email = userEntity.Email
            // };
            // await _cache.StoreAsync(key, userDto);
            await GetOrUpdateUserInfoAsync(upn, true);
        }

        public Task<UserInfoDto> GetUserInfoAsync(string upn)
        {
            return GetOrUpdateUserInfoAsync(upn, false);
        }

        private async Task<UserInfoDto> GetOrUpdateUserInfoAsync(string upn, bool forceUpdate)
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClients.USER_FUNCTION);
            var payload = await httpClient.PostAsync($"fnc/usr/users/info?forceUpdate={forceUpdate}",
                 new StringContent(
                     JsonConvert.SerializeObject(new
                     {
                         Upn = upn,
                         NotificationType = "user/profile"
                     }),
                     System.Text.Encoding.UTF8, "application/json"
                 )
             );
            payload.EnsureSuccessStatusCode();
            var stream = await payload.Content.ReadAsByteArrayAsync();
            return stream.Deserialize<UserInfoDto>();
        }

        public async Task<IEnumerable<User.Model.SubscriptionDto>> GetUserSubscriptionsAsync(Guid userId)
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClients.USER_FUNCTION);
            var payload = await httpClient.PostAsync($"fnc/usr/users/subscriptions",
                 new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        Id = userId
                    }),
                    System.Text.Encoding.UTF8, "application/json"
                 )
             );
            payload.EnsureSuccessStatusCode();
            var stream = await payload.Content.ReadAsByteArrayAsync();
            return stream.Deserialize<IEnumerable<User.Model.SubscriptionDto>>();
        }
    }
}
