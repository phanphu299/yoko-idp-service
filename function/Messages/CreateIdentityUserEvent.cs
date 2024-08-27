using AHI.Infrastructure.Bus.ServiceBus.Abstraction;

namespace Function.Message
{
    public class CreateIdentityUserMessage : BusEvent
    {
        public string UserName { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public string DefaultSubscriptionId { get; set; }
        public override string TopicName => "";
    }
}
