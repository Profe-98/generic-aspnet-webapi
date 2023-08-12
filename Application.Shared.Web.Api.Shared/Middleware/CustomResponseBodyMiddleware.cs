#define DISABLE_SITEPROTECT
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing;
using System.Net;
using System.Reflection;
using Application.Shared.Kernel.Data.Format.Json;
using Application.Shared.Kernel.Data.Web.Api.Abstractions.JsonApiV1;
using Application.Shared.Kernel.Web.Websocket.SignalR.HubService;
using Application.Shared.Kernel.Web.AspNet.Controller;
using Application.Shared.Kernel.Configuration.Const;
using Application.Shared.Kernel.Infrastructure.Cache.Distributed.RedisCache;
using Application.Shared.Kernel.Infrastructure.Ampq.Rabbitmq;
using Application.Shared.Kernel.Application.Model.Internal;

namespace Application.Shared.Web.Api.Shared.Middleware
{
    public class CustomResponseBodyMiddleware : CustomControllerBase
    {
        private readonly IActionSelector _actionSelector;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly ISingletonJsonHandler _jsonHandler;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly RequestDelegate _next;
        public CustomResponseBodyMiddleware(RequestDelegate requestDelegate, IWebHostEnvironment webHostEnvironment, ISingletonJsonHandler jsonHandler, ICachingHandler cachingHandler, IActionSelector actionSelector, IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, Microsoft.Extensions.Configuration.IConfiguration configuration, IRabbitMqHandler rabbitMqHandler) :
            base(webHostEnvironment, cachingHandler, configuration, actionDescriptorCollectionProvider, actionSelector, rabbitMqHandler)
        {
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _actionSelector = actionSelector;

            _next = requestDelegate;
            _jsonHandler = jsonHandler;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var routeContext = new RouteContext(context);
            bool errControllerRequ = context.Request.Path.IsErrorControllerRequest();
            bool hpControllerRequ = context.Request.Path.IsHealthControllerRequest();
            bool isSignalRHubRoute = HubServiceExtensions.RegisteredHubServices.Values?.ToList().Find(x => context.Request.Path.ToString().Contains(x.RouteAttribute.Route)) != null;
            var action = GetMatchingAction(context.Request.Path.Value, context.Request.Method);
            if (!errControllerRequ && !hpControllerRequ && !isSignalRHubRoute)
            {
                if (action == null)//route zu endpoint existiert nicht
                {
                    MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
                    var response = JsonApiErrorResult(new List<ApiErrorModel> {
                new ApiErrorModel{ Code = ApiErrorModel.ERROR_CODES.HTTP_REQU_RESOURCE_NOT_FOUND, Id = Guid.Empty, Detail = BackendAPIDefinitionsProperties.HttpRequestNotFound}
            }, HttpStatusCode.NotFound, "an error occurred", "if (candidates == null || candidates.Count == 0)", methodInfo);

                    var r = await response.AppendToHttpResponse(context.Response);

                    //throw new HttpStatusException(System.Net.HttpStatusCode.NotFound, Models.ApiErrorModel.ERROR_CODES.HTTP_REQU_RESOURCE_NOT_FOUND, BackendAPIDefinitionsProperties.HttpRequestNotFound, "");
                }
                else//route existiert
                {

                }

            }
            await _next(context);
        }
    }

    public static class CustomResponseBodyMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomResponseBodyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomResponseBodyMiddleware>();
        }
    }
}
