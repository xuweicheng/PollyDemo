using FunctionApi;
using FunctionApi.IntegrationClient;
using FunctionApi.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Polly;
using System;
using System.Collections.Generic;
using System.Text;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(Startup))]
namespace FunctionApi
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            //This does not work with AddHttpClient<,>()
            //RegisterServices(builder.Services);
            //builder.AddExtension<InjectConfiguration>();

            //From https://blog.wille-zone.de/post/dependency-injection-for-azure-functions/
            builder.AddDependencyInjection(RegisterServices);
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<ITokenService, TokenService>();

            var provider = services.BuildServiceProvider();

            var authPolicy = Policy.HandleResult<string>(result => result == "401")
                .RetryAsync(
                retryCount: 1,
                onRetry: (response, retryNumber, context) =>
                {
                    var tokenService = provider.GetService<ITokenService>();
                    tokenService.RefreshToken();
                    //telemetryClient.log retried
                });
            var policyRegistry = services.AddPolicyRegistry();
            policyRegistry.Add("auth_policy", authPolicy);

            services.AddHttpClient<IClaimsClient, ClaimsClient>()
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new []
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(1)
                }));
        }
    }

    //The following Injection code does not work for AddHttpClient<,>()
    //[AttributeUsage(AttributeTargets.Parameter)]
    //[Binding]
    //public class InjectAttribute : Attribute
    //{
    //    public InjectAttribute(Type type)
    //    {
    //        Type = type;
    //    }
    //    public Type Type { get; }
    //}

    //public class InjectConfiguration : IExtensionConfigProvider
    //{
    //    private IServiceProvider _serviceProvider;

    //    // the InjectConfiguration gets called before Initialize
    //    public InjectConfiguration(IServiceProvider serviceProvider)
    //    {
    //        // Use injected Service Provider rather than AutoFac.
    //        _serviceProvider = serviceProvider;
    //        Console.WriteLine("InjectConfiguration() completed");
    //    }

    //    public void Initialize(ExtensionConfigContext context)
    //    {
    //        context
    //            .AddBindingRule<InjectAttribute>()
    //            .BindToInput<dynamic>(i => _serviceProvider.GetRequiredService(i.Type));
    //    }
    //}
}
