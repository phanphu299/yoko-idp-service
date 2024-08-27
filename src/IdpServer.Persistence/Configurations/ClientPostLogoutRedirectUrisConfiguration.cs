using IdpServer.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpServer.Persistence.Configurations
{
    public class ClientPostLogoutRedirectUrisConfiguration : IEntityTypeConfiguration<ClientPostLogoutRedirectUris>
    {
        public void Configure(EntityTypeBuilder<ClientPostLogoutRedirectUris> builder)
        {
            builder.ToTable("ClientPostLogoutRedirectUris");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("Id");
            builder.Property(x => x.ClientId).HasColumnName("ClientId");
            builder.Property(x => x.PostLogoutRedirectUri).HasColumnName("PostLogoutRedirectUri");
        }
    }
}
