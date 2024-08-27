using System;
using System.Collections.Generic;

namespace IdpServer.Application.Client.Model
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Guid> Users { get; set; }
        public IEnumerable<Guid> UserGroups { get; set; }
        public IEnumerable<string> RoleOverrides { get; set; } = new List<string>();
        public IEnumerable<string> ObjectOverrides { get; set; } = new List<string>();
        public IEnumerable<EntityDto> Entities { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public RoleDto()
        {
            Entities = new List<EntityDto>();
            Users = new List<Guid>();
            UserGroups = new List<Guid>();
        }
    }
}
