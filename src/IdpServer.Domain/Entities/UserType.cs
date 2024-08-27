using System;
using System.Collections.Generic;
using AHI.Infrastructure.Repository.Model.Generic;

namespace IdpServer.Domain.Entity
{
    public class UserType : IEntity<string>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<User> Users { get; private set; }
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
        public bool Deleted { set; get; } = false;
        public UserType()
        {
            CreatedUtc = DateTime.UtcNow;
            UpdatedUtc = DateTime.UtcNow;
            Users = new List<User>();
        }
    }
}
