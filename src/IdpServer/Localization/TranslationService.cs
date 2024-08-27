using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace IdpServer.Localization
{
    public class TranslationService
    {
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, string> _translation = new Dictionary<string, string>();
        private readonly JsonSerializer _serializer = new JsonSerializer();
        private readonly HttpClient _httpClient = new HttpClient();

        public TranslationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Translate(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var cacheKey = $"language_{CultureInfo.CurrentCulture.Name}_{key}";
            try
            {
                var cacheValue = _translation[cacheKey];
                return cacheValue;
            }
            catch (KeyNotFoundException)
            {
                UpdateTranslation();
                var cacheValue = "";
                _translation.TryGetValue(cacheKey, out cacheValue);
                return string.IsNullOrEmpty(cacheValue) ? key : cacheValue;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings()
        {
            if (string.IsNullOrWhiteSpace(_configuration["LanguageUrl"]))
                return new List<LocalizedString>();

            return FetchTranslation();
        }

        public void ResetTranslation()
        {
            _translation.Clear();
        }

        private void UpdateTranslation()
        {
            if (string.IsNullOrWhiteSpace(_configuration["LanguageUrl"]))
                return;

            var languageCode = CultureInfo.CurrentCulture.Name;
            foreach (var item in FetchTranslation())
            {
                var key = $"language_{languageCode}_{item.Name}";
                if (_translation.ContainsKey(key))
                {
                    _translation[key] = item.Value;
                }
                else
                {
                    _translation.Add(key, item.Value);
                }
            }
        }

        private IEnumerable<LocalizedString> FetchTranslation()
        {
            var languageUrl = _configuration["LanguageUrl"];
            var languageCode = CultureInfo.CurrentCulture.Name;
            StreamReader sr = null;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{languageUrl.TrimEnd('/')}/{languageCode}.json");
                var response = _httpClient.Send(request);
                response.EnsureSuccessStatusCode();
                sr = new StreamReader(response.Content.ReadAsStream());
            }
            catch
            {
                var path = Path.Combine(Environment.CurrentDirectory, "Localization/Languages", $"{languageCode}.json");
                sr = new StreamReader(path);
            }

            using (var reader = new JsonTextReader(sr))
            {
                while (reader.Read())
                {
                    if (reader.TokenType != JsonToken.PropertyName)
                        continue;
                    string key = (string)reader.Value;
                    reader.Read();
                    string value = _serializer.Deserialize<string>(reader);
                    yield return new LocalizedString(key, value ?? key, string.IsNullOrEmpty(value));
                }
            }
        }
    }
}
