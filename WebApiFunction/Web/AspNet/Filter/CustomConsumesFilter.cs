using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System.Net;
using System.Reflection;
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
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Application.Model.Database.MySQL;
using WebApiFunction.Application.Model.Database.MySQL.Data;
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
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Http;
using WebApiFunction.Web.AspNet.Controller;

namespace WebApiFunction.Web.AspNet.Filter
{
    public class CustomConsumesFilter : Attribute, IAsyncActionFilter, IOrderedFilter
    {
        private readonly ILogger<CustomConsumesFilter> _logger;
        public int Order { get; set; } = int.MinValue + 1;
        public readonly string ContentType = null;
        public readonly string[] OtherContentTypes = null;
        public CustomConsumesFilter()
        {

        }
        public CustomConsumesFilter(ILogger<CustomConsumesFilter> logger)
        {

            _logger = logger;
        }
        public CustomConsumesFilter(string contentType, params string[] otherTypes) : this(new LoggerFactory().CreateLogger<CustomConsumesFilter>())
        {
            ContentType = contentType;

            OtherContentTypes = otherTypes;
        }
        //Bevor Action ausgeführt wird, pre-request-execution um Content-Type zu prüfen
        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.TraceHttpTraffic(MethodBase.GetCurrentMethod(), context.HttpContext, GetType().Name);
            bool returnStarted = context.HttpContext.Response.HasStarted;
            if (!returnStarted)
            {
                string contentType = context.HttpContext.Request.ContentType;
                if (string.IsNullOrEmpty(contentType))
                {

                    var response = CustomControllerBase.JsonApiErrorResultS(new List<ApiErrorModel>
                {
                    new ApiErrorModel{
                 Code =  ApiErrorModel.ERROR_CODES.HTTP_REQU_MEDIA_TYPE_NOT_SUPPORTED,
                 Detail = BackendAPIDefinitionsProperties.HttpRequestWrongContentType
                }
                }, HttpStatusCode.UnsupportedMediaType, "an error occurred", BackendAPIDefinitionsProperties.HttpRequestWrongContentType, null);
                    context.Result = response;
                }
                else
                {
                    contentType = contentType.ToLower();
                    if (!contentType.StartsWith(ContentType.ToLower()) || OtherContentTypes.Length != 0 && OtherContentTypes.ToList().IndexOf(contentType) == GeneralDefs.NotFoundResponseValue)
                    {
                        var response = CustomControllerBase.JsonApiErrorResultS(new List<ApiErrorModel>
                {
                    new ApiErrorModel{
                 Code =  ApiErrorModel.ERROR_CODES.HTTP_REQU_MEDIA_TYPE_NOT_SUPPORTED,
                 Detail = BackendAPIDefinitionsProperties.HttpRequestWrongContentType
                }
                }, HttpStatusCode.UnsupportedMediaType, "an error occurred", BackendAPIDefinitionsProperties.HttpRequestWrongContentType, null);
                        context.Result = response;
                    }
                }
            }
            if (context.Result == null)
            {
                return next();

            }
            return Task.CompletedTask;
        }
    }
}
