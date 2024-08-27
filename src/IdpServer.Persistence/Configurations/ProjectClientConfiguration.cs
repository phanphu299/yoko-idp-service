using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpServer.Persistence.Configuration
{
    public class ProjectClientConfiguration : IEntityTypeConfiguration<IdpServer.Domain.Entity.ProjectClient>
    {
        public void Configure(EntityTypeBuilder<IdpServer.Domain.Entity.ProjectClient> builder)
        {
            // configure the model.
            builder.ToTable("project_clients");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.ClientId).HasColumnName("client_id");
            builder.Property(x => x.TenantId).HasColumnName("tenant_id");
            builder.Property(x => x.SubscriptionId).HasColumnName("subscription_id");
            builder.Property(x => x.ProjectId).HasColumnName("project_id");
            builder.HasOne(x => x.Client).WithOne(x => x.ProjectClient);
        }
    }
}
