using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Ocelot.Configuration;
using Ocelot.Logging;
using Ocelot.Middleware;
using System.Threading.Tasks;
using Ocelot.DownstreamRouteFinder.Middleware;
using System.Collections.Generic;
using WebApiFunction.Data.Format.Json;

namespace WebApiGateway.Middleware
{
    public class CustomAuthentificationMiddleware : OcelotMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISingletonJsonHandler _jsonHandler;

        public CustomAuthentificationMiddleware(RequestDelegate next,
            IOcelotLoggerFactory loggerFactory,
            ISingletonJsonHandler jsonHandler)
            : base(loggerFactory.CreateLogger<AuthenticationMiddleware>())
        {
            _next = next;
            _jsonHandler = jsonHandler;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var downstreamRoute = httpContext.Items.DownstreamRoute();

            if (httpContext.Request.Method.ToUpper() != "OPTIONS" && IsAuthenticatedRoute(downstreamRoute))
            {
                Logger.LogInformation($"{httpContext.Request.Path} is an authenticated route. {MiddlewareName} checking if client is authenticated");

                var result = await httpContext.AuthenticateAsync(downstreamRoute.AuthenticationOptions.AuthenticationProviderKey);//called den authentificationhandler für das Schema welches in der route.json für die Route definiert ist, Handler muss in ConfigureServices vorhanden/definiert sein
                /*
                 
                 services.AddAuthentication("Base").
                AddScheme<BasicAuthenticationOptions, AuthentificationHandler>("Base",null, new Action<BasicAuthenticationOptions>(o =>
                 
                 
                 */
                httpContext.User = result.Principal;

                if (httpContext.User.Identity.IsAuthenticated)
                {
                    Logger.LogInformation($"Client has been authenticated for {httpContext.Request.Path}");
                    await _next.Invoke(httpContext);
                }
                else
                {
                    var error = new UnauthenticatedError(
                        $"Request for authenticated route {httpContext.Request.Path} by {httpContext.User.Identity.Name} was unauthenticated");

                    Logger.LogWarning($"Client has NOT been authenticated for {httpContext.Request.Path} and pipeline error set. {error}");
                    httpContext.Items.SetError(error);
                }
            }
            else
            {
                Logger.LogInformation($"No authentication needed for {httpContext.Request.Path}");

                await _next.Invoke(httpContext);
            }
        }

        private static bool IsAuthenticatedRoute(DownstreamRoute route)
        {
            return route.IsAuthenticated;
        }
    }
}
