#define DISABLE_SITEPROTECT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using WebApiApplicationService.Handler;

using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApiApplicationService.InternalModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing;
using System.Net;

using System.Reflection;
using WebApiFunction.Mail;
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Application.Controller.Modules;
using WebApiFunction.Application.Model.Internal;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Database.MySQL;
using WebApiFunction.Database.MySQL.Data;
using WebApiFunction.Filter;
using WebApiFunction.Formatter;
using WebApiFunction.LocalSystem.IO.File;
using WebApiFunction.Log;
using WebApiFunction.Metric;
using WebApiFunction.Metric.Influxdb;
using WebApiFunction.MicroService;
using WebApiFunction.Network;
using WebApiFunction.Security;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Threading;
using WebApiFunction.Threading.Service;
using WebApiFunction.Threading.Task;
using WebApiFunction.Utility;
using WebApiFunction.Web;
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Http;
using WebApiFunction.Healthcheck;
using WebApiFunction.Application;
using WebApiFunction.Web.Authentification.JWT;

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
                    MethodDescriptor methodInfo = _webHostEnvironment.IsDevelopment() ? new MethodDescriptor { c = this.GetType().Name, m = MethodBase.GetCurrentMethod().Name } : null;
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
