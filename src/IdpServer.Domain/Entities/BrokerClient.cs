using System;
using AHI.Infrastructure.Repository.Model.Generic;

namespace IdpServer.Domain.Entity
{
    public class BrokerClient : IEntity<string>
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public DateTime ExpiredUtc { get; set; }
        public string CreatedBy { get; set; }
        public int ExpiredDays { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public string ProjectId { get; set; }
        public bool Deleted { get; set; }
    }
}
