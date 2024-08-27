using System;
using System.Threading.Tasks;
using AHI.Infrastructure.Repository.Generic;

namespace IdpServer.Application.Repository.Abstraction
{
    public interface IEmailTemplateRepository : IRepository<Domain.Entity.EmailTemplate, Guid>
    {
        Task<Domain.Entity.EmailTemplate> GetEmailTemplateByTypeCodeAsync(string typeCode);
        Task<Domain.Entity.EmailTemplate> DeleteEmailTemplateAsync(Guid id);
    }
}