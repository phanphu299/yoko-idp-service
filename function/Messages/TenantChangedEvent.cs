using System;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;

namespace Function.Message
{
    public class TenantChangedEvent : BusEvent
    {
        public override string TopicName => "";
        public Guid Id { get; }
        public string Name { get; }
        public string Email { get; set; }
    }
}
