using FunctionApi;
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

[assembly: WebJobsStartup(typeof(Startup))]
namespace FunctionApi
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            RegisterServices(builder.Services);

            builder.AddExtension<InjectConfiguration>();
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddHttpClient<IClaimService, ClaimService>()
                .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new []
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4)
                }));
            services.BuildServiceProvider();
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public class InjectAttribute : Attribute
    {
        public InjectAttribute(Type type)
        {
            Type = type;
        }
        public Type Type { get; }
    }

    public class InjectConfiguration : IExtensionConfigProvider
    {
        private IServiceProvider _serviceProvider;

        // the InjectConfiguration gets called before Initialize
        public InjectConfiguration(IServiceProvider serviceProvider)
        {
            // Use injected Service Provider rather than AutoFac.
            _serviceProvider = serviceProvider;
            Console.WriteLine("InjectConfiguration() completed");
        }

        public void Initialize(ExtensionConfigContext context)
        {
            context
                .AddBindingRule<InjectAttribute>()
                .BindToInput<dynamic>(i => _serviceProvider.GetRequiredService(i.Type));
        }
    }
}
