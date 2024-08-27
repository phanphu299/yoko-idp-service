using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace IdpServer.Localization
{
    public class LocalizationMiddleware : IMiddleware
    {
        const string defaultCulture = "en-US";
        private readonly IOptions<RequestLocalizationOptions> _requestLocalizationOptions;
        public LocalizationMiddleware(IOptions<RequestLocalizationOptions> requestLocalizationOptions)
        {
            _requestLocalizationOptions = requestLocalizationOptions;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var userLanguage = context.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
            if (string.IsNullOrWhiteSpace(userLanguage))
            {
                var userBrowserLanguage = getUserBrowserLanguage(context.Request.GetTypedHeaders().AcceptLanguage);
                userLanguage = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(userBrowserLanguage));
                context.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, userLanguage, new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(7) });
            }

            var cultureResult = CookieRequestCultureProvider.ParseCookieValue(userLanguage);
            var culture = new CultureInfo(cultureResult.UICultures.FirstOrDefault().Value);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            await next(context);
        }

        private string getUserBrowserLanguage(IList<StringWithQualityHeaderValue> acceptLanguages)
        {
            var userBrowserLanguage = string.Empty;
            try
            {
                var browserLanguage = acceptLanguages.OrderByDescending(x => x.Quality ?? 1).FirstOrDefault().Value.ToString();
                var browserCulture = _requestLocalizationOptions.Value.SupportedUICultures.FirstOrDefault(cult => cult.TwoLetterISOLanguageName == browserLanguage || cult.Name == browserLanguage);
                userBrowserLanguage = browserCulture.Name;
            }
            catch (Exception)
            {
                userBrowserLanguage = defaultCulture;
            }
            return userBrowserLanguage;
        }
    }
}
