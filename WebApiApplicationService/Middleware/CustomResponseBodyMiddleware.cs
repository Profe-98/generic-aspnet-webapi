#define DISABLE_SITEPROTECT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using WebApiApplicationService.Handler;
using WebApiApplicationService.Modules;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApiApplicationService.InternalModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing;
using System.Net;
using WebApiApplicationService.Models;
using System.Reflection;

namespace WebApiApplicationService.Middleware
{
    public class CustomResponseBodyMiddleware : CustomControllerBase
    {
        private readonly IActionSelector _actionSelector;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly ISingletonJsonHandler _jsonHandler;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly RequestDelegate _next;
        public CustomResponseBodyMiddleware(RequestDelegate requestDelegate,IWebHostEnvironment webHostEnvironment, ISingletonJsonHandler jsonHandler,ICachingHandler cachingHandler,IActionSelector actionSelector,IActionDescriptorCollectionProvider actionDescriptorCollectionProvider, Microsoft.Extensions.Configuration.IConfiguration configuration,IRabbitMqHandler rabbitMqHandler) : 
            base(webHostEnvironment,cachingHandler, configuration, actionDescriptorCollectionProvider,actionSelector,rabbitMqHandler)
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
            var action = GetMatchingAction(context.Request.Path.Value, context.Request.Method);
            if(!errControllerRequ && !hpControllerRequ)
            {
                if (action == null)//route zu endpoint existiert nicht
                {
                    WebApiApplicationService.Models.InternalModels.MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new WebApiApplicationService.Models.InternalModels.MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
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
