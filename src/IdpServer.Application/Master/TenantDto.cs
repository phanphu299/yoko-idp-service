using System;

namespace IdpServer.Application.Model
{
    public class TenantDto
    {
        public string Id { get; set; }
        public string RootTenantId { get; set; }
        public string ResourceId { get; set; }
        public string Name { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }

        public bool IsMigrated { get; set; }
        public bool Deleted { get; set; }
    }
}