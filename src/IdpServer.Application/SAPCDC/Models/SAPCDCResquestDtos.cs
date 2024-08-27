using System;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using IdpServer.Application.User.Command;

namespace IdpServer.Application.SAPCDC.Model
{
    public class AddAccountRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public UserProfileDto Profile { get; set; }
        public UserDataDto Data { get; set; }

        public AddAccountRequest(CreateUser command, ITenantContext tenantContext, Guid userId)
        {
            Email = command.Upn.ToLowerInvariant();
            Password = Guid.NewGuid().ToString("N"); // auto populate password for registration process.
            Data = new UserDataDto()
            {
                PhoneNumber = command.PhoneNumber,
                SubscriptionId = tenantContext.SubscriptionId,
                TenantId = tenantContext.TenantId,
                UserId = userId.ToString()
            };
            Profile = new UserProfileDto()
            {
                Email = command.Upn.ToLowerInvariant(),
                FirstName = command.FirstName,
                LastName = command.LastName
            };
        }
    }

    public class UpdateAccountRequest
    {
        public string UId { get; set; }
        public UserDataDto Data { get; set; }
        public UserProfileDto Profile { get; set; }

        public UpdateAccountRequest(CreateUser command, ITenantContext tenantContext, Guid userId)
        {
            Data = new UserDataDto()
            {
                PhoneNumber = command.PhoneNumber,
                SubscriptionId = tenantContext.SubscriptionId,
                TenantId = tenantContext.TenantId,
                UserId = userId.ToString()
            };
            Profile = new UserProfileDto()
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Upn.ToLowerInvariant()
            };
        }
    }
}