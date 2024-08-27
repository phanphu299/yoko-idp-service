using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Domain.Entity;
using IdpServer.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AHI.Infrastructure.Repository.Generic;

namespace IdpServer.Persistence.Repository
{
    public class ClientRoleRepository : GenericRepository<ClientRole, Guid>, IClientRoleRepository
    {
        private readonly IdpDbContext _context;

        public ClientRoleRepository(IdpDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ClientRole>> GetByClientIdAsync(Guid clientId)
        {
            return await _context.ClientRoles.Where(x => x.ClientId == clientId).AsNoTracking().ToListAsync();
        }

        protected override void Update(ClientRole requestObject, ClientRole targetObject)
        {
            targetObject.RoleId = requestObject.RoleId;
        }
    }
}
