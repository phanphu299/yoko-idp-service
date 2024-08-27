using System;

namespace IdpServer.Application.Client.Model
{
    public class ApplicationProjectClientOverrideDto
    {
        public Guid ApplicationId { get; set; }
        public string EntityCode { get; set; }
        public string PrivilegeCode { get; set; }
        public string ProjectId { get; set; }
        public bool IsSelected { get; set; }
        public PrivilegeDto Privilege { get; set; }
        public Guid ClientId { get; set; }
    }
}
