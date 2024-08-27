using System;

namespace Function.Model
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public bool Locked { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Upn { get; set; }
        public bool ForceResetPassword { get; set; }
        public bool MFA { get; set; }
        public string UserTypeCode { get; set; } = "la";
        public string LoginTypeCode { get; set; } = "ahi-local";
    }
}
