using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Globalization;

namespace IdpServer.Localization
{
    public class JsonStringLocalizer : IStringLocalizer
    {
        private readonly TranslationService _service;

        public JsonStringLocalizer(TranslationService service)
        {
            _service = service;
        }

        public LocalizedString this[string key]
        {
            get
            {
                string value = _service.Translate(key);
                return new LocalizedString(key, value ?? key, value == null);
            }
        }

        public LocalizedString this[string key, params object[] arguments]
        {
            get
            {
                var actualValue = this[key];
                return !actualValue.ResourceNotFound
                    ? new LocalizedString(key, string.Format(actualValue.Value, arguments), false)
                    : actualValue;
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _service.GetAllStrings();
        }

        public IStringLocalizer WithCulture(CultureInfo culture) => this;
    }
}
