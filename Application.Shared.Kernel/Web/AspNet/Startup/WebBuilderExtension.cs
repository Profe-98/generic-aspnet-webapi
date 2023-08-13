using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shared.Kernel.Web.AspNet.Startup
{
    public static class WebBuilderExtension
    {
        public static IWebHostBuilder ConfigureTrafficLayerSecurity(this IWebHostBuilder webBuilder,string aspNetCoreUrls,string aspNetCoreSslPfxPassword,string aspNetCoreSslPfxPath)
        {
            var envVarAspNetCoreUrls = aspNetCoreUrls;
            var envVarAspNetCoreSslPfxPassword = aspNetCoreSslPfxPassword;
            var envVarAspNetCoreSslPfxPath = aspNetCoreSslPfxPath;
            try
            {
                var transformToValidUriStr = envVarAspNetCoreUrls.Replace("+", "tmphostname");
                if (Uri.TryCreate(transformToValidUriStr, UriKind.RelativeOrAbsolute, out Uri result))
                {
                    if (result.Port == 443 && !String.IsNullOrWhiteSpace(envVarAspNetCoreSslPfxPath))
                    {
                        webBuilder.ConfigureKestrel(opt =>
                        {
                            Console.WriteLine("[SSL]: Init ssl encryption process...");
                            if (File.Exists(envVarAspNetCoreSslPfxPath))
                            {
                                try
                                {

                                    Console.WriteLine("[SSL]: Following pfx-file found: " + envVarAspNetCoreSslPfxPath + "");
                                    var port = 443;
                                    // The password you specified when exporting the PFX file using OpenSSL.
                                    // This would normally be stored in configuration or an environment variable;
                                    // I've hard-coded it here just to make it easier to see what's going on.

                                    Console.WriteLine("[SSL]: try to read pfx and setup encryption");
                                    opt.Listen(IPAddress.Any, port, listenOptions => {
                                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
                                        // Configure Kestrel to use a certificate from a local .PFX file for hosting HTTPS
                                        if (String.IsNullOrWhiteSpace(envVarAspNetCoreSslPfxPassword))
                                        {

                                            Console.WriteLine("[SSL-WARN]: try to open pfx without password, environment var 'ASPNETCORE_SSL_PFX_PASSWORD' doesnt filled with any string");
                                        }
                                        listenOptions.UseHttps(envVarAspNetCoreSslPfxPath, envVarAspNetCoreSslPfxPassword);
                                    });
                                    Console.WriteLine("[SSL]: kestrel is now listening on port 443 with ssl encryption");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("[SSL-ERR]: exception happend by reading pfx");
                                }
                            }
                            else
                            {
                                Console.WriteLine("[SSL-ERR]: pfx-file not found: " + envVarAspNetCoreSslPfxPath + "");
                            }




                        });



                    }
                    else
                    {

                        Console.WriteLine("[SSL]: start kestrel without ssl encryption");
                    }
                }
            }
            catch
            {

            }
            return webBuilder;
        }
    }
}
