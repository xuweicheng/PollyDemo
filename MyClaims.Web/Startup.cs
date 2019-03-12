using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace MyClaims.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //services.AddSingleton<ITokenService, TokenService>();

            var provider = services.BuildServiceProvider();

            //Moved to FunctionApi
            //var authPolicy = Policy.HandleResult<string>(result => result == "401")
            //    .RetryAsync(
            //    retryCount: 1,
            //    onRetry: (response, retryNumber, context) =>
            //    {
            //        var tokenService = provider.GetService<ITokenService>();
            //        tokenService.RefreshToken();
            //        //telemetryClient.log retried
            //    });
            //var policyRegistry = services.AddPolicyRegistry();
            //policyRegistry.Add("auth_policy",authPolicy);

            services.AddHttpClient<IClaimsClient, ClaimsClient>()
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[] {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1)
                }));

            //services.AddHttpClient("CMS-Get")
            //    .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[] {
            //            TimeSpan.FromSeconds(1),
            //            TimeSpan.FromSeconds(1),
            //            TimeSpan.FromSeconds(1),
            //    }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
