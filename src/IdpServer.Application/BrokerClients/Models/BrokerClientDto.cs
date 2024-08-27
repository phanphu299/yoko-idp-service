using System;
using System.Linq.Expressions;

namespace IdpServer.Application.BrokerClient.Model
{
    public class BrokerClientDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime UpdatedUtc { get; set; }
        public DateTime ExpiredUtc { get; set; }
        public string CreatedBy { get; set; }
        public int ExpiredDays { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public string ProjectId { get; set; }
        public bool Deleted { get; set; }

        public BrokerClientDto()
        {
        }

        public static Expression<Func<Domain.Entity.BrokerClient, BrokerClientDto>> Projection
        {
            get
            {
                return entity => new BrokerClientDto
                {
                    Username = entity.Id,
                    Password = entity.Password,
                    CreatedBy = entity.CreatedBy,
                    ExpiredDays = entity.ExpiredDays,
                    CreatedUtc = entity.CreatedUtc,
                    UpdatedUtc = entity.UpdatedUtc,
                    ExpiredUtc = entity.ExpiredUtc,
                    TenantId = entity.TenantId,
                    SubscriptionId = entity.SubscriptionId,
                    ProjectId = entity.ProjectId,
                    Deleted = entity.Deleted
                };
            }
        }

        public static BrokerClientDto Create(Domain.Entity.BrokerClient entity)
        {
            if (entity == null)
                return null;
            return Projection.Compile().Invoke(entity);
        }
    }
}
