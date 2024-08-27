using AHI.Infrastructure.MultiTenancy.Abstraction;
using AHI.Infrastructure.MultiTenancy.Extension;
using AHI.Infrastructure.Security.Helper;
using AHI.Infrastructure.Service.Tag.Model;
using AHI.Infrastructure.Service.Tag.Service.Abstraction;
using AHI.Infrastructure.SharedKernel.Abstraction;
using AHI.Infrastructure.SharedKernel.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace Function.Http
{
    public class TagsController
    {
        private readonly ITenantContext _tenantContext;
        private readonly ITagService _tagService;
        private readonly ILoggerAdapter<TagsController> _logger;
        private readonly IConfiguration _configuration;

        public TagsController(ITenantContext tenantContext, ITagService tagService, ILoggerAdapter<TagsController> logger, IConfiguration configuration)
        {
            _tenantContext = tenantContext;
            _tagService = tagService;
            _logger = logger;
            _configuration = configuration;
        }

        [FunctionName("DeleteTags")]
        public async Task<IActionResult> DeleteTagsAsync(
               [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "fnc/idp/tags")] HttpRequestMessage req)
        {
            _logger.LogDebug($"DeleteTagsAsync Start delete tags");
            if (!await SecurityHelper.AuthenticateRequestAsync(req, _configuration))
            {
                _logger.LogDebug($"DeleteTagsAsync Unauthorize");
                return new UnauthorizedResult();
            }

            _tenantContext.RetrieveFromHeader(req.Headers);
            var content = await req.Content.ReadAsByteArrayAsync();
            var deleteTagMessage = content.Deserialize<DeleteTagMessage>();
            _logger.LogDebug($"DeleteTagsAsync Delete message deleteTagMessage: {deleteTagMessage.ToJson()}");
            await _tagService.DeleteTagsAsync(deleteTagMessage.TagIds);
            return new OkResult();
        }
    }
}