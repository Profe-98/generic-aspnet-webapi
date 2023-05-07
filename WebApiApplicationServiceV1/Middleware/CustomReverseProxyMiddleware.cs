using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using WebApiApplicationService.Models;
using WebApiApplicationService.InternalModels;
using Microsoft.AspNetCore.HttpOverrides;

namespace WebApiApplicationService.Middleware
{
    public class CustomReverseProxyMiddleware : Controller
    {

        private readonly RequestDelegate _next;

        public CustomReverseProxyMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            await _next(context);
        }

    }

    public static class CustomReverseProxyMiddlewareExtensions
    {
        public static IApplicationBuilder UseApache2ReverseProxy(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomReverseProxyMiddleware>().UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto});
        }
    }
}
