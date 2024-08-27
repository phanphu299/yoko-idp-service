using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AHI.Infrastructure.Exception;
using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.SharedKernel.Abstraction;
using IdpServer.Application.Constant;
using IdpServer.Application.Model;
using IdpServer.Application.Repository.Abstraction;
using IdpServer.Application.Service.Abstraction;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace IdpServer.Application.Service
{
    public class EmailService : IEmailService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMasterService _masterService;
        private readonly ITenantContext _tenantContext;
        private readonly ILoggerAdapter<EmailService> _logger;
        public EmailService(
                            IServiceProvider serviceProvider,
                            IConfiguration configuration,
                            IEmailTemplateRepository emailTemplateRepository,
                            IMasterService masterService,
                            ITenantContext tenantContext,
                            IHttpClientFactory httpClientFactory,
                            ILoggerAdapter<EmailService> logger)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _emailTemplateRepository = emailTemplateRepository;
            _tenantContext = tenantContext;
            _masterService = masterService;
            _logger = logger;
        }

        public bool IsValid(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }
            string regex = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                            + "@"
                            + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
            Regex regexStandard = new Regex(regex);
            if (!regexStandard.IsMatch(input))
            {
                return false;
            }
            return true;
        }

        public async Task SendEmailAsync(string email, string typeCode, IDictionary<string, object> customField = null)
        {
            var template = await _emailTemplateRepository.GetEmailTemplateByTypeCodeAsync(typeCode);
            var request = new SendEmailRequestDto()
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
                Attachments = template?.Attachments.Select(at => new FileAttachment { Disposition = at.Disposition, FilePath = at.FilePath }) ?? Array.Empty<FileAttachment>()
            };
            var httpClient = _httpClientFactory.CreateClient(HttpClients.NOTIFICATION_SERVICE, _tenantContext);
            HttpContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, Constant.MimeType.JSON);
            try
            {
                var response = await httpClient.PostAsync("ntf/emails", content);
                response.EnsureSuccessStatusCode();
            }
            catch (System.Exception exc)
            {
                _logger.LogError($"Send email exception: {exc.ToString()}", exc.Message);
            }
        }
    }
}
