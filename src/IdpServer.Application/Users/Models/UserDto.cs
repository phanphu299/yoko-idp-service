using System;
using System.Linq.Expressions;

namespace IdpServer.Application.Model
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Upn { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public bool IsLocked { get; set; }
        public bool RequiredChangePassword { get; set; }
        public bool MFA { get; set; }
        public string UserTypeCode { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public string Avatar { get; set; }
        public string DateTimeFormat { get; set; }
        public string DisplayDateTimeFormat { get; set; }
        public int TimezoneId { get; set; }
        public string CountryCode { get; set; }
        public string LanguageCode { get; set; }
        public bool Deleted { get; set; }
        public string LoginTypeCode { get; set; }
        public string DefaultPage { get; set; }
        public bool SetupMFA { get; set; }
        public static Expression<Func<Domain.Entity.User, UserDto>> Projection
        {
            get
            {
                return entity => new UserDto
                {
                    Id = entity.UserId,
                    Upn = entity.Id,
                    UserId = entity.UserId,
                    Email = entity.Email,
                    Password = entity.Password,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    RequiredChangePassword = entity.RequiredChangePassword,
                    MFA = entity.MFA,
                    //IsLocked = entity.IsLocked,
                    UserTypeCode = entity.UserTypeCode,
                    TenantId = entity.TenantId,
                    SubscriptionId = entity.SubscriptionId,
                    Avatar = entity.Avatar,
                    DateTimeFormat = entity.DateTimeFormat,
                    DisplayDateTimeFormat = entity.DisplayDateTimeFormat,
                    TimezoneId = entity.TimezoneId,
                    CountryCode = entity.CountryCode,
                    LanguageCode = entity.LanguageCode,
                    Deleted = entity.Deleted,
                    LoginTypeCode = entity.LoginTypeCode,
                    SetupMFA = entity.SetupMFA,
                    DefaultPage = entity.DefaultPage
                };
            }
        }

        public static UserDto Create(Domain.Entity.User entity)
        {
            if (entity == null)
                return null;
            return Projection.Compile().Invoke(entity);
        }
    }
}