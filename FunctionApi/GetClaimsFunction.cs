using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using FunctionApi.IntegrationClient;

namespace FunctionApi
{
    public static class GetClaimsFunction
    {
        [FunctionName("GetClaims")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Inject]IClaimsClient claimsClient,
            ILogger log)
        {
            var claims = await claimsClient.GetAllAsync();

            return new OkObjectResult(claims);
        }
    }
}
