using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Function.Http.Health
{
    public class HealthCheckController
    {
        [FunctionName("HealthProbeCheck")]
        public IActionResult LivenessProbeCheck(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "fnc/healthz")] HttpRequestMessage req)
        {
            return new OkResult();
        }
    }
}