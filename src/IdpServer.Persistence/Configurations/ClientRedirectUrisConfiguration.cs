using IdpServer.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpServer.Persistence.Configurations
{
    public class ClientRedirectUrisConfiguration : IEntityTypeConfiguration<ClientRedirectUris>
    {
        public void Configure(EntityTypeBuilder<ClientRedirectUris> builder)
        {
            builder.ToTable("ClientRedirectUris");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("Id");
            builder.Property(x => x.ClientId).HasColumnName("ClientId");
            builder.Property(x => x.RedirectUri).HasColumnName("RedirectUri");
        }
    }
}
