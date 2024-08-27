using IdpServer.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpServer.Persistence.Configurations
{
    public class ClientScopeConfiguration : IEntityTypeConfiguration<ClientScope>
    {
        public void Configure(EntityTypeBuilder<ClientScope> builder)
        {
            builder.ToTable("ClientScopes");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("Id");
            builder.Property(x => x.ClientId).HasColumnName("ClientId");
            builder.Property(x => x.Scope).HasColumnName("Scope");
        }
    }
}
