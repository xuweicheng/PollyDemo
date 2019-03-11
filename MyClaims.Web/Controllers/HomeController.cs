using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyClaims.Web.Models;
using Polly;
using Polly.Registry;

namespace MyClaims.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IClaimsClient claimsClient;
        private readonly IPolicyRegistry<string> policyRegistry;

        public HomeController(IClaimsClient claimsClient,
            IPolicyRegistry<string> policyRegistry)
        {
            this.claimsClient = claimsClient;
            this.policyRegistry = policyRegistry;
        }
        public async Task<IActionResult> Index()
        {
            var claims = await claimsClient.GetAllAsync();
            return View(claims);
        }

        public IActionResult Claim()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Claim(MyClaim myClaim)
        {
            string referenceNum = "";

            var authPolicy = policyRegistry.Get<IAsyncPolicy<string>>("auth_policy");

            await authPolicy.ExecuteAsync(
                 async () =>
                    referenceNum = await claimsClient.PostAsync(myClaim)
                );

            if (string.IsNullOrEmpty(referenceNum))
                return View(myClaim);

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
