using IdpServer.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpServer.Persistence.Configuration
{
    public class PersistedGrantConfiguration : IEntityTypeConfiguration<PersistedGrant>
    {
        public void Configure(EntityTypeBuilder<PersistedGrant> builder)
        {
            // configure the model.
            builder.ToTable("PersistedGrants");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("Key");
            builder.Property(x => x.Type).HasColumnName("Type");
            builder.Property(x => x.SubjectId).HasColumnName("SubjectId");
            builder.Property(x => x.ClientId).HasColumnName("ClientId");
            builder.Property(x => x.CreationTime).HasColumnName("CreationTime");
            builder.Property(x => x.Expiration).HasColumnName("Expiration");
            builder.Property(x => x.Data).HasColumnName("Data");
        }
    }
}
