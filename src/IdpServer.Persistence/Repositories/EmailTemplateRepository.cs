using System;
using System.Linq;
using System.Threading.Tasks;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Domain.Entity;
using IdpServer.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AHI.Infrastructure.Repository.Generic;

namespace IdpServer.Persistence.Repository
{
    public class EmailTemplateRepository : GenericRepository<EmailTemplate, Guid>, IEmailTemplateRepository
    {
        private readonly IdpDbContext _context;
        public EmailTemplateRepository(IdpDbContext context)
            : base(context)
        {
            _context = context;
        }

        protected override void Update(EmailTemplate requestObject, EmailTemplate targetObject)
        {
            targetObject.Name = requestObject.Name;
            targetObject.Subject = requestObject.Subject;
            targetObject.Html = requestObject.Html;
            targetObject.UpdatedUtc = DateTime.UtcNow;
        }

        public Task<EmailTemplate> GetEmailTemplateByTypeCodeAsync(string typeCode)
        {
            return _context.EmailTemplates
                            .Include(et => et.Attachments)
                            .Where(x => x.TypeCode == typeCode)
                            .AsNoTracking()
                            .FirstOrDefaultAsync();
        }

        public async Task<EmailTemplate> DeleteEmailTemplateAsync(Guid id)
        {
            var entity = await FindAsync(id);
            if (entity != null)
            {
                entity.Deleted = true;
                entity.UpdatedUtc = DateTime.UtcNow;
                await Context.SaveChangesAsync();
                return entity;
            }
            return null;
        }
    }
}
