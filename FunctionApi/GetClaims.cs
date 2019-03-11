using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net;

namespace FunctionApi
{
    public static class GetClaims
    {
        [FunctionName("GetClaims")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            //[Inject(typeof(HttpClient))]HttpClient httpClient,
            [Inject(typeof(IClaimService))]IClaimService claimService,
            ILogger log)
        {
            var response = await claimService.GetClaims();
            //var httpClient = httpClientFactory.CreateClient("CMS");
            //var response = await httpClient.GetAsync("http://localhost:8202/api/claims");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new BadRequestResult();
            }

            return new OkObjectResult(await response.Content.ReadAsStringAsync());
        }
    }
}
