using System.Threading.Tasks;
using IdpServer.Application.SAPCDC.Model;
namespace IdpServer.Application.Service.Abstraction
{
    public interface ISAPCDCService
    {
        Task<string> GetResetPasswordTokenAsync(string upn);
        Task<BaseSAPCDCResponseDto> ResetPasswordByTokenAsync(string passwordResetToken, string newPassword);
        Task<BaseSAPCDCResponseDto> ResetPasswordByUpnAsync(string upn, string newPassword);
        Task<RegTokenResponseDto> LoginAsync(string userName, string password);
        Task<BaseSAPCDCResponseDto> FinalizeRegistrationAsync(string regToken);
        Task<SearchResponseDto> SearchAsync(string upn = null, int pageSize = 20);
        Task<BaseSAPCDCResponseDto> UpdateAsync(UpdateAccountRequest request);
        Task<UserDetailsResponseDto> AddAsync(AddAccountRequest request);
    }
}