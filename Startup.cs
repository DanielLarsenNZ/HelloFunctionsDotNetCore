using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

[assembly: FunctionsStartup(typeof(HelloFunctionsDotNetCore.Startup))]

namespace HelloFunctionsDotNetCore
{
    public class Startup : FunctionsStartup
    {
        public Startup() { }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            // all of this to get configuration in Startup :/
            string currentDirectory = builder.Services
                .BuildServiceProvider()
                .GetService<IOptions<ExecutionContextOptions>>()
                .Value.AppDirectory;

            var config = new ConfigurationBuilder()
               .SetBasePath(currentDirectory)
               .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables()
               .Build();

            builder.Services.AddSingleton<IConfiguration>(config);

            if (!string.IsNullOrEmpty(config["Startup.ThrowException"]))
            {
                throw new Exception($"App Setting \"Startup.ThrowException\" is set. Throwing Exception in Startup.Configure. Setting value is {config["Startup.ThrowException"]}");
            }

            builder.Services.AddSingleton((s) =>
            {
                var http = new HttpClient();
                http.Timeout = TimeSpan.FromSeconds(10);

                // User-Agent header
                http.DefaultRequestHeaders.UserAgent.Add(
                    new ProductInfoHeaderValue(new ProductHeaderValue("DanielLarsenNZ-HelloFunctionsDotNetCore")));

                return http;
            });

            if (!string.IsNullOrEmpty(config["Blob1.StorageConnectionString"]))
            {
                builder.Services.AddSingleton(
                    (s) => CloudStorageAccount.Parse(config["Blob1.StorageConnectionString"]).CreateCloudBlobClient());
            }

            if (!string.IsNullOrEmpty(config["Blob2.StorageConnectionString"]))
            {
                builder.Services.AddSingleton(
                    (s) => CloudStorageAccount.Parse(config["Blob2.StorageConnectionString"]).CreateCloudBlobClient());
            }
        }
    }
}
