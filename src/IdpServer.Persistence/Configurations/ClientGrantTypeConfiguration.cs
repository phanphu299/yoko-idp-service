using IdpServer.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpServer.Persistence.Configurations
{
    public class ClientGrantTypeConfiguration : IEntityTypeConfiguration<ClientGrantType>
    {
        public void Configure(EntityTypeBuilder<ClientGrantType> builder)
        {
            builder.ToTable("ClientGrantTypes");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("Id");
            builder.Property(x => x.ClientId).HasColumnName("ClientId");
            builder.Property(x => x.GrantType).HasColumnName("GrantType");
        }
    }
}
