using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpServer.Persistence.Configuration
{
    public class TimezoneConfiguration : IEntityTypeConfiguration<Domain.Entity.Timezone>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.Timezone> builder)
        {
            // configure the model.
            builder.ToTable("timezones");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasColumnName("name");
        }
    }
}