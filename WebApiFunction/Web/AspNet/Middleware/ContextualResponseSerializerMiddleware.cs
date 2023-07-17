#define DISABLE_SITEPROTECT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using WebApiFunction.Mail;
using WebApiFunction.Data.Web.MIME;
using WebApiFunction.Application.Controller.Modules;
using WebApiFunction.Application.Model.Internal;


using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Data;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Web.AspNet.Filter;
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
using WebApiFunction.Web.AspNet.Healthcheck;
using WebApiFunction.Application;
using WebApiFunction.Web.Authentification.JWT;
using WebApiFunction.Web.AspNet.Controller;

namespace WebApiFunction.Web.AspNet.Middleware
{
    public class ContextualResponseSerializerMiddleware : CustomControllerBase
    {
        private readonly RequestDelegate _next;

        public ContextualResponseSerializerMiddleware(RequestDelegate requestDelegate)
        {
            _next = requestDelegate;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            var claims = context.User.Claims;
            if(context.Response.HasStarted)
            {

            }
        }
    }

    public static class ContextualResponseSerializerMiddlewareExtensions
    {
        public static IApplicationBuilder UseContextualResponseSerializer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ContextualResponseSerializerMiddleware>();
        }
    }
}
