using IdpServer.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdpServer.Persistence.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // configure the model.
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("upn");
            builder.Property(x => x.FirstName).HasColumnName("first_name");
            builder.Property(x => x.LastName).HasColumnName("last_name");
            builder.Property(x => x.RequiredChangePassword).HasColumnName("required_change_password");
            //builder.Property(x => x.IsLocked).HasColumnName("is_locked");
            builder.Property(x => x.UserTypeCode).HasColumnName("user_type_code");
            builder.Property(x => x.UserId).HasColumnName("user_id");
            builder.Property(x => x.TenantId).HasColumnName("tenant_id");
            builder.Property(x => x.SubscriptionId).HasColumnName("subscription_id");
            builder.Property(x => x.Deleted).HasColumnName("deleted");
            builder.Property(x => x.Avatar).HasColumnName("avatar");
            builder.Property(x => x.DateTimeFormat).HasColumnName("date_time_format");
            builder.Property(x => x.DisplayDateTimeFormat).HasColumnName("display_date_time_format");
            builder.Property(x => x.TimezoneId).HasColumnName("timezone_id");
            builder.Property(x => x.LanguageCode).HasColumnName("language_code");
            builder.Property(x => x.CountryCode).HasColumnName("country_code");
            builder.Property(x => x.LoginTypeCode).HasColumnName("login_type_code");
            builder.Property(x => x.SetupMFA).HasColumnName("setup_mfa");
            builder.Property(x => x.DefaultPage).HasColumnName("default_page");
            builder.HasOne(x => x.UserType).WithMany(x => x.Users).HasForeignKey(x => x.UserTypeCode);
            builder.HasOne(x => x.LoginType).WithMany(x => x.Users).HasForeignKey(x => x.LoginTypeCode);
            builder.HasMany(x => x.PasswordHistories).WithOne(x => x.User).HasForeignKey(x => x.Upn);
            builder.HasQueryFilter(x => !x.Deleted);
        }
    }
}
