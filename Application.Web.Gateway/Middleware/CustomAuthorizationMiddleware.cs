using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Responses;
using System.Net.Http;
using System.Threading;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IO;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json.Serialization;
using Ocelot.DownstreamRouteFinder;
using Ocelot.DownstreamRouteFinder.Middleware;
using Ocelot.LoadBalancer;
using Ocelot.LoadBalancer.Middleware;
using Ocelot.LoadBalancer.LoadBalancers;
using Ocelot.DownstreamPathManipulation;
using Ocelot.DownstreamPathManipulation.Middleware;
using Ocelot.DownstreamUrlCreator;
using Ocelot.DownstreamUrlCreator.Middleware;
using Ocelot.Infrastructure.Extensions;
using Ocelot.Request;
using Ocelot.RequestId;
using Ocelot.RequestId.Middleware;
using Ocelot.Responder.Middleware;
using Ocelot.ServiceDiscovery;
using Ocelot.Authentication.Middleware;
using Ocelot.Authorization.Middleware;
using Ocelot.Multiplexer;
using Ocelot.Requester.Middleware;
using Ocelot.DownstreamRouteFinder.UrlMatcher;
using Ocelot.Request.Middleware;
using Ocelot.QueryStrings.Middleware;
using Ocelot.RateLimit.Middleware;
using Ocelot.Security.Middleware;
using Ocelot.Cache.Middleware;
using Ocelot.Claims.Middleware;
using Ocelot.Errors.Middleware;
using Ocelot.Headers.Middleware;
using Microsoft.AspNetCore.Http;
using Ocelot.WebSockets.Middleware;
using Ocelot.Authorization;
using Ocelot.Configuration;
using Ocelot.Errors;
using Ocelot.Logging;
using Application.Shared.Kernel.Data.Format.Json;

namespace Application.Web.Gateway.Middleware
{

    public class CustomAuthorizationMiddleware : OcelotMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IClaimsAuthorizer _claimsAuthorizer;
        private readonly IScopesAuthorizer _scopesAuthorizer;
        private readonly ISingletonJsonHandler _jsonHandler;

        public CustomAuthorizationMiddleware(RequestDelegate next,
            IClaimsAuthorizer claimsAuthorizer,
            IScopesAuthorizer scopesAuthorizer,
            IOcelotLoggerFactory loggerFactory,
            ISingletonJsonHandler jsonHandler)
            : base(loggerFactory.CreateLogger<AuthorizationMiddleware>())
        {
            _next = next;
            _claimsAuthorizer = claimsAuthorizer;
            _scopesAuthorizer = scopesAuthorizer;
            _jsonHandler = jsonHandler;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var downstreamRoute = httpContext.Items.DownstreamRoute();

            if (!IsOptionsHttpMethod(httpContext) && IsAuthenticatedRoute(downstreamRoute))
            {
                Logger.LogInformation("route is authenticated scopes must be checked");

                var authorized = _scopesAuthorizer.Authorize(httpContext.User, downstreamRoute.AuthenticationOptions.AllowedScopes);

                if (authorized.IsError)
                {
                    Logger.LogWarning("error authorizing user scopes");

                    httpContext.Items.UpsertErrors(authorized.Errors);
                    return;
                }

                if (IsAuthorized(authorized))
                {
                    Logger.LogInformation("user scopes is authorized calling next authorization checks");
                }
                else
                {
                    Logger.LogWarning("user scopes is not authorized setting pipeline error");

                    httpContext.Items.SetError(new UnauthorizedError(
                            $"{httpContext.User.Identity.Name} unable to access {downstreamRoute.UpstreamPathTemplate.OriginalValue}"));
                }
            }

            if (!IsOptionsHttpMethod(httpContext) && IsAuthorizedRoute(downstreamRoute))
            {
                Logger.LogInformation("route is authorized");


                //_claimsAuthorizer.Authorize: user muss alle claims enthalten die als RouteClaimsRequirement definiert sind
                var authorized = _claimsAuthorizer.Authorize(httpContext.User, downstreamRoute.RouteClaimsRequirement, httpContext.Items.TemplatePlaceholderNameAndValues());

                if (authorized.IsError)
                {
                    Logger.LogWarning($"Error while authorizing {httpContext.User.Identity.Name}. Setting pipeline error");

                    httpContext.Items.UpsertErrors(authorized.Errors);
                    return;
                }

                if (IsAuthorized(authorized))
                {
                    Logger.LogInformation($"{httpContext.User.Identity.Name} has succesfully been authorized for {downstreamRoute.UpstreamPathTemplate.OriginalValue}.");
                    await _next.Invoke(httpContext);
                }
                else
                {
                    Logger.LogWarning($"{httpContext.User.Identity.Name} is not authorized to access {downstreamRoute.UpstreamPathTemplate.OriginalValue}. Setting pipeline error");

                    httpContext.Items.SetError(new UnauthorizedError($"{httpContext.User.Identity.Name} is not authorized to access {downstreamRoute.UpstreamPathTemplate.OriginalValue}"));
                }
            }
            else
            {
                Logger.LogInformation($"{downstreamRoute.DownstreamPathTemplate.Value} route does not require user to be authorized");
                await _next.Invoke(httpContext);
            }
        }

        private static bool IsAuthorized(Response<bool> authorized)
        {
            return authorized.Data;
        }

        private static bool IsAuthenticatedRoute(DownstreamRoute route)
        {
            return route.IsAuthenticated;
        }

        private static bool IsAuthorizedRoute(DownstreamRoute route)
        {
            return route.IsAuthorized;
        }

        private static bool IsOptionsHttpMethod(HttpContext httpContext)
        {
            return httpContext.Request.Method.ToUpper() == "OPTIONS";
        }
    }
}
