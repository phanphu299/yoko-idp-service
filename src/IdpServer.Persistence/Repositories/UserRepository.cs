using System;
using System.Linq;
using System.Threading.Tasks;
using AHI.Infrastructure.Repository.Generic;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Domain.Entity;
using IdpServer.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace IdpServer.Persistence.Repository
{
    public class UserRepository : GenericRepository<User, string>, IUserRepository
    {
        private readonly IdpDbContext _dbContext;
        public UserRepository(IdpDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        protected override void Update(User requestObject, User targetObject)
        {
            targetObject.FirstName = requestObject.FirstName;
            targetObject.LastName = requestObject.LastName;
            targetObject.Password = requestObject.Password;
            targetObject.RequiredChangePassword = requestObject.RequiredChangePassword;
            //targetObject.IsLocked = requestObject.IsLocked;
            targetObject.UserTypeCode = requestObject.UserTypeCode;
            targetObject.MFA = requestObject.MFA;
            targetObject.TenantId = requestObject.TenantId;
            targetObject.SubscriptionId = requestObject.SubscriptionId;
            targetObject.LoginTypeCode = requestObject.LoginTypeCode;
            targetObject.DefaultPage = requestObject.DefaultPage;
        }

        public override IQueryable<User> AsQueryable()
        {
            return base.AsQueryable().Include(x => x.UserType);
        }

        public Task<User> FindUserByUserIdAsync(Guid userId, bool ignoreQueryFilters = false)
        {
            if (!ignoreQueryFilters)
                return AsQueryable().AsNoTracking().Where(x => x.UserId == userId).FirstOrDefaultAsync();
            else
                return AsQueryable().IgnoreQueryFilters().AsNoTracking().Where(x => x.UserId == userId).FirstOrDefaultAsync();
        }

        // override with ignore query filter to remove soft deleted entity
        public override async Task<bool> RemoveAsync(string key)
        {
            var entity = await AsQueryable().IgnoreQueryFilters().FirstOrDefaultAsync(e => e.Id.Equals(key));
            if (entity != null)
            {
                _dbContext.Users.Remove(entity);
                return true;
            }
            return false;
        }
    }
}
