using System;
using AHI.Infrastructure.Repository.Model.Generic;

namespace IdpServer.Domain.Entity
{
    public class PersistedGrant : IEntity<string>
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string SubjectId { get; set; }
        public string ClientId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime Expiration { get; set; }
        public string Data { get; set; }
    }
}
