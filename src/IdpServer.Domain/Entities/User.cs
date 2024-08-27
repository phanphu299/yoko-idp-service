using System;
using System.Collections.Generic;
using AHI.Infrastructure.Repository.Model.Generic;

namespace IdpServer.Domain.Entity
{
    public class User : IEntity<string>
    {
        public string Id { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // public bool IsLocked { get; set; }
        public bool RequiredChangePassword { get; set; }
        public bool MFA { get; set; }
        public string UserTypeCode { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public UserType UserType { get; set; }
        public string Avatar { get; set; }
        public string DateTimeFormat { get; set; }
        public string DisplayDateTimeFormat { get; set; }
        public int TimezoneId { get; set; }
        public string CountryCode { get; set; }
        public string LanguageCode { get; set; }
        public string DefaultPage { get; set; }
        public bool Deleted { get; set; }
        public string LoginTypeCode { get; set; }
        public LoginType LoginType { get; set; }
        public ICollection<PasswordHistory> PasswordHistories { get; set; }
        public bool SetupMFA { get; set; }
    }
}
