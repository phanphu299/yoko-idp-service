using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IdpServer.Application.Model;
using Microsoft.AspNetCore.Identity;

namespace IdpServer.Customized.Dummy
{
    public class DummyUserStore<TUser> :
           IUserStore<TUser>,
           IUserAuthenticatorKeyStore<TUser>
           where TUser : UserDto
    {
        public Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
        {
            var token = user.UserId.ToString("N");
            var array = Encoding.UTF8.GetBytes(token);
            return Task.FromResult(Base32.Encode(array));
        }
        public Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {

        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }


        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Upn);
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Upn);
        }



        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}