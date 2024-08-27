using System.Threading.Tasks;
using IdpServer.Application.Service.Abstraction;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Application.Model;
using IdpServer.Application.Enum;
using System.Linq;

namespace IdpServer.Application.Service
{
    public class TokenService : ITokenService
    {
        private readonly IUserTokenRepository _userTokenRepository;
        public TokenService(IUserTokenRepository userTokenRepository)
        {
            _userTokenRepository = userTokenRepository;
        }

        public async Task<UserTokenDto> GenerateTokenAsync(string userName, TokenTypeEnum tokenType, string redirectUrl = "", int tokenLenght = 32, double expiredInSeconds = 86400)
        {
            var result = await _userTokenRepository.GenerateTokenAsync(userName, tokenType.ToString(), redirectUrl, tokenLenght, expiredInSeconds);
            return UserTokenDto.Create(result);
        }

        public Task DeleteTokenByTypeAsync(string userName, TokenTypeEnum tokenType)
        {
            return _userTokenRepository.DeleteTokenByTypeAsync(userName, tokenType.ToString());
        }

        public async Task<UserTokenDto> GetLatestTokenByTypeAsync(string userName, TokenTypeEnum tokenType)
        {
            var result = await _userTokenRepository.GetLatestTokenByTypeAsync(userName, tokenType.ToString());
            return UserTokenDto.Create(result);
        }

        public async Task<UserTokenDto> GetUserTokenAsync(string userName, string tokenKey, TokenTypeEnum tokenType)
        {
            var allowTokenTypes = new TokenTypeEnum[]{
                TokenTypeEnum.ResetPassword,
                TokenTypeEnum.SetPassword,
                TokenTypeEnum.ChangePassword
            };
            var result = await _userTokenRepository.GetUserTokenAsync(userName, tokenKey, tokenType.ToString());
            if (!allowTokenTypes.Contains(tokenType))
            {
                // delete other token type than set password.
                // in some cases, the email provider implement threat security scanning
                // we need to allow set password token to be get twice, but not greater than max_click_count (2)
                await _userTokenRepository.DeleteTokenAsync(userName, tokenKey);
            }
            return UserTokenDto.Create(result);
        }
    }
}
