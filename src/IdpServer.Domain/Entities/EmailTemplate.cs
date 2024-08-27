using System;
using System.Collections.Generic;
using AHI.Infrastructure.Repository.Model.Generic;

namespace IdpServer.Domain.Entity
{
    public class EmailTemplate : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Html { get; set; }
        public string TypeCode { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public bool Deleted { set; get; }

        public IEnumerable<EmailAttachment> Attachments { get; set; }
        public EmailTemplate()
        {
            CreatedUtc = DateTime.UtcNow;
            UpdatedUtc = DateTime.UtcNow;
            Attachments = new List<EmailAttachment>();
        }
    }
}