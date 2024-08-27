using IdpServer.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Quickstart.UI
{
    [Route("idp/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "oidc")]
    public class LocalizationController : ControllerBase
    {
        private readonly TranslationService _service;
        public LocalizationController(TranslationService service)
        {
            _service = service;
        }

        [HttpGet("reset")]
        public IActionResult ResetTranslation()
        {
            _service.ResetTranslation();
            return Ok();
        }
    }
}
