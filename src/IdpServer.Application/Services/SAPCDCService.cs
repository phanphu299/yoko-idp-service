using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AHI.Infrastructure.Exception;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using IdpServer.Application.Constant;
using IdpServer.Application.SAPCDC.Model;
using IdpServer.Application.Service.Abstraction;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
namespace IdpServer.Application.Service
{
    public class SAPCDCService : ISAPCDCService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILoggerAdapter<SAPCDCService> _logger;
        public SAPCDCService(
                            IHttpClientFactory httpClientFactory,
                            IConfiguration configuration,
                            ILoggerAdapter<SAPCDCService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<RegTokenResponseDto> LoginAsync(string userName, string password)
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClients.SAP_CDC);
            var requestBody = new Dictionary<string, string>();
            requestBody.Add("loginID", userName);
            requestBody.Add("password", password);
            var response = await httpClient.PostAsync(SAPCDCConstants.ENDPOINT_LOGIN, new FormUrlEncodedContent(requestBody));
            var content = await response.Content.ReadAsByteArrayAsync();
            var result = content.Deserialize<RegTokenResponseDto>();
            if (result == null)
            {
                throw new SystemCallServiceException(ExceptionErrorCode.ERROR_SYSTEM_CALL_SERVICE);
            }
            return result;
        }

        public async Task<BaseSAPCDCResponseDto> FinalizeRegistrationAsync(string regToken)
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClients.SAP_CDC);
            var requestBody = new Dictionary<string, string>();
            requestBody.Add("regToken", regToken);
            var response = await httpClient.PostAsync(SAPCDCConstants.ENDPOINT_FINALIZE, new FormUrlEncodedContent(requestBody));
            var content = await response.Content.ReadAsByteArrayAsync();
            var result = content.Deserialize<BaseSAPCDCResponseDto>();
            if (result == null)
            {
                throw new SystemCallServiceException(ExceptionErrorCode.ERROR_SYSTEM_CALL_SERVICE);
            }
            return result;
        }

        public async Task<string> GetResetPasswordTokenAsync(string userName)
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClients.SAP_CDC);
            var requestBody = new Dictionary<string, string>();
            requestBody.Add("loginID", userName);
            requestBody.Add("sendEmail", "false");
            var response = await httpClient.PostAsync(SAPCDCConstants.ENDPOINT_RESET_PASSWORD, new FormUrlEncodedContent(requestBody));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsByteArrayAsync();
            var body = content.Deserialize<PasswordResetTokenResponseDto>();
            if (body == null)
            {
                throw new SystemCallServiceException(ExceptionErrorCode.ERROR_SYSTEM_CALL_SERVICE);
            }
            return body.PasswordResetToken;
        }

        public async Task<BaseSAPCDCResponseDto> ResetPasswordByTokenAsync(string passwordResetToken, string newPassword)
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClients.SAP_CDC);
            var requestBody = new Dictionary<string, string>();
            requestBody.Add("passwordResetToken", passwordResetToken);
            requestBody.Add("newPassword", newPassword);
            var response = await httpClient.PostAsync(SAPCDCConstants.ENDPOINT_RESET_PASSWORD, new FormUrlEncodedContent(requestBody));
            var content = await response.Content.ReadAsByteArrayAsync();
            var result = content.Deserialize<BaseSAPCDCResponseDto>();
            if (result == null)
            {
                throw new SystemCallServiceException(ExceptionErrorCode.ERROR_SYSTEM_CALL_SERVICE);
            }
            return result;
        }

        public async Task<BaseSAPCDCResponseDto> ResetPasswordByUpnAsync(string upn, string newPassword)
        {
            var passwordResetToken = await GetResetPasswordTokenAsync(upn);
            var httpClient = _httpClientFactory.CreateClient(HttpClients.SAP_CDC);
            var requestBody = new Dictionary<string, string>
            {
                { "passwordResetToken", passwordResetToken },
                { "newPassword", newPassword }
            };
            var response = await httpClient.PostAsync(SAPCDCConstants.ENDPOINT_RESET_PASSWORD, new FormUrlEncodedContent(requestBody));
            var content = await response.Content.ReadAsByteArrayAsync();
            var result = content.Deserialize<BaseSAPCDCResponseDto>();
            if (result == null)
            {
                throw new SystemCallServiceException(ExceptionErrorCode.ERROR_SYSTEM_CALL_SERVICE);
            }
            return result;
        }

        public async Task<UserDetailsResponseDto> AddAsync(AddAccountRequest request)
        {
            var environment = _configuration["Environment"];
            request.Data.Environment = environment;
            var httpClient = _httpClientFactory.CreateClient(HttpClients.SAP_CDC);
            var requestBody = new Dictionary<string, string>();
            requestBody.Add("email", request.Email.ToLowerInvariant());
            requestBody.Add("password", request.Password);
            requestBody.Add("data", JsonConvert.SerializeObject(request.Data).Replace('"', '\''));
            requestBody.Add("profile", JsonConvert.SerializeObject(request.Profile).Replace('"', '\''));
            requestBody.Add("finalizeRegistration", "false");
            var response = await httpClient.PostAsync(SAPCDCConstants.ENDPOINT_REGISTER, new FormUrlEncodedContent(requestBody));
            var content = await response.Content.ReadAsByteArrayAsync();
            var result = content.Deserialize<UserDetailsResponseDto>();
            if (result == null)
            {
                throw new SystemCallServiceException(ExceptionErrorCode.ERROR_SYSTEM_CALL_SERVICE);
            }
            return result;
        }

        public async Task<SearchResponseDto> SearchAsync(string upn = null, int pageSize = 20)
        {
            var httpClient = _httpClientFactory.CreateClient(HttpClients.SAP_CDC);
            var requestBody = new Dictionary<string, string>();
            var environment = _configuration["Environment"];
            var query = $"select UID, profile, data, isVerified, isRegistered, identities.provider, identities.providerUID from accounts where profile.email='{upn.ToLowerInvariant()}' and data.environment='{environment}' limit {pageSize}";
            requestBody.Add("query", query);
            var response = await httpClient.PostAsync(SAPCDCConstants.ENDPOINT_SEARCH, new FormUrlEncodedContent(requestBody));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsByteArrayAsync();
            var result = content.Deserialize<SearchResponseDto>();
            if (result == null)
            {
                throw new SystemCallServiceException(ExceptionErrorCode.ERROR_SYSTEM_CALL_SERVICE);
            }
            return result;
        }

        public async Task<BaseSAPCDCResponseDto> UpdateAsync(UpdateAccountRequest request)
        {
            var environment = _configuration["Environment"];
            request.Data.Environment = environment;
            var httpClient = _httpClientFactory.CreateClient(HttpClients.SAP_CDC);
            var requestBody = new Dictionary<string, string>();
            requestBody.Add("UID", request.UId);
            requestBody.Add("data", JsonConvert.SerializeObject(request.Data).Replace('"', '\''));
            requestBody.Add("profile", JsonConvert.SerializeObject(request.Profile).Replace('"', '\''));
            var response = await httpClient.PostAsync(SAPCDCConstants.ENDPOINT_UPDATE, new FormUrlEncodedContent(requestBody));
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsByteArrayAsync();
            var result = content.Deserialize<BaseSAPCDCResponseDto>();
            if (result == null)
            {
                throw new SystemCallServiceException(ExceptionErrorCode.ERROR_SYSTEM_CALL_SERVICE);
            }
            return result;
        }
    }
}