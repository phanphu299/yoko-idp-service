using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpServer.Persistence.Configuration
{
    public class EmailTemplateConfiguration : IEntityTypeConfiguration<Domain.Entity.EmailTemplate>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.EmailTemplate> builder)
        {
            // configure the model.
            builder.ToTable("email_templates");
            builder.HasQueryFilter(x => !x.Deleted);
            builder.Property(x => x.TypeCode).HasColumnName("type_code");
            builder.Property(x => x.UpdatedUtc).HasColumnName("updated_utc");
            builder.Property(x => x.CreatedUtc).HasColumnName("created_utc");
            builder.HasMany(x => x.Attachments).WithOne().HasForeignKey(x => x.EmailTemplateId);
        }
    }
}