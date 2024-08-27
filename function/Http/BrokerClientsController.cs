using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using AHI.Infrastructure.SharedKernel.Extension;
using Function.Model;
using Function.Service.Abstraction;

namespace Function.Http
{
    public class BrokerClientsController
    {
        private readonly IBrokerClientsService _brokerClientsService;

        public BrokerClientsController(IBrokerClientsService brokerClientsService)
        {
            _brokerClientsService = brokerClientsService;
        }

        [FunctionName("AuthenticateBrokerClient")]
        public async Task<IActionResult> AuthenticateBrokerClientAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fnc/idp/brokerclients/authenticate")] HttpRequestMessage req)
        {
            var payload = await req.Content.ReadAsByteArrayAsync();
            var message = payload.Deserialize<AuthenticateBrokerClientRequest>();

            var response = await _brokerClientsService.AuthenticateAsync(message);
            return new OkObjectResult(response);
        }
    }
}