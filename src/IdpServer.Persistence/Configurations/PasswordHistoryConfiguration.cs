using IdpServer.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpServer.Persistence.Configuration
{
    public class PasswordHistoryConfiguration : IEntityTypeConfiguration<PasswordHistory>
    {
        public void Configure(EntityTypeBuilder<PasswordHistory> builder)
        {
            // configure the model.
            builder.ToTable("password_histories");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Upn).HasColumnName("upn");
            builder.Property(x => x.Password).HasColumnName("password");
            builder.Property(x => x.CreatedDate).HasColumnName("created_utc");
            builder.HasOne(x => x.User).WithMany(x => x.PasswordHistories);
        }
    }
}