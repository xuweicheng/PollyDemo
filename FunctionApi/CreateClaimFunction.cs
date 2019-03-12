using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using Polly.Registry;
using Polly;
using FunctionApi.IntegrationClient;

namespace FunctionApi
{
    public static class CreateClaimFunction
    {
        [FunctionName("CreateClaim")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] MyClaim myClaim,
            [Inject]IClaimsClient claimsClient,
            [Inject]IPolicyRegistry<string> policyRegistry,
            ILogger log)
        {
            string referenceNum = default(string);

            var authPolicy = policyRegistry.Get<IAsyncPolicy<string>>("auth_policy");

            await authPolicy.ExecuteAsync(
                 async () =>
                    referenceNum = await claimsClient.PostAsync(myClaim)
                );

            return new OkObjectResult(referenceNum);
        }
    }
}
