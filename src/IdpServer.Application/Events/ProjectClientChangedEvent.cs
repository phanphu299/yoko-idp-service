using System;
using AHI.Infrastructure.Bus.ServiceBus.Abstraction;
using AHI.Infrastructure.Bus.ServiceBus.Enum;
using AHI.Infrastructure.MultiTenancy.Abstraction;

namespace IdpServer.Application.Event
{
    public class ClientChangedEvent : BusEvent
    {
        public override string TopicName => "identity.application.event.client.changed";
        public Guid ClientId { get; set; }
        public string ProjectId { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public ClientChangedEvent(Guid clientId, string tenantId, string subscriptionId, string projectId, ActionTypeEnum actionType = ActionTypeEnum.Deleted)
        {
            ClientId = clientId;
            ProjectId = projectId;
            ActionType = actionType;
            TenantId = tenantId;
            SubscriptionId = subscriptionId;
        }
        public static ClientChangedEvent CreateFrom(Domain.Entity.Client entity, ITenantContext tenantContext, ActionTypeEnum actionType = ActionTypeEnum.Created)
        {
            return new ClientChangedEvent(Guid.Parse(entity.ClientId), tenantContext.TenantId, tenantContext.SubscriptionId, tenantContext.ProjectId, actionType);
        }
    }
}
