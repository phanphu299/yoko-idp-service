using IdpServer.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpServer.Persistence.Configuration
{
    public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
    {
        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            // configure the model.
            builder.ToTable("user_tokens");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserName).HasColumnName("user_name");
            builder.Property(x => x.TokenKey).HasColumnName("token_key");
            builder.Property(x => x.TokenType).HasColumnName("token_type");
            builder.Property(x => x.CreatedDate).HasColumnName("created_date");
            builder.Property(x => x.ExpiredDate).HasColumnName("expired_date");
            builder.Property(x => x.RedirectUrl).HasColumnName("redirect_url");
            builder.Property(x => x.ClickCount).HasColumnName("click_count");
            builder.Property(x => x.MaxClickCount).HasColumnName("max_click_count");
            builder.HasQueryFilter(x => !x.Deleted);
        }
    }
}