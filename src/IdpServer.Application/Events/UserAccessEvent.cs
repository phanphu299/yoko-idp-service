using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using AHI.Infrastructure.Bus.ServiceBus.Enum;

namespace IdpServer.Application.Event
{
    public class UserAccessEvent : BusEvent
    {
        public override string TopicName => "identity.application.event.user.access";
        public string Upn { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public string ProjectId { get; set; }
        public UserAccessEvent()
        {
        }
        public UserAccessEvent(string upn, string tenantId, string subscriptionId, string projectId, ActionTypeEnum actionType = ActionTypeEnum.Updated)
        {
            Upn = upn;
            ActionType = actionType;
            TenantId = tenantId;
            SubscriptionId = subscriptionId;
            ProjectId = projectId;
        }
    }
}
