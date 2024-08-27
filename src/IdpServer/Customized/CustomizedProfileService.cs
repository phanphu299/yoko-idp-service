using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
namespace IdpServer.Customized
{
    public class CustomizedProfileService : IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.IssuedClaims.AddRange(context.Subject.Claims);
            context.IssuedClaims.AddRange(context.Client.Claims);
            return Task.CompletedTask;
        }
        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}