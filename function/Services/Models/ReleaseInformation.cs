using System;

namespace Function.Model
{
    public class ReleaseInformation
    {
        public string TenantId { get; set; }
        public Guid SubTenantId { get; set; }
        public string TenantDomain { get; set; }
        public string SubTenantDomain { get; set; }
        public string ApplicationName { get; set; }
        public int ApplicationId { get; set; }
        public string ServiceName { get; set; }
        public string ProjectName { get; set; }
        public int DefinitionId { get; set; }
        public string ReleaseName { get; set; }
        public string ArtifactName { get; set; }
        public string BuildVersion { get; set; }
    }
}