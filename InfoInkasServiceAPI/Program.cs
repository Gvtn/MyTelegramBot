using InfoInkasServiceAPI.Models.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace InfoInkasService.InfoInkasServiceAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.Sources.Clear();

                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                                         optional: true, reloadOnChange: true);

                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel((builderContext, serverOptions) =>
                    {
                        var config = builderContext.Configuration;
                        var kestrelConfig = new KestrelOptions();
                        config.GetSection("KestrelOptionsConfig").Bind(kestrelConfig);
                        serverOptions.Listen(IPAddress.Parse(kestrelConfig.IpAdress), kestrelConfig.Port);
                        // Set properties and call methods on options
                    })
                    .UseStartup<Startup>();
                });
    }
}