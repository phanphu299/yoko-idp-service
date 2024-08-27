using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using AHI.Infrastructure.Bus.ServiceBus.Enum;

namespace Function.Message
{
    public class IdentityUserChangedEvent : BusEvent
    {
        public override string TopicName => "identity.application.event.identity.user.changed";
        public string Upn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Locked { get; set; }
        public bool ForceChangePassword { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public IdentityUserChangedEvent()
        {
        }
        public IdentityUserChangedEvent(string upn, string firstName, string lastName, bool locked, bool forceChangePassword, string tenantId, string subscriptionId, ActionTypeEnum actionType = ActionTypeEnum.Updated)
        {
            Upn = upn;
            Locked = locked;
            ForceChangePassword = forceChangePassword;
            ActionType = actionType;
            TenantId = tenantId;
            SubscriptionId = subscriptionId;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
