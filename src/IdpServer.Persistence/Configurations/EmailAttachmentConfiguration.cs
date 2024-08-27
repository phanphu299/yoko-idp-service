using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpServer.Persistence.Configuration
{
    public class EmailAttachmentConfiguration : IEntityTypeConfiguration<Domain.Entity.EmailAttachment>
    {
        public void Configure(EntityTypeBuilder<Domain.Entity.EmailAttachment> builder)
        {
            // configure the model.
            builder.ToTable("email_attachments");
            builder.Property(x => x.EmailTemplateId).HasColumnName("email_template_id");
            builder.Property(x => x.FilePath).HasColumnName("file_path");
        }
    }
}
