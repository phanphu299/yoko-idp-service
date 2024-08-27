namespace IdentityServer4.Events
{
    public class AHIUserLoginFailureEvent : UserLoginFailureEvent
    {
        public string ApplicationId { get; set; }
        public string DisplayName { get; set; }
        public AHIUserLoginFailureEvent(string username, string error, bool interactive = true, string clientId = null, string applicationId = null, string displayName = null) : base (username, error, interactive, clientId)
        {
            ApplicationId = applicationId;
            DisplayName = displayName;
        }
    }

}