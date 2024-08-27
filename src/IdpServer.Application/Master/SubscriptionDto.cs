using System;

namespace IdpServer.Application.Model
{
    public class SubscriptionDto
    {
        public string Id { get; set; }
        public string TenantResourceId { get; set; }
        public string TenantId { get; set; }
        public string Name { get; set; }
        public string LocationId { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public bool IsMigrated { get; set; }
        public bool Deleted { get; set; }
    }
}