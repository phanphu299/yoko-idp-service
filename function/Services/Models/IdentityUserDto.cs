using System;

namespace Function.Model
{
    public class IdentityUserDto
    {
        public string Upn { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public bool MFA { get; set; } = false;
        public bool RequiredChangePassword { get; set; } = false;
        public DateTime LastPasswordChangeUtc { get; set; }
        public string UserTypeCode { get; set; }
        public bool IsLocked { get; set; }
    }
}