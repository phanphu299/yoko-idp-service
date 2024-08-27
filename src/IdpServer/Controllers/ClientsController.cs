using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdpServer.Application.Client.Command;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Idp.Api.Controllers
{

    [Route("idp/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "oidc")]
    public class ClientsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ClientsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("info", Name = "GetClientInfo")]
        public async Task<IActionResult> GetClientInfoAsync(bool forceUpdate)
        {
            var clientId = User.Claims.First(x => x.Type == "client_id").Value;
            var allowHeaderClaim = User.Claims.FirstOrDefault(x => x.Type == "allowHeader");
            bool allowHeader = false;
            if (allowHeaderClaim != null)
            {
                bool.TryParse(allowHeaderClaim.Value, out allowHeader);
            }
            var command = new GetClientInfoById(Guid.Parse(clientId), forceUpdate, allowHeader);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("{id}/info")]
        public async Task<IActionResult> GetClientInfoAsync([FromRoute] string id, [FromQuery] bool forceUpdate = false)
        {
            var command = new GetClientInfoById(Guid.Parse(id), forceUpdate);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchAsync(SearchClient command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(AddClient command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] UpdateClient command)
        {
            command.ClientId = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPatch]
        public async Task<IActionResult> PatchAsync([FromBody] JsonPatchDocument patchDoc)
        {
            var response = await _mediator.Send(new PartialUpdateClient(patchDoc));
            return Ok(response);
        }
        
        [HttpDelete]
        public async Task<IActionResult> DeleteClientAsync([FromBody] DeleteClient command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClientAsync([FromRoute] Guid id)
        {
            var command = new DeleteClient
            {
                ClientIds = new List<Guid> {
                    id
                }
            };
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClientByIdAsync([FromRoute] Guid id)
        {
            var command = new GetClientById(id);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateClientSecretAsync([FromBody] GenerateClientSecret generateSecret)
        {
            var response = await _mediator.Send(generateSecret);
            return Ok(response);
        }

        [HttpHead("{id}")]
        public async Task<IActionResult> CheckExistClientAsync(string id)
        {
            var command = new CheckExistClient(new[] { id });
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("exist")]
        public async Task<IActionResult> CheckExistClientAsync([FromBody] CheckExistClient command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("{id}/fetch")]
        public async Task<IActionResult> FetchClientByIdAsync(string id)
        {
            var command = new FetchClientById(id);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("archive")]
        public async Task<IActionResult> ArchiveAsync([FromBody] ArchiveClient command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("retrieve")]
        public async Task<IActionResult> RetrieveAsync([FromBody] RetrieveClient command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("archive/verify")]
        public async Task<IActionResult> VerifyAsync([FromBody] VerifyClient command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
