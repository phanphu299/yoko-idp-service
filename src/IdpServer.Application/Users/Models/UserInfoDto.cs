using System;
using System.Collections.Generic;

namespace IdpServer.Application.User.Model
{
    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Upn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string HomeTenantId { get; set; }
        public string HomeSubscriptionId { get; set; }
        public IEnumerable<SubscriptionDto> Subscriptions { get; set; }
        public bool IsLocked { get; set; }
        public UserInfoDto()
        {
            Subscriptions = new List<SubscriptionDto>();
        }
    }
}