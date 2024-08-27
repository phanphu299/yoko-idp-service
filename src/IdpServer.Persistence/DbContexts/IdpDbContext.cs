using AHI.Infrastructure.Service.Tag.Configuration;
using AHI.Infrastructure.Service.Tag.SqlServer.Configuration;
using IdpServer.Domain.Entity;
using IdpServer.Persistence.Configuration;
using IdpServer.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace IdpServer.Persistence.Context
{
    public class IdpDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<EmailAttachment> EmailAttachments { get; set; }
        public DbSet<ClientRole> ClientRoles { get; set; }
        public DbSet<ProjectClient> ProjectClients { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<PersistedGrant> PersistedGrants { get; set; }
        public DbSet<ClientSecret> ClientSecrets { get; set; }
        public DbSet<Timezone> Timezones { get; set; }
        public DbSet<LoginType> LoginTypes { get; set; }
        public DbSet<BrokerClient> BrokerClients { get; set; }
        public DbSet<ClientPostLogoutRedirectUris> ClientPostLogoutRedirectUris { get; set; }
        public DbSet<ClientRedirectUris> ClientRedirectUris { get; set; }
        public DbSet<PasswordHistory> PasswordHistories { get; set; }
        public DbSet<EntityTagDb> EntityTags { get; set; }
        public IdpDbContext(DbContextOptions<IdpDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new TimezoneConfiguration());
            modelBuilder.ApplyConfiguration(new UserTokenConfiguration());
            modelBuilder.ApplyConfiguration(new EmailTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new ClientRoleConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectClientConfiguration());
            modelBuilder.ApplyConfiguration(new ClientConfiguration());
            modelBuilder.ApplyConfiguration(new UserTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ClientSecretConfiguration());
            modelBuilder.ApplyConfiguration(new ClientGrantTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ClientScopeConfiguration());
            modelBuilder.ApplyConfiguration(new ClientClaimConfiguration());
            modelBuilder.ApplyConfiguration(new ClientRedirectUrisConfiguration());
            modelBuilder.ApplyConfiguration(new ClientPostLogoutRedirectUrisConfiguration());
            modelBuilder.ApplyConfiguration(new LoginTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BrokerClientConfiguration());
            modelBuilder.ApplyConfiguration(new EmailAttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new PasswordHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new PersistedGrantConfiguration());
            modelBuilder.ApplyConfiguration(new EntityTagConfiguration<EntityTagDb>());
        }
    }
}
