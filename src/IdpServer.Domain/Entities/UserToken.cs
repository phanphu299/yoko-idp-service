using System;
using AHI.Infrastructure.Repository.Model.Generic;

namespace IdpServer.Domain.Entity
{
    public class UserToken : IEntity<int>
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string TokenKey { get; set; }
        public string TokenType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public string RedirectUrl { get; set; }
        public bool Deleted { get; set; }
        public int ClickCount { get; set; }
        public int MaxClickCount { get; set; } = 5;
    }
}