using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AHI.Infrastructure.Cache.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using AHI.Infrastructure.SharedKernel.Model;
using IdpServer.Application.Constant;
using IdpServer.Application.Model;
using IdpServer.Application.Service.Abstraction;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace IdpServer.Application.Service
{
    public class LocalizationService : ILocalizationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICache _cache;
        private readonly IMemoryCache _memoryCache;
        private readonly string _podName;
        public LocalizationService(IHttpClientFactory httpClientFactory, ICache cache, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _memoryCache = memoryCache;
            _podName = configuration["PodName"] ?? "identity-service";
        }

        public async Task<IEnumerable<LanguageDto>> GetLanguagesAsync()
        {
            var key = $"{_podName}_idp_language_list";
            var cacheHit = await _cache.GetStringAsync(key);
            if (cacheHit == null)
            {
                // need to clear the cache
                _memoryCache.Set<IEnumerable<LanguageDto>>(key, null);
            }
            var languages = _memoryCache.Get<IEnumerable<LanguageDto>>(key);
            if (languages == null)
            {
                var httpClient = _httpClientFactory.CreateClient(HttpClients.LOCALIZATION_SERVICE);
                var response = await httpClient.PostAsync("/loc/languages/search", new StringContent(JsonConvert.SerializeObject(new
                {
                    PageSize = int.MaxValue
                }), Encoding.UTF8, mediaType: "application/json"));
                response.EnsureSuccessStatusCode();
                var stream = await response.Content.ReadAsByteArrayAsync();
                var responseData = stream.Deserialize<BaseSearchResponse<LanguageDto>>();
                languages = responseData.Data;
                _memoryCache.Set(key, languages);
                await _cache.StoreAsync(key, "1");
            }
            return languages;
        }
    }
}
