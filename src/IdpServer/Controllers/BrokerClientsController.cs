using System.Collections.Generic;
using System.Threading.Tasks;
using IdpServer.Application.BrokerClient.Command;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Idp.Api.Controllers
{

    [Route("idp/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "oidc")]
    public class BrokerClientsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public BrokerClientsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchAsync(SearchBrokerClient command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(AddBrokerClient command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromBody] UpdateBrokerClient command)
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBrokerClientAsync([FromBody] DeleteBrokerClient command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrokerClientAsync([FromRoute] string id)
        {
            var command = new DeleteBrokerClient
            {
                Ids = new List<string> {
                    id
                }
            };
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrokerClientByClientIdAsync([FromRoute] string id)
        {
            var command = new GetBrokerClientById(id);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("{id}/fetch")]
        public async Task<IActionResult> FetchByIdAsync(string id)
        {
            var command = new FetchBrokerClientByClientId(id);
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
