using System;
using System.Collections.Generic;

namespace Function.Model
{
    public class EmailTemplateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Html { get; set; }
        public string TypeCode { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public IEnumerable<EmailAttachmentDto> Attachments { get; set; }
        public bool Deleted { set; get; }
        
        public EmailTemplateDto()
        {
            CreatedUtc = DateTime.UtcNow;
            UpdatedUtc = DateTime.UtcNow;
            Attachments = new List<EmailAttachmentDto>();
        }
    }
}