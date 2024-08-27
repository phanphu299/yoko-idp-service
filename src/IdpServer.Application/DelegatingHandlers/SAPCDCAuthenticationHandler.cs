using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using IdpServer.Application.Constant;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IdpServer.Application.Delegating.Handler
{
    public class SAPCDCAuthenticationHandler : DelegatingHandler
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SAPCDCAuthenticationHandler> _logger;

        public SAPCDCAuthenticationHandler(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, IConfiguration configuration, ILogger<SAPCDCAuthenticationHandler> logger)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var apiKey = _configuration["SAP:ApiKey"];
            var userKey = _configuration["SAP:UserKey"];
            var secret = _configuration["SAP:Secret"];

            var contentString = await request.Content.ReadAsStringAsync();
            var content = contentString.Split('&').Select(part => part.Split('=')).Where(part => part.Length == 2).ToDictionary(sp => HttpUtility.UrlDecode(sp[0]), sp => HttpUtility.UrlDecode(sp[1]));

            if (!content.Any(x => x.Key == SAPCDCConstants.CONTENT_API_KEY))
                content.Add(SAPCDCConstants.CONTENT_API_KEY, apiKey);

            if (!content.Any(x => x.Key == SAPCDCConstants.CONTENT_USER_KEY))
                content.Add(SAPCDCConstants.CONTENT_USER_KEY, userKey);

            if (!content.Any(x => x.Key == SAPCDCConstants.CONTENT_SECRET))
                content.Add(SAPCDCConstants.CONTENT_SECRET, secret);

            if (!content.Any(x => x.Key == SAPCDCConstants.CONTENT_HTTP_STATUS_CODE))
                content.Add(SAPCDCConstants.CONTENT_HTTP_STATUS_CODE, "true");

            if (!content.Any(x => x.Key == SAPCDCConstants.CONTENT_FORMAT))
                content.Add(SAPCDCConstants.CONTENT_FORMAT, "json");

            request.Content = new FormUrlEncodedContent(content);

            var result = await base.SendAsync(request, cancellationToken);

            return result;
        }
    }
}