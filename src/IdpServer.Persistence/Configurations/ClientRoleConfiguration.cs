using IdpServer.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpServer.Persistence.Configuration
{
    public class ClientRoleConfiguration : IEntityTypeConfiguration<ClientRole>
    {
        public void Configure(EntityTypeBuilder<ClientRole> builder)
        {
            // configure the model.
            builder.ToTable("client_roles");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("id");
            builder.Property(x => x.ClientId).HasColumnName("client_id");
            builder.Property(x => x.TenantId).HasColumnName("tenant_id");
            builder.Property(x => x.SubscriptionId).HasColumnName("subscription_id");
            builder.Property(x => x.ApplicationId).HasColumnName("application_id");
            builder.Property(x => x.RoleId).HasColumnName("role_id");
            builder.Property(x => x.ProjectId).HasColumnName("project_id");
        }
    }
}