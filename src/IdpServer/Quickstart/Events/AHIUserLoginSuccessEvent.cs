using System;

namespace IdentityServer4.Events
{
    public class AHIUserLoginSuccessEvent : UserLoginSuccessEvent
    {
        public string ApplicationId { get; set; }
        public Guid CorrelationId { get; set; }

        public AHIUserLoginSuccessEvent(string username, string subjectId, string name, bool interactive = true, string clientId = null, string applicationId = null, Guid? correlationId = null) : base(username, subjectId, name, interactive, clientId)
        {
            ApplicationId = applicationId;
            CorrelationId = correlationId ?? Guid.NewGuid();
        }
    }
}