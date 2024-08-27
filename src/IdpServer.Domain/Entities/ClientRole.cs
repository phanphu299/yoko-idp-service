using System;
using AHI.Infrastructure.Repository.Model.Generic;

namespace IdpServer.Domain.Entity
{
    public class ClientRole : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public Guid ApplicationId { get; set; }
        public string ProjectId { get; set; }
        public Guid RoleId { get; set; }
    }

}