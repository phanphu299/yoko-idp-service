using System;

namespace Function.Model
{
    public class UserTokenDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string TokenKey { get; set; }
        public string TokenType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string RedirectUrl { get; set; }
        public bool Deleted { get; set; }
    }
}