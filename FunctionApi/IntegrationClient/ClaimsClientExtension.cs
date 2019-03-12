using FunctionApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FunctionApi.IntegrationClient
{
    public partial class ClaimsClient : IClaimsClient
    {
        private readonly ITokenService tokenService;

        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
            // get access token from cache
            var token = tokenService.GetToken();
            request.Headers.Add("Authorization", "Bearer " + token);
        }
    }
}
