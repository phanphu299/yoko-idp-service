using System.Linq;
using AHI.Infrastructure.Repository.Generic;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Domain.Entity;
using IdpServer.Persistence.Context;

namespace IdpServer.Persistence.Repository
{
    public class TimezoneRepository : GenericRepository<Timezone, int>, ITimezoneRepository
    {
        private readonly IdpDbContext _dbContext;
        public TimezoneRepository(IdpDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        protected override void Update(Timezone requestObject, Timezone targetObject)
        {

        }

        public override IQueryable<Domain.Entity.Timezone> AsQueryable()
        {
            return base.AsQueryable();
        }
    }
}
