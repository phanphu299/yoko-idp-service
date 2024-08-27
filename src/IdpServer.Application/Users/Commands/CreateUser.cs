using System;
using System.Linq.Expressions;
using MediatR;
using IdpServer.Application.Constant;

namespace IdpServer.Application.User.Command
{
    public class CreateUser : IRequest<Guid>
    {
        public string Upn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool MFA { get; set; } = true;
        public string UserTypeCode { get; set; } = UserTypes.LOCAL;
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public int TimezoneId { get; set; } = 108; // 'Singapore Standard Time','(UTC+08:00) Kuala Lumpur, Singapore','+08:00'
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd (HH:mm:ss.fff)";
        public string DisplayDateTimeFormat { get; set; } = "YYYY-MM-DD (HH:mm:ss.SSS)";
        public string Avatar { get; set; }
        public string CountryCode { get; set; } = "SG";
        public string LanguageCode { get; set; } = "en-US";
        public string LoginTypeCode { get; set; }
        public static Expression<Func<CreateUser, Domain.Entity.User>> Projection
        {
            get
            {
                return entity => new Domain.Entity.User
                {
                    Id = entity.Upn,
                    Email = entity.Upn,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    MFA = entity.MFA,
                    UserTypeCode = entity.UserTypeCode,
                    UserId = Guid.NewGuid(),
                    TenantId = entity.TenantId,
                    SubscriptionId = entity.SubscriptionId,
                    TimezoneId = entity.TimezoneId,
                    DateTimeFormat = entity.DateTimeFormat,
                    DisplayDateTimeFormat = entity.DisplayDateTimeFormat,
                    Avatar = entity.Avatar,
                    CountryCode = entity.CountryCode,
                    LanguageCode = entity.LanguageCode,
                    LoginTypeCode = entity.LoginTypeCode
                };
            }
        }

        public static Domain.Entity.User Create(CreateUser command)
        {
            return Projection.Compile().Invoke(command);
        }
    }
}
