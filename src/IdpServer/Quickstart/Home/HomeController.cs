// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using IdentityServer4.Quickstart.Model;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using AHI.Infrastructure.SharedKernel.Abstraction;
using Microsoft.AspNetCore.Http.Extensions;

namespace IdentityServer4.Quickstart.UI
{
    [SecurityHeaders]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IWebHostEnvironment _environment;
        private readonly IClientStore _clientStore;
        private readonly IConfiguration _configuration;
        private readonly ILoggerAdapter<HomeController> _logger;

        public HomeController(IIdentityServerInteractionService interaction, IWebHostEnvironment environment, ILoggerAdapter<HomeController> logger, IClientStore clientStore, IConfiguration configuration)
        {
            _interaction = interaction;
            _environment = environment;
            _logger = logger;
            _clientStore = clientStore;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            _logger.LogInformation($"Home/Index {HttpContext.Request.GetDisplayUrl()}");
            return Redirect(_configuration["DefaultEmail:RedirectUrl"] ?? "~/");
        }

        /// <summary>
        /// Shows the error page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;

                if (!_environment.IsDevelopment())
                {
                    message.ErrorDescription = null;
                }
                _logger.LogDebug($"Error handling request {message.RequestId} : {JsonConvert.SerializeObject(message)}");
            }
            vm.ReturnUrl = message.RedirectUri;
            return View("Error", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Error(ErrorViewModel model, string button = "Ok")
        {
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            if (context != null)
            {
                if (await _clientStore.IsPkceClientAsync(context.ClientId))
                {
                    return this.LoadingPage("Redirect", model.ReturnUrl);
                }
                return Redirect(model.ReturnUrl);
            }
            if (Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            else
            {
                return Redirect(_configuration["DefaultEmail:RedirectUrl"] ?? "~/");
            }
        }
    }
}