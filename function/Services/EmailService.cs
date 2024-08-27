using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.SharedKernel.Abstraction;
using Dapper;
using Function.Constant;
using Function.Model;
using Function.Service.Abstraction;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Function.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ITenantContext _tenantContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILoggerAdapter<EmailService> _logger;

        public EmailService(IConfiguration configuration,
                            ITenantContext tenantContext,
                            ILoggerAdapter<EmailService> logger,
                            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _tenantContext = tenantContext;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string typeCode, IDictionary<string, object> customField = null)
        {
            string uri = Constant.Endpoints.SendEmail;
            var template = await GetEmailTemplateByTypeCodeAsync(typeCode);

            var request = new SendEmailRequest()
            {
                TypeCode = typeCode,
                Subject = template?.Subject ?? "",
                HtmlBody = template?.Html ?? "",
                Emails = new List<Email>(){
                        new Email()
                        {
                            To = email,
                            Payload = customField
                        }
                    },
                Attachments = template?.Attachments.Select(at => new FileAttachment { Disposition = at.Disposition, FilePath = at.FilePath }) ?? System.Array.Empty<FileAttachment>()
            };
            var httpClient = _httpClientFactory.CreateClient(HttpClients.NOTIFICATION_SERVICE, _tenantContext);
            HttpContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, Constant.MimeType.JSON);
            try
            {
                var response = await httpClient.PostAsync(uri, content);
                response.EnsureSuccessStatusCode();
            }
            catch (System.Exception exc)
            {
                _logger.LogError($"Send email exception: {exc.ToString()}", exc.Message);
            }
        }

        private async Task<EmailTemplateDto> GetEmailTemplateByTypeCodeAsync(string typeCode)
        {
            var connectionString = _configuration["ConnectionStrings:Default"];
            var emailTemplate = (EmailTemplateDto)null;
            using (var connection = new SqlConnection(connectionString))
            {
                Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
                emailTemplate = await connection.QueryFirstOrDefaultAsync<EmailTemplateDto>(@"SELECT * FROM email_templates WITH(NOLOCK) WHERE type_code = @TypeCode", new { TypeCode = typeCode });
                emailTemplate.Attachments = await connection.QueryAsync<EmailAttachmentDto>(@"SELECT * FROM email_attachments where email_template_id = @TemplateId", new { TemplateId = emailTemplate.Id });
                await connection.CloseAsync();
            }
            return emailTemplate;
        }
    }
}
