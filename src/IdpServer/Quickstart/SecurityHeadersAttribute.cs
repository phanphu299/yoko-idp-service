// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IdentityServer4.Quickstart.UI
{
    public class SecurityHeadersAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var result = context.Result;
            if (result is ViewResult)
            {
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Type-Options"))
                {
                    context.HttpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                }

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Frame-Options"))
                {
                    context.HttpContext.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                }

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
                var csp = new StringBuilder();
                csp.Append("default-src 'self'; object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';");
                // also consider adding upgrade-insecure-requests once you have HTTPS in place for production
                //csp += "upgrade-insecure-requests;";
                // also an example if you need client images to be displayed from twitter
                csp.Append("img-src 'self' https://cdn.ahi.apps.yokogawa.com https://cdn.ahi.apps.yokogawa.com.cn;");
                csp.Append("frame-src 'self' https://www.google.com;");
                csp.Append("script-src 'self' https://www.google.com https://www.gstatic.com https://cdn.ahi.apps.yokogawa.com https://cdn.ahi.apps.yokogawa.com.cn;");
                csp.Append("style-src 'self' https://fonts.googleapis.com https://cdn.ahi.apps.yokogawa.com https://cdn.ahi.apps.yokogawa.com.cn;");
                csp.Append("font-src 'self' https://fonts.gstatic.com https://cdn.ahi.apps.yokogawa.com https://cdn.ahi.apps.yokogawa.com.cn;");


                // once for standards compliant browsers
                if (!context.HttpContext.Response.Headers.ContainsKey("Content-Security-Policy"))
                {
                    context.HttpContext.Response.Headers.Add("Content-Security-Policy", csp.ToString());
                }
                // and once again for IE
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Security-Policy"))
                {
                    context.HttpContext.Response.Headers.Add("X-Content-Security-Policy", csp.ToString());
                }

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
                var referrer_policy = "no-referrer";
                if (!context.HttpContext.Response.Headers.ContainsKey("Referrer-Policy"))
                {
                    context.HttpContext.Response.Headers.Add("Referrer-Policy", referrer_policy);
                }
            }
        }
    }
}
