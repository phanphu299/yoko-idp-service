using System;
using System.Threading.Tasks;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Domain.Entity;
using IdpServer.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AHI.Infrastructure.Repository.Generic;

namespace IdpServer.Persistence.Repository
{
    public class ProjectClientRepository : GenericRepository<ProjectClient, Guid>, IProjectClientRepository
    {
        private readonly IdpDbContext _context;

        public ProjectClientRepository(IdpDbContext context) : base(context)
        {
            _context = context;
        }
        protected override void Update(
            ProjectClient requestObject,
            ProjectClient targetObject)
        {
        }   
        public async Task AddClientRoleAsync(ClientRole model)
        {
            if (model != null)
            {
                _context.ClientRoles.Add(model);
                await _context.SaveChangesAsync();
            }
        }
        
        public Task<ProjectClient> FindByClientIdAsync(int clientId)
        {
            return _context.ProjectClients.FirstOrDefaultAsync(x => x.ClientId == clientId);
        }
    }
}
