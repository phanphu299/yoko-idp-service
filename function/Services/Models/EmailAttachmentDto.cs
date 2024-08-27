using System;
using System.Collections.Generic;

namespace Function.Model
{
    public class EmailAttachmentDto
    {
        public Guid Id { get; set; }
        public Guid EmailTemplateId { get; set; }
        public string Disposition { get; set; }
        public string FilePath { get; set; }
    }

}