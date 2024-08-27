using IdpServer.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpServer.Persistence.Configurations
{
    public class ClientClaimConfiguration : IEntityTypeConfiguration<ClientClaim>
    {
        public void Configure(EntityTypeBuilder<ClientClaim> builder)
        {
            builder.ToTable("ClientClaims");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("Id");
            builder.Property(x => x.ClientId).HasColumnName("ClientId");
            builder.Property(x => x.Type).HasColumnName("Type");
            builder.Property(x => x.Value).HasColumnName("Value");
        }
    }
}
