using System;
using AHI.Infrastructure.Repository.Model.Generic;

namespace IdpServer.Domain.Entity
{
    public class PasswordHistory : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Upn { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
        public User User { get; private set; }
    }
}