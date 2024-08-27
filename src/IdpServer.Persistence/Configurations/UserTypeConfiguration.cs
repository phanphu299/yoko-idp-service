using IdpServer.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpServer.Persistence.Configuration
{
    public class UserTypeConfiguration : IEntityTypeConfiguration<UserType>
    {
        public void Configure(EntityTypeBuilder<UserType> builder)
        {
            // configure the model.
            builder.ToTable("user_types");
            builder.HasQueryFilter(x => !x.Deleted);
            builder.Property(x => x.Id).HasColumnName("code");
            builder.Property(x => x.Name).HasColumnName("name");
            builder.Property(x => x.CreatedUtc).HasColumnName("created_utc");
            builder.Property(x => x.UpdatedUtc).HasColumnName("updated_utc");
            builder.Property(x => x.Deleted).HasColumnName("deleted");
            builder.HasMany(x => x.Users).WithOne(x => x.UserType).HasForeignKey(x => x.UserTypeCode);
        }
    }
}