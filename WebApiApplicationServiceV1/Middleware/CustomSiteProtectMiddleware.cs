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

namespace WebApiApplicationService.Middleware
{
    public class CustomSiteProtectMiddleware : CustomControllerBase
    {
        private readonly ISingletonDatabaseHandler _singletonDatabaseHandler;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ISingletonSiteProtectHandler _siteProtectHandler;
        private readonly RequestDelegate _next;

        public CustomSiteProtectMiddleware(RequestDelegate requestDelegate,ISingletonDatabaseHandler singletonDatabaseHandler,IWebHostEnvironment env,ICachingHandler caching,ISingletonSiteProtectHandler siteProtectHandler, Microsoft.Extensions.Configuration.IConfiguration configuration, IRabbitMqHandler rabbitMqHandler) :
            base(env,caching,configuration,rabbitMqHandler)
        {
            _singletonDatabaseHandler = singletonDatabaseHandler;
            _webHostEnvironment = env;
            _next = requestDelegate;
            _siteProtectHandler = siteProtectHandler;
        }
        public async Task InvokeAsync(HttpContext context)
        {
#if !DISABLE_SITEPROTECT
            SiteProtectHandler.ObserveResult result = await _siteProtectHandler.ObserveEndpoint(context,new System.Net.IPEndPoint(context.Connection.RemoteIpAddress, 0),_singletonDatabaseHandler);
            if (result != null)
            {
                bool requestObserved = true;
                bool isDev = _webHostEnvironment.IsDevelopment();
                if (!isDev)
                {
                    requestObserved = result.CanPerformAction;
                }
                if (!result.UserInformation.HasActiveBan)
                {
                    if(requestObserved)
                    {
                        await _next(context);
                    }
                    else
                    {
                        if(result.IsUriTooLong)
                        {

                            throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, Models.ApiErrorModel.ERROR_CODES.HTTP_REQU_URI_TO_LONG, "", "uri is too long");
                        }
                        else if (result.IsHeaderFieldTooLong || result.IsHeaderFieldValueTooLong)
                        {

                            throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, Models.ApiErrorModel.ERROR_CODES.HTTP_REQU_HEADER_FIELD_TO_LARGE, "", "http header-field or header-field value is too long");
                        }
                        else if (result.IsContentLenghtGiven && (result.IsPayloadTooLarge))
                        {

                            throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, Models.ApiErrorModel.ERROR_CODES.HTTP_REQU_PAYLOAD_TO_LARGE, "", "body/content is too long");
                        }
                        else if (result.IsContentLenghtGiven && (result.ContentLenAndBodyLenUnequal))
                        {

                            throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, Models.ApiErrorModel.ERROR_CODES.HTTP_REQU_CONTENT_LEN_REQUIRED, "", "body/content len given but not equal than content-length in http header");
                        }
                        throw new HttpStatusException(System.Net.HttpStatusCode.BadRequest, Models.ApiErrorModel.ERROR_CODES.HTTP_REQU_BAD, "", "operation can not perform");
                    }

                }
                else
                {
                    bool errControllerRequ = context.Request.Path.IsErrorControllerRequest();
                    if (!errControllerRequ)
                        throw new HttpStatusException(System.Net.HttpStatusCode.TooManyRequests, Models.ApiErrorModel.ERROR_CODES.HTTP_REQU_TO_MANY_REQU, "");

                    await _next(context);
                }
            }
            else
            {
                throw new HttpStatusException(System.Net.HttpStatusCode.InternalServerError, Models.ApiErrorModel.ERROR_CODES.INTERNAL, "","");
            }

#else
            if (!context.Response.HasStarted)
                await _next(context);
#endif

        }
    }

    public static class CustomSiteProtectMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomSiteProtectMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomSiteProtectMiddleware>();
        }
    }
}
