using System;
using AHI.Infrastructure.Repository.Model.Generic;

namespace IdpServer.Domain.Entity
{
    public class ProjectClient : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public int ClientId { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public string ProjectId { get; set; }

        public virtual Client Client { get; set; }
    }
}
