using System;

namespace Function.Service.Message
{
    public class UserChangedMessage
    {
        public string Upn { get; set; }
        public string OrgName { get; set; }
        public Guid OrgId { get; set; }

        // public override string TopicName => throw new NotImplementedException();
    }
}