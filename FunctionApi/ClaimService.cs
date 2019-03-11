using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApi
{
    public class ClaimService : IClaimService
    {
        private readonly HttpClient httpClient;

        public ClaimService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> GetClaims()
        {
            //var httpClient = httpClientFactory.CreateClient("CMS");
            var response = await httpClient.GetAsync("http://localhost:8202/api/claims");
            return response;
        }
    }
}
