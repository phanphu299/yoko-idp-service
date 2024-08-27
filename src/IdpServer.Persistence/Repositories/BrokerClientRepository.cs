using System;
using System.Linq;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Domain.Entity;
using IdpServer.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using AHI.Infrastructure.Repository.Generic;

namespace IdpServer.Persistence.Repository
{
    public class BrokerClientRepository : GenericRepository<BrokerClient, string>, IBrokerClientRepository
    {
        private readonly IdpDbContext _context;
        private readonly ITenantContext _tenantContext;

        public BrokerClientRepository(
            IdpDbContext context
            , ITenantContext tenantContext) : base(context)
        {
            _context = context;
            _tenantContext = tenantContext;
        }


        public override IQueryable<BrokerClient> AsFetchable()
        {
            return _context.BrokerClients.AsNoTracking().Select(x => new BrokerClient { Id = x.Id, CreatedBy = x.CreatedBy, ExpiredUtc = x.ExpiredUtc });
        }

        protected override void Update(BrokerClient requestObject, BrokerClient targetObject)
        {
            targetObject.Password = requestObject.Password;
            targetObject.ExpiredUtc = requestObject.ExpiredUtc;
            targetObject.UpdatedUtc = DateTime.UtcNow;
        }
    }
}
