using IdpServer.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpServer.Persistence.Configuration
{
    public class BrokerClientConfiguration : IEntityTypeConfiguration<BrokerClient>
    {
        public void Configure(EntityTypeBuilder<BrokerClient> builder)
        {
            // configure the model.
            builder.ToTable("broker_clients");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Password).HasColumnName("password");
            builder.Property(x => x.CreatedUtc).HasColumnName("created_utc");
            builder.Property(x => x.UpdatedUtc).HasColumnName("updated_utc");
            builder.Property(x => x.ExpiredUtc).HasColumnName("expired_utc");
            builder.Property(x => x.CreatedBy).HasColumnName("created_by");
            builder.Property(x => x.TenantId).HasColumnName("tenant_id");
            builder.Property(x => x.SubscriptionId).HasColumnName("subscription_id");
            builder.Property(x => x.ProjectId).HasColumnName("project_id");
            builder.Property(x => x.Deleted).HasColumnName("deleted");
            builder.Property(x => x.ExpiredDays).HasColumnName("expired_days");
            builder.HasQueryFilter(x => !x.Deleted);
        }
    }
}
