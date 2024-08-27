using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpServer.Persistence.Configuration
{
    public class ClientConfiguration : IEntityTypeConfiguration<Domain.Entity.Client>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.Client> builder)
        {
            // configure the model.
            builder.ToTable("Clients");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("Id");
            builder.Property(x => x.ClientId).HasColumnName("ClientId");
            builder.Property(x => x.ClientName).HasColumnName("ClientName");
            builder.Property(x => x.Created).HasColumnName("Created");
            builder.HasMany(x => x.ClientSecrets);
            builder.HasMany(x => x.ClientGrantTypes);
            builder.HasMany(x => x.ClientScopes);
            builder.HasMany(x => x.ClientClaims);
            builder.HasMany(x => x.ClientRedirectUris);
            builder.HasMany(x => x.ClientPostLogoutRedirectUris);
            builder.HasMany(e => e.EntityTags).WithOne(x => x.Client).HasForeignKey(x => x.EntityIdInt).OnDelete(DeleteBehavior.Cascade);
        }
    }
}