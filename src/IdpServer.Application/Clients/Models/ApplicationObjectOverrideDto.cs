using System;

namespace IdpServer.Application.Client.Model
{
    public class ApplicationObjectOverrideDto
    {
        public Guid ApplicationId { get; set; }
        public string EntityCode { get; set; }
        public string PrivilegeCode { get; set; }
        public string ProjectId { get; set; }
        public bool IsSelected { get; set; }
        public string ObjectId { get; set; }
        public string ObjectName { get; set; }
        public Guid TargetId { get; set; }
        public string TargetType { get; set; }
        public bool IsCascaded { get; set; }
        public PrivilegeDto Privilege { get; set; }
    }
}
