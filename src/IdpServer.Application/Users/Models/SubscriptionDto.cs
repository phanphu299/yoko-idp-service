using System.Collections.Generic;

namespace IdpServer.Application.User.Model
{
    public class SubscriptionDto
    {
        public string TenantId { get; set; }
        public string Id { get; set; }
        public IEnumerable<ApplicationDto> Applications { get; set; }
        public SubscriptionDto()
        {
            Applications = new List<ApplicationDto>();
        }
    }
}