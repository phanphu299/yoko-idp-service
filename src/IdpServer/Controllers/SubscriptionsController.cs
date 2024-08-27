using System;
using System.Threading.Tasks;
using IdpServer.Application.User.Command;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idp.Api.Controllers
{

    [Route("idp/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "oidc")]
    public class SubscriptionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SubscriptionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpDelete("{id}/users")]
        public async Task<IActionResult> RemoveUserBySubscriptionId(Guid id)
        {
            var command = new RemoveUserBySubscriptionId(id);
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
