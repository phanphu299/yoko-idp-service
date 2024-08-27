namespace IdentityServer4.Events
{
    public class AHIUserLogoutSuccessEvent : UserLogoutSuccessEvent
    {
        public string ApplicationId { get; set; }
        public AHIUserLogoutSuccessEvent(string subjectId, string name, string applicationId = null) : base (subjectId, name)
        {
            ApplicationId = applicationId;
        }
    }

}