using Application.Shared.Kernel.Web.AspNet.Startup;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace Application.Web.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var envVarAspNetCoreUrls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
                    var envVarAspNetCoreSslPfxPassword = Environment.GetEnvironmentVariable("ASPNETCORE_SSL_PFX_PASSWORD");
                    var envVarAspNetCoreSslPfxPath = Environment.GetEnvironmentVariable("ASPNETCORE_SSL_PFX_PATH");
                    webBuilder.ConfigureTrafficLayerSecurity(envVarAspNetCoreUrls, envVarAspNetCoreSslPfxPassword, envVarAspNetCoreSslPfxPath);
                    webBuilder.UseStartup<Startup>();
                });
    }
}
