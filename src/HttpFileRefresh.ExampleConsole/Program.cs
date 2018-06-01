using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HttpFileRefresh.ExampleConsole
{
  class Program
    {
        static IConfiguration Configuration { get; set; }

        public static async Task Main(string[] args)
        {
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-2.1
            var host = new HostBuilder()
            .ConfigureAppConfiguration((context, builder) => {
                builder.AddEnvironmentVariables();
                builder.AddJsonFile("appsettings.json", optional: true);
                builder.AddJsonFile(
                    $"appsettings.{context.HostingEnvironment.EnvironmentName}.json", 
                    optional: true);
                builder.AddCommandLine(args);
            })
            .ConfigureLogging((context, builder) => {
                builder.AddConsole();
                builder.AddDebug();
            })
            .ConfigureServices((context, services) => {
                services.Configure<AadAuthOptions>(Configuration.GetSection("auth"));
                services.AddSingleton<AadAuthorizationHandler>();
                services.AddHttpClient("filerefresh").AddHttpMessageHandler<AadAuthorizationHandler>();

                services.Configure<FileRefreshServiceOptions>(Configuration.GetSection("filerefresh"));
                services.AddSingleton<IHostedService, FileRefreshService>();
            })
            .Build();

            await host.RunAsync();
        }
    }
}
