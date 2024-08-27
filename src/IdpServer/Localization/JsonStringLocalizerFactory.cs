using Microsoft.Extensions.Localization;
using System;

namespace IdpServer.Localization
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly TranslationService _service;

        public JsonStringLocalizerFactory(TranslationService service)
        {
            _service = service;
        }

        public IStringLocalizer Create(Type resourceSource) => new JsonStringLocalizer(_service);

        public IStringLocalizer Create(string baseName, string location) => new JsonStringLocalizer(_service);
    }
}
