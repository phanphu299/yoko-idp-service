using System;
using System.Threading.Tasks;
using AHI.Infrastructure.Exception;
using IdentityServer4.Stores;
using IdpServer.Application.Constant;
using IdpServer.Application.SharedKernel;
using IdpServer.Application.User.Command;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Idp.Api.Controllers
{

    [Route("idp/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "oidc")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("id")]
        public async Task<IActionResult> GetUserByUpnAsync([FromBody] GetUserByUpn command)
        {
            var response = await _mediator.Send(command);
            if (response == null)
            {
                return NotFound(new BaseResponse(false, MessageConstants.ENTITY_NOT_FOUND, (int)ErrorCodeConstants.NOT_FOUND_STATUS_CODE));
            }
            return Ok(response.UserId);
        }

        [HttpPut("{id}/info")]
        public async Task<IActionResult> UpdateUserInfoAsync(Guid id, [FromBody] UpdateUserInfo command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserById(Guid id)
        {
            var command = new DeleteUserById(id);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete("{id}/remove")]
        public async Task<IActionResult> RemoveUserById(Guid id)
        {
            RemoveUserById command = new RemoveUserById(id);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> RestoreUserById(Guid id)
        {
            var command = new RestoreUser(id);
            try
            {
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUser command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeUserSessionAsync([FromBody] RevokeUserSessionByUpn command)
        {
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ForceResetPasswordAsync([FromBody] ForceResetPassword command, [FromQuery] string tenantId, [FromQuery] string subscriptionId)
        {
            try
            {
                command.TenantId = tenantId;
                command.SubscriptionId = subscriptionId;
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (EntityNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}/mfa")]
        public async Task<IActionResult> SetUserMfaAsync(Guid id, [FromBody] SetUserMFA command)
        {
            command.UserId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("defaultpage")]
        public async Task<IActionResult> UpdateUserDefaultPageAsync([FromBody] SetUserDefaultPage command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(Guid id, [FromQuery] bool ignoreQueryFilters = false)
        {
            var command = new GetUserById(id, ignoreQueryFilters);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("details")]
        public async Task<IActionResult> GetUserDetailsAsync([FromBody] GetUserDetails command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPatch("{id}/profile")]
        public async Task<IActionResult> PartialUpdateUserInfoAsync(Guid id, [FromBody] JsonPatchDocument patchDoc)
        {
            var response = await _mediator.Send(new PartialUpdateUser(id, patchDoc));
            return Ok(response);
        }

        [HttpGet("logintypes")]
        public async Task<IActionResult> GetLoginTypes()
        {
            var response = await _mediator.Send(new SearchLoginType());
            return Ok(response);
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchUsersAsync([FromBody] SearchUser command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("{id}/password")]
        public async Task<IActionResult> ChangePasswordAsync([FromRoute] Guid id, [FromBody] ChangePassword command)
        {
            command.UserId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}