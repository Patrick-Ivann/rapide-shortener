using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
namespace rapide_shortener_service
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // CreateHostBuilder(args).Build().RunAsync();
            // CreateHostBuilderRest(args).Build().Run();

            await Generate(args);
        }

        public static async Task Generate(string[] args)
        {
            await Task.WhenAll(CreateHostBuilder(args).Build().RunAsync(), CreateHostBuilderRest(args).Build().RunAsync());
        }
        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.Sources.Remove(
                config.Sources.First(source =>
                source.GetType() == typeof(EnvironmentVariablesConfigurationSource))); //remove the default one first
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") { config.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), true, true); }
                config.AddEnvironmentVariables(prefix: "RapideShortener_");
            })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        // Setup a HTTP/2 endpoint without TLS.
                        options.ListenAnyIP(5001, o => o.Protocols = HttpProtocols.Http2);
                    });
                    webBuilder.UseStartup<Startup>();
                });
        public static IHostBuilder CreateHostBuilderRest(string[] args) =>
            Host.CreateDefaultBuilder(args)
             .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.Sources.Remove(
                config.Sources.First(source =>
                source.GetType() == typeof(EnvironmentVariablesConfigurationSource))); //remove the default one first
                if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development") { config.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), true, true); }
                config.AddEnvironmentVariables(prefix: "RapideShortener_");
                System.Console.WriteLine(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
            })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        // Setup a HTTP/2 endpoint without TLS.

                        options.ListenAnyIP(3333, o => o.Protocols = HttpProtocols.Http1);
                    });
                    webBuilder.UseStartup<StartupRest>();
                });
    }
}
