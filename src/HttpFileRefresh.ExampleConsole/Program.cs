using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.Extensions.Logging;

namespace HttpFileRefresh.ExampleConsole
{
    class Program
    {
        static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            var env = "development";

            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddLogging(configure => {
                configure.AddConsole();
                configure.AddConfiguration(Configuration.GetSection("Logging"));
            });

            services.AddSingleton<HostedServiceExecutor>();
            
            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();             
            var hostedServiceExecutor = serviceProvider.GetRequiredService<HostedServiceExecutor>();

            hostedServiceExecutor.StartAsync(CancellationToken.None).GetAwaiter().GetResult();

        
        }
        public static void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AadAuthOptions>(Configuration.GetSection("auth"));
            services.AddSingleton<AadAuthorizationHandler>();
            services.AddHttpClient("filerefresh").AddHttpMessageHandler<AadAuthorizationHandler>();

            services.Configure<FileRefreshServiceOptions>(Configuration.GetSection("filerefresh"));
            services.AddSingleton<IHostedService, FileRefreshService>();
        }
    }
}
