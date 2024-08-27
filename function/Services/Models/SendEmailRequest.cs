using System.Collections.Generic;

namespace Function.Model
{
    public class SendEmailRequest
    {
        public string TypeCode { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public bool OverrideTemplate { get; set; } = true;
        public IEnumerable<Email> Emails { get; set; }
        public IEnumerable<FileAttachment> Attachments { get; set; }
    }

    public class Email
    {
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public IDictionary<string, object> Payload { get; set; }
    }

    public class FileAttachment
    {
        public string FilePath { get; set; }
        public string Disposition { get; set; }
    }
}